using AutoMapper;
using System.Text.RegularExpressions;
using userMS.Application.DTOs;
using userMS.Application.DTOs.Request;
using userMS.Application.DTOs.Response;
using userMS.Application.Enums;
using userMS.Application.Repositories;
using userMS.Application.Services;
using userMS.Domain.Entities;
using userMS.Domain.Exceptions;
using userMS.Infrastructure.Statics;

namespace userMS.Persistence.Services
{
    public class AuthService : IAuthService
    {
        private readonly IRepository<User, Guid> _userRepository;
        private readonly IMapper _mapper;
        private readonly ITokenService _tokenService;
        private readonly IEmailService _emailService;
        private readonly ISmsService _smsService;
        private readonly IOtpService _otpService;
        private readonly IFirebaseAuthService _firebaseAuthService;
        private readonly IRedisCacheService _cacheService;
        private readonly HttpClient _httpClient;

        public AuthService(
            IRepository<User, Guid> userRepository, 
            IMapper mapper, 
            ITokenService tokenService,
            IEmailService emailService,
            ISmsService smsService,
            IOtpService otpService,
            IFirebaseAuthService firebaseAuthService,
            IRedisCacheService cacheService,
            HttpClient httpClient)
        {
            _userRepository = userRepository;
            _mapper = mapper;
            _tokenService = tokenService;
            _emailService = emailService;
            _smsService = smsService;
            _otpService = otpService;
            _firebaseAuthService = firebaseAuthService;
            _cacheService = cacheService;
            _httpClient = httpClient;
        }

        public async Task<FirebaseAuthResponseDto> IdentifierLoginUserAsync(UsernameOrEmailLoginUserDto userLog)
        {
            var email = await GetLoggedInEmailAsync(userLog);

            var user = (await _userRepository.FindByAsync(u => u.Email == email)).FirstOrDefault();

            if (user is null)
                throw new BadRequestException(ErrorMessages.IncorrectIdentifierProvided);

            if (!BCrypt.Net.BCrypt.Verify(userLog.Password, user.Password))
                throw new BadRequestException(ErrorMessages.IncorrectPasswordProvided);

            if (!user.IsEmailVerified)
                throw new BadRequestException(ErrorMessages.EmailIsNotVerified);

            var firebaseResponse = await _firebaseAuthService.FirebaseEmailLoginAsync(
                new FirebaseEmailSignInRequestDto
                {
                    Email = email,
                    Password = userLog.Password,
                });

            // email
            await _emailService.SendLoginEmailAsync(new UserLoginMailRequestDto
            {
                Username = user.UserName,
                Email = email
            });

            // if login successfull , initialize token logic
            var resp = _mapper.Map<LoginResponseDto>(user);

            resp.Token = _tokenService.GenerateToken(resp);

            return firebaseResponse;

        }

        public async Task<FirebaseAuthResponseDto> PhoneLoginUserAsync(PhoneLoginUserDto userLog)
        {
            var user = (await _userRepository.FindByAsync(u => u.PhoneNo == userLog.PhoneNumber)).FirstOrDefault();

            if (user is null)
                throw new BadRequestException(ErrorMessages.IncorrectIdentifierProvided);

            if (!BCrypt.Net.BCrypt.Verify(userLog.Password, user.Password))
                throw new BadRequestException(ErrorMessages.IncorrectPasswordProvided);

            if (!user.IsPhoneNumberVerified)
                throw new BadRequestException(ErrorMessages.PhoneNumberIsNotVerified);

            var firebaseResponse = await _firebaseAuthService.FirebasePhoneLoginAsync(
                new FirebasePhoneSignInRequestDto
                {
                    PhoneNumber = userLog.PhoneNumber,
                });

            // email
            var email = await GetLoggedInEmailAsync(userLog);
            await _emailService.SendLoginEmailAsync(new UserLoginMailRequestDto
            {
                Username = user.UserName,
                Email = email
            });

            // if login successfull , initialize token logic
            var resp = _mapper.Map<LoginResponseDto>(user);

            resp.Token = _tokenService.GenerateToken(resp);

            return firebaseResponse;
        }

        public async Task<FirebaseAuthResponseDto> RegisterUserAsync(RegisterUserDto userReg)
        {
            #region Delete cache entries of OTPs if there are any assigned for provided user

            var emailOtpDeleteResult = await _cacheService.BulkDeleteAsync(
                await _cacheService.GetKeysByPrefix($"OTP:{userReg.Email}"));
            
            var phoneOtpDeleteResult = await _cacheService.BulkDeleteAsync(
                await _cacheService.GetKeysByPrefix($"OTP:{userReg.PhoneNo}"));

            #endregion

            string providedPassword = userReg.Password;

            var user = _mapper.Map<User>(userReg);

            await IsExistIdenticalInfoAsync(user);

            // want -> email doesnt exist or (email exists and provider doesnt contain 'password')
            var userWithSameEmail = (await _userRepository.FindByAsync(r => r.Email == user.Email)).FirstOrDefault();

            if (userWithSameEmail is not null)
            {
                foreach (ProviderData provider in userWithSameEmail.ProviderData)
                {
                    if (provider.Provider == "password")
                        throw new BadRequestException(ErrorMessages.UserIsAlreadySignedInWithSameProvider);
                }
            }

            user.Password = BCrypt.Net.BCrypt.HashPassword(providedPassword);

            var firebaseResponse = await _firebaseAuthService.FirebaseRegisterAsync(
                new FirebaseEmailSignInRequestDto
                {
                    Email = userReg.Email,
                    Password = userReg.Password,
                });

            var providerData = new ProviderData
            {
                FirebaseUid = firebaseResponse.LocalId,
                Identifier = firebaseResponse.Email,
                Provider = "password"
            };

            if (userWithSameEmail is null)
            {
                user.ProviderData.Add(providerData);

                await _userRepository.AddAsync(user);
            }
            else
            {
                userWithSameEmail.ProviderData.Add(providerData);
                userWithSameEmail.Password = user.Password;
                userWithSameEmail.PhoneNo = user.PhoneNo;

                await _userRepository.UpdateAsync(userWithSameEmail);
            }

            #region Email and otp sending logic
            // otp generation
            string otp = await _otpService.GenerateTotp();

            // send register email and otp to the user
            await _emailService.SendRegisterEmailAsync(new UserRegisterMailRequestDto
            {
                Username = userReg.UserName,
                Email = userReg.Email,
                Otp = otp
            });

            // caching otp for further access
            var otpObjectEmail = new 
            {
                TransactionId = userReg.Email + otp,
                UserId = user.Id,
                CreatedAt = DateTime.Now,
                Otp = otp,
                VerificationMethod = VerificationMethods.Email
            };

            var saveResult = await _cacheService.SetAsync($"OTP:{otpObjectEmail.TransactionId}", otpObjectEmail, TimeSpan.FromMinutes(5));

            if (saveResult is not true) throw new BadRequestException(ErrorMessages.EmailOtpCannotBeSaved);
            #endregion

            #region Phone number verification (otp) logic

            //var phoneOtpSendDto = new SendPhoneOtpRequestDto
            //{
            //    PhoneNo = userReg.PhoneNo
            //};

            //await SendPhoneOtpAsync(phoneOtpSendDto);

            #endregion

            return firebaseResponse;
        }

        public async Task<OauthVerificationResponseDto> ExternalProviderOauthLogin(ExternalProviderOauthLoginRequestDto 
            externalProviderOauthLoginRequestDto)
        {
            var requestAccessToken = externalProviderOauthLoginRequestDto.AccessToken;

            var requestProviderId = externalProviderOauthLoginRequestDto.ProviderId;

            #region Generate post body for firebase api call
            var postBody = $"access_token={requestAccessToken}&providerId={requestProviderId}";

            if (externalProviderOauthLoginRequestDto.OauthTokenSecret is not null)
                postBody += $"&oauth_token_secret={externalProviderOauthLoginRequestDto.OauthTokenSecret}";

            #endregion

            #region Firebase oauth sign in
            var oauthVerificationRequestDto = new OauthVerificationRequestDto
            {
                RequestUri = "https://localhost:7010",  // request uri is subject to change
                PostBody = postBody
            };

            var verificationResult = await _firebaseAuthService.FirebaseOauthLoginAsync(oauthVerificationRequestDto);

            // TODO verification result null check

            #endregion

            var userInDb = (await _userRepository.FindByAsync(r => r.Email == verificationResult.Email))
                .FirstOrDefault();

            // if user with the identifier already exists - handle user accordingly
            if (userInDb is not null)
            {
                var providerData = new ProviderData
                {
                    FirebaseUid = verificationResult.LocalId,
                    Identifier = verificationResult.Email,
                    Provider = verificationResult.ProviderId
                };

                userInDb.ProviderData.RemoveAll(r => r.Provider == verificationResult.ProviderId);

                userInDb.ProviderData.Add(providerData);

                await _userRepository.UpdateAsync(userInDb);
            }
            // below part is expected to run if user doesn't exist with its identifier (mostly email)
            else
            {
                var user = new User
                {
                    Email = verificationResult.Email,
                    UserName = verificationResult.DisplayName
                };

                var providerData = new ProviderData
                {
                    FirebaseUid = verificationResult.LocalId,
                    Identifier = verificationResult.Email,
                    Provider = verificationResult.ProviderId
                };

                user.ProviderData.Add(providerData);

                await _userRepository.AddAsync(user);
            }

            return verificationResult;
        }

        public async Task SendEmailOtpAsync(SendEmailOtpRequestDto sendEmailOtpRequestDto)
        {
            var user = (await _userRepository.FindByAsync(r => r.Email == sendEmailOtpRequestDto.Email))
                .FirstOrDefault();

            if (user is null) throw new NotFoundException(ErrorMessages.UserEmailNotFound);

            if (user.IsEmailVerified) throw new BadRequestException(ErrorMessages.EmailIsAlreadyVerified);

            #region Delete cache entries of OTPs if there are any assigned for provided user

            var emailOtpDeleteResult = await _cacheService.BulkDeleteAsync(
                await _cacheService.GetKeysByPrefix($"OTP:{sendEmailOtpRequestDto.Email}"));

            #endregion

            #region otp sending logic
            // otp generation
            string otp = await _otpService.GenerateTotp();

            try
            {
                // sending otp to the user
                await _emailService.SendCustomEmailAsync(new EmailSendRequestDto
                {
                    Subject = "Verify your account",
                    Body = $"Your verification code : {otp}",
                    To = sendEmailOtpRequestDto.Email
                });
            }
            catch
            {
                throw new BadRequestException(ErrorMessages.EmailCouldntBeSent);
            }

            // caching otp for further access
            var otpObjectEmail = new
            {
                TransactionId = sendEmailOtpRequestDto.Email + otp,
                UserId = user.Id,
                CreatedAt = DateTime.Now,
                Otp = otp,
                VerificationMethod = VerificationMethods.Email
            };

            var saveResult = await _cacheService.SetAsync($"OTP:{otpObjectEmail.TransactionId}", otpObjectEmail, TimeSpan.FromMinutes(5));

            if (saveResult is not true) throw new BadRequestException(ErrorMessages.EmailOtpCannotBeSaved);
            #endregion
        }

        public async Task SendPhoneOtpAsync(SendPhoneOtpRequestDto sendPhoneOtpRequestDto)
        {
            var user = (await _userRepository.FindByAsync(r => r.PhoneNo == sendPhoneOtpRequestDto.PhoneNo)).FirstOrDefault();

            if (user is null) throw new NotFoundException(ErrorMessages.UserPhoneNumberNotFound);

            if (user.IsPhoneNumberVerified) throw new BadRequestException(ErrorMessages.PhoneIsAlreadyVerified);

            #region Delete cache entries of OTPs if there are any assigned for provided user

            var emailOtpDeleteResult = await _cacheService.BulkDeleteAsync(
                await _cacheService.GetKeysByPrefix($"OTP:{sendPhoneOtpRequestDto.PhoneNo}"));

            #endregion

            #region Phone number verification (otp) logic

            // otp generation
            string otpPhone = await _otpService.GenerateTotp();

            // sending otp to the user
            try
            {
                await _smsService.SendSmsAsync(new SmsSendRequestDto
                {
                    Body = $"Your verification code : {otpPhone}",
                    To = sendPhoneOtpRequestDto.PhoneNo
                });
            }
            catch
            {
                throw new BadRequestException(ErrorMessages.SmsCouldntBeSent);
            }


            // caching otp for further access
            var otpObjectPhone = new
            {
                TransactionId = sendPhoneOtpRequestDto.PhoneNo + otpPhone,
                UserId = user.Id,
                CreatedAt = DateTime.Now,
                Otp = otpPhone,
                VerificationMethod = VerificationMethods.Phone
            };
            var saveResult = await _cacheService.SetAsync($"OTP:{otpObjectPhone.TransactionId}", otpObjectPhone, TimeSpan.FromMinutes(5));

            if (saveResult is not true) throw new BadRequestException(ErrorMessages.PhoneOtpCannotBeSaved);

            #endregion
        }

        public async Task<bool> VerifyOtpAsync(OtpVerificationRequestDto otpVerificationRequestDto)
        {
            if (!await _cacheService.HasKeyValue($"OTP:{otpVerificationRequestDto.TransactionId}"))
            {
                throw new NotFoundException(ErrorMessages.OtpIsExpired);
            }

            var expectedOtp = await _cacheService.GetAsync<OtpObjectStored>($"OTP:{otpVerificationRequestDto.TransactionId}");

            if (expectedOtp.Otp != otpVerificationRequestDto.Otp)
            {
                throw new BadRequestException(ErrorMessages.OtpIsIncorrect);
            }

            var user = (await _userRepository.FindByAsync(r => r.Id == expectedOtp.UserId)).FirstOrDefault();

            if (user is null) throw new NotFoundException(ErrorMessages.UserNotFound);

            // update verification status of the user
            if (otpVerificationRequestDto.VerificationMethod is VerificationMethods.Email)
                user.IsEmailVerified = true;
            if (otpVerificationRequestDto.VerificationMethod is VerificationMethods.Phone)
                user.IsPhoneNumberVerified = true;

            /* will think about usage of Update method here since it's basically 
              replacing the given entity instead of updating a field */
            await _userRepository.UpdateAsync(user);

            // no need for verified otp anymore
            await _cacheService.DeleteAsync($"OTP:{otpVerificationRequestDto.TransactionId}");
          
            return true;
        }

        public async Task<string> GetLoggedInEmailAsync(UsernameOrEmailLoginUserDto userLog)
        {
            var identifier = userLog.Identifier;

            // if provided identifier is email return it directly
            if (IsEmail(identifier))
            {
                return identifier;
            }

            // if identifier is not email, it means user provided his username
            var loggedUser = (await _userRepository.FindByAsync(u => u.UserName == userLog.Identifier)).FirstOrDefault();

            // if user with the provided username is not found, throw bad credentials
            if (loggedUser is null)
            {
                throw new BadRequestException(ErrorMessages.IncorrectIdentifierProvided);
            }


            return loggedUser.Email;
        }

        public async Task<string> GetLoggedInEmailAsync(PhoneLoginUserDto userLog)
        {
            var identifier = userLog.PhoneNumber;

            // if identifier is not email, it means user provided his username
            var loggedUser = (await _userRepository.FindByAsync(u => u.PhoneNo == identifier)).FirstOrDefault();

            // if user with the provided username is not found, throw bad credentials
            if (loggedUser is null)
            {
                throw new BadRequestException(ErrorMessages.IncorrectIdentifierProvided);
            }

            return loggedUser.Email;
        }

        public bool IsEmail(string identifier)
        {
            Regex emailRegex = new Regex(@"^[a-zA-Z0-9._-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,4}$");

            if (emailRegex.IsMatch(identifier))
            {
                return true;
            }

            return false;
        }

        public async Task IsExistIdenticalInfoAsync(User user)
        {
            var userNameExists = await _userRepository.AnyAsync(u => u.UserName == user.UserName);
            var phoneNoExists = await _userRepository.AnyAsync(u => u.PhoneNo == user.PhoneNo);

            if (userNameExists)
            {
                throw new DuplicateEntityException(nameof(User), nameof(User.UserName), user.UserName);
            }
            else if (phoneNoExists)
            {
                throw new DuplicateEntityException(nameof(User), nameof(User.PhoneNo), user.PhoneNo);
            }
        }
    }
}

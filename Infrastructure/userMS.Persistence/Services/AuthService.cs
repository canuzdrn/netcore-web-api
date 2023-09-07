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
        private readonly IFirebaseAuthService _firebaseAuthService;
        private readonly IRedisCacheService _cacheService;

        public AuthService(
            IRepository<User, Guid> userRepository, 
            IMapper mapper, 
            ITokenService tokenService,
            IEmailService emailService,
            IFirebaseAuthService firebaseAuthService,
            IRedisCacheService cacheService)
        {
            _userRepository = userRepository;
            _mapper = mapper;
            _tokenService = tokenService;
            _emailService = emailService;
            _firebaseAuthService = firebaseAuthService;
            _cacheService = cacheService;
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
            await _emailService.SendLoginEmailAsync(email);

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
            await _emailService.SendLoginEmailAsync(email);

            // if login successfull , initialize token logic
            var resp = _mapper.Map<LoginResponseDto>(user);

            resp.Token = _tokenService.GenerateToken(resp);

            return firebaseResponse;
        }

        public async Task<FirebaseAuthResponseDto> RegisterUserAsync(RegisterUserDto userReg)
        {
            string providedPassword = userReg.Password;

            var user = _mapper.Map<User>(userReg);

            await IsExistIdenticalInfoAsync(user);

            user.Password = BCrypt.Net.BCrypt.HashPassword(providedPassword);

            var firebaseResponse = await _firebaseAuthService.FirebaseRegisterAsync(
                new FirebaseEmailSignInRequestDto
                {
                    Email = userReg.Email,
                    Password = userReg.Password,
                });

            await _userRepository.AddAsync(user);

            #region Email and otp sending logic
            // welcome email
            await _emailService.SendRegisterEmailAsync(userReg.Email);

            // otp generation
            Random random = new Random();
            int otp = random.Next(100000, 999999);
            
            // sending otp to the user
            await _emailService.SendCustomEmailAsync(new EmailSendRequestDto
            {
                Subject = "Verify your account",
                Body = $"Your verification code : {otp}",
                To = userReg.Email
            });

            // caching otp for further access
            var otpObject = new
            {
                TransactionId = Guid.NewGuid(),
                UserId = user.Id,
                CreatedAt = DateTime.Now,
                Otp = otp,
                VerificationMethod = VerificationMethods.Email
            };

            var saveResult = await _cacheService.SetAsync($"OTP:{otpObject.TransactionId}",otpObject, TimeSpan.FromMinutes(5));

            if (saveResult is not true) throw new BadRequestException(ErrorMessages.OtpCannotBeSaved);
            #endregion

            return firebaseResponse;
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
            var emailExists = await _userRepository.AnyAsync(u => u.Email == user.Email);

            if (userNameExists)
            {
                throw new DuplicateEntityException(nameof(User), nameof(User.UserName), user.UserName);
            }
            else if (phoneNoExists)
            {
                throw new DuplicateEntityException(nameof(User), nameof(User.PhoneNo), user.PhoneNo);
            }
            else if (emailExists)
            {
                throw new DuplicateEntityException(nameof(User), nameof(User.Email), user.Email);
            }

        }
    }
}

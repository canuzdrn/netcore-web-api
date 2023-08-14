﻿using AutoMapper;
using System.Text.RegularExpressions;
using userMS.Application.DTOs;
using userMS.Application.Repositories;
using userMS.Application.Services;
using userMS.Domain.Entities;
using userMS.Domain.Exceptions;
using userMS.Infrastructure.Statics;

namespace userMS.Persistence.Services
{
    public class AuthService : IAuthService
    {
        private readonly IRepository<User, Guid> _repository;
        private readonly IMapper _mapper;
        private readonly ITokenService _tokenService;

        public AuthService(IRepository<User, Guid> repository, IMapper mapper, ITokenService tokenService)
        {
            _repository = repository;
            _mapper = mapper;
            _tokenService = tokenService;
        }

        public async Task<LoginResponseDto> LoginUserAsync(LoginUserDto userLog)
        {
            var email = await GetLoggedInEmailAsync(userLog);

            var userExist = await _repository.AnyAsync(u => u.Email == email);

            if (!userExist)
                throw new BadRequestException(ErrorMessages.IncorrectUsernameProvided);

            var result = await _repository.FindByAsync(u => u.Email == email);

            var user = result.FirstOrDefault();

            if (!BCrypt.Net.BCrypt.Verify(userLog.Password, user.Password))
                throw new BadRequestException(ErrorMessages.IncorrectPasswordProvided);

            // if login successfull , initialize token logic

            var resp = _mapper.Map<LoginResponseDto>(user);

            resp.Token = _tokenService.GenerateToken(resp);

            return resp;

        }

        public async Task<RegisterUserDto> RegisterUserAsync(RegisterUserDto userReg)
        {
            string providedPassword = userReg.Password;

            var user = _mapper.Map<User>(userReg);

            await IsExistIdenticalInfoAsync(user);

            user.Password = BCrypt.Net.BCrypt.HashPassword(providedPassword);

            await _repository.AddAsync(user);

            return _mapper.Map<RegisterUserDto>(user);
        }

        public async Task<string> GetLoggedInEmailAsync(LoginUserDto userLog)
        {
            var identifier = userLog.Identifier;

            // if provided identifier is email return it directly
            if (IsEmail(identifier))
            {
                return identifier;
            }

            // if identifier is not email, it means user provided his username
            var loggedUser = await _repository.FindByAsync(u => u.UserName == userLog.Identifier);

            // if user with the provided username is not found, throw bad credentials
            if (!loggedUser.Any())
            {
                throw new BadRequestException(ErrorMessages.IncorrectUsernameProvided);
            }


            return loggedUser.FirstOrDefault().Email;
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
            var userNameExists = await _repository.AnyAsync(u => u.UserName == user.UserName);
            var phoneNoExists = await _repository.AnyAsync(u => u.PhoneNo == user.PhoneNo);
            var emailExists = await _repository.AnyAsync(u => u.Email == user.Email);

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

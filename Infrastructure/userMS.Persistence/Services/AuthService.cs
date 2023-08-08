﻿using AutoMapper;
using userMS.Application.DTOs;
using userMS.Application.Repositories;
using userMS.Application.Services;
using userMS.Domain.Entities;
using userMS.Domain.Exceptions;

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
            var userExist = await _repository.AnyAsync(u => u.Email == userLog.Email);

            if (!userExist)
                throw new BadRequestException("Invalid Credentials, user does not exist !");

            var result = await _repository.FindByAsync(u => u.Email == userLog.Email);

            var user = result.FirstOrDefault();

            if (user.Password != userLog.Password)
                throw new BadRequestException("Invalid Credentials, incorrect password !");

            // if login successfull , initial token logic

            var resp = _mapper.Map<LoginResponseDto>(user);

            resp.Token = _tokenService.GenerateToken(resp);

            return resp;

        }

        public async Task<RegisterUserDto> RegisterUserAsync(RegisterUserDto userReg)
        {
            var user = _mapper.Map<User>(userReg);

            await IsExistIdenticalInfoAsync(user);

            await _repository.AddAsync(user);

            return _mapper.Map<RegisterUserDto>(user);
        }

        public async Task IsExistIdenticalInfoAsync(User user)
        {
            var userNameExists = await _repository.AnyAsync(u => u.UserName == user.UserName);
            var phoneNoExists = await _repository.AnyAsync(u => u.PhoneNo == user.PhoneNo);
            var emailExists = await _repository.AnyAsync(u => u.Email == user.Email);

            if (userNameExists)
            {
                throw new BadRequestException("There exist a user with the same username !");
            }
            else if (phoneNoExists)
            {
                throw new BadRequestException("There exist a user with the same phone number !");
            }
            else if (emailExists)
            {
                throw new BadRequestException("There exist a user with the same email address !");
            }

        }
    }
}

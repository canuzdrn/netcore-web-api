﻿using AutoMapper;
using System.Linq.Expressions;
using userMS.Application.DTOs;
using userMS.Application.Repositories;
using userMS.Application.Services;
using userMS.Domain.Entities;
using userMS.Domain.Exceptions;
using userMS.Infrastructure.Statics;

namespace userMS.Persistence.Services
{
    public class UserService : IUserService
    {
        private readonly IRepository<User, Guid> _repository;
        private readonly IRedisCacheService _cache;
        private readonly IMapper _mapper;

        #region cache prefixes
        private readonly string _idPrefix = "USER:ID";
        private readonly string _phonePrefix = "USER:PHONE";
        private readonly string _emailPrefix = "USER:EMAIL";
        private readonly string _usernamePrefix = "USER:USERNAME";
        #endregion

        public UserService(IRepository<User, Guid> repository, IRedisCacheService cache,IMapper mapper)
        {
            _repository = repository;
            _cache = cache;
            _mapper = mapper;
        }

        public async Task<UserDto> AddUserAsync(UserDto userDto)
        {
            var user = _mapper.Map<User>(userDto);

            await IsExistIdenticalInfo(user);

            await _repository.AddAsync(user);

            #region cache newly accessed user
            var lifeTime = TimeSpan.FromMinutes(10);
            await _cache.SetAsync<User>($"{_idPrefix}:{user.Id}", user, lifeTime);
            await _cache.SetAsync<User>($"{_phonePrefix}:{user.PhoneNo}", user, lifeTime);
            await _cache.SetAsync<User>($"{_emailPrefix}:{user.Email}", user, lifeTime);
            await _cache.SetAsync<User>($"{_usernamePrefix}:{user.UserName}", user, lifeTime);
            #endregion

            return _mapper.Map<UserDto>(user);
        }

        public async Task<IEnumerable<UserDto>> AddUsersAsync(IEnumerable<UserDto> userDtos)
        {
            var users = _mapper.Map<List<User>>(userDtos);

            bool allUnique = !users.GroupBy(u => u.Id).Any(g => g.Count() > 1)
                && !users.GroupBy(u => u.UserName).Any(g => g.Count() > 1)
                && !users.GroupBy(u => u.Email).Any(g => g.Count() > 1)
                && !users.GroupBy(u => u.PhoneNo).Any(g => g.Count() > 1);

            if (!allUnique)
            {
                throw new DuplicateEntityException(ErrorMessages.User.IdenticalInfoInBulk);
            }

            foreach (User user in users)
            {
                // ensures id, username, email, phone no to be unique
                await IsExistIdenticalInfo(user);
            }

            var result = await _repository.AddRangeAsync(users);

            return _mapper.Map<List<UserDto>>(users);
        }

        public async Task<bool> DeleteUserAsync(UserDto userDto)
        {
            var user = _mapper.Map<User>(userDto);

            // need additional check since there may exist a user
            // with the given id but not with the given user info such as
            // username, email address etc.
            var isExists = await IsUserExists(user);

            if (!isExists)
            {
                throw new NotFoundException(ErrorMessages.User.UserNotFound);
            }

            #region delete user if it is cached
            await _cache.DeleteAsync($"{_idPrefix}:{user.Id}");
            await _cache.DeleteAsync($"{_phonePrefix}:{user.PhoneNo}");
            await _cache.DeleteAsync($"{_emailPrefix}:{user.Email}");
            await _cache.DeleteAsync($"{_usernamePrefix}:{user.UserName}");
            #endregion

            return await _repository.DeleteAsync(user);
        }

        public async Task<bool> DeleteUserByIdAsync(Guid id)
        {
            var user = _repository.GetById(id);

            if (user is null)
            {
                throw new NotFoundException(ErrorMessages.User.UserIdNotFound);
            }

            #region delete user if it is cached
            await _cache.DeleteAsync($"{_idPrefix}:{user.Id}");
            await _cache.DeleteAsync($"{_phonePrefix}:{user.PhoneNo}");
            await _cache.DeleteAsync($"{_emailPrefix}:{user.Email}");
            await _cache.DeleteAsync($"{_usernamePrefix}:{user.UserName}");
            #endregion

            var deleteResult = await _repository.DeleteByIdAsync(id);

            return deleteResult;
        }

        public async Task<bool> DeleteUsersAsync(IEnumerable<UserDto> userDtos)
        {
            // TODO : Add range caching (delete)
            var users = _mapper.Map<List<User>>(userDtos);

            foreach (User user in users)
            {
                if (!(await IsUserExists(user)))
                {
                    throw new NotFoundException(ErrorMessages.User.UserNotFoundInBulk);
                }
            }

            return await _repository.DeleteRangeAsync(users);
        }

        public async Task<IEnumerable<User>> FindByAsync(Expression<Func<User, bool>> predicate)
        {
            return await _repository.FindByAsync(predicate);
        }

        public async Task<IEnumerable<UserDto>> GetAllUsersAsync()
        {
            var users = await _repository.GetAllAsync();

            return _mapper.Map<List<UserDto>>(users);
        }

        public async Task<UserDto> GetUserByUsernameAsync(string username)
        {
            var userFromCache = await _cache.GetAsync<User>($"{_usernamePrefix}:{username}");

            if (userFromCache is not null)
                return _mapper.Map<UserDto>(userFromCache);

            var filterResult = await _repository.FindByAsync(u => u.UserName == username);
            var user = filterResult.FirstOrDefault();

            if(user is null)
            {
                throw new NotFoundException(ErrorMessages.User.UsernameNotFound);
            }

            #region cache newly accessed user
            await _cache.SetAsync<User>($"{_idPrefix}:{user.Id}", user, TimeSpan.FromMinutes(10));
            await _cache.SetAsync<User>($"{_phonePrefix}:{user.PhoneNo}", user, TimeSpan.FromMinutes(10));
            await _cache.SetAsync<User>($"{_emailPrefix}:{user.Email}", user, TimeSpan.FromMinutes(10));
            await _cache.SetAsync<User>($"{_usernamePrefix}:{user.UserName}", user, TimeSpan.FromMinutes(10));
            #endregion


            return _mapper.Map<UserDto>(user);
        }

        public async Task<UserDto> GetUserByEmailAddressAsync(string email)
        {
            var userFromCache = await _cache.GetAsync<User>($"{_emailPrefix}:{email}");

            if (userFromCache is not null)
                return _mapper.Map<UserDto>(userFromCache);

            var filterResult = await _repository.FindByAsync(u => u.Email == email);
            var user = filterResult.FirstOrDefault();

            if (user == null)
            {
                throw new NotFoundException(ErrorMessages.User.UserEmailNotFound);
            }

            #region cache newly accessed user
            await _cache.SetAsync<User>($"{_idPrefix}:{user.Id}", user, TimeSpan.FromMinutes(10));
            await _cache.SetAsync<User>($"{_phonePrefix}:{user.PhoneNo}", user, TimeSpan.FromMinutes(10));
            await _cache.SetAsync<User>($"{_emailPrefix}:{user.Email}", user, TimeSpan.FromMinutes(10));
            await _cache.SetAsync<User>($"{_usernamePrefix}:{user.UserName}", user, TimeSpan.FromMinutes(10));
            #endregion

            return _mapper.Map<UserDto>(user);
        }

        public async Task<UserDto> GetUserByIdAsync(Guid id)
        {
            // if user is cached, retrieve it from the cache
            var userFromCache = await _cache.GetAsync<User>($"{_idPrefix}:{id}");

            if (userFromCache is not null)
                return _mapper.Map<UserDto>(userFromCache);

            // Thread.Sleep(10000); --> can be used as a test whether data resides in cache or not

            var user = await _repository.GetByIdAsync(id);

            if (user == null)
            {
                throw new NotFoundException(ErrorMessages.User.UserIdNotFound);
            }

            #region cache newly accessed user
            await _cache.SetAsync<User>($"{_idPrefix}:{user.Id}", user, TimeSpan.FromMinutes(10));
            await _cache.SetAsync<User>($"{_phonePrefix}:{user.PhoneNo}", user, TimeSpan.FromMinutes(10));
            await _cache.SetAsync<User>($"{_emailPrefix}:{user.Email}", user, TimeSpan.FromMinutes(10));
            await _cache.SetAsync<User>($"{_usernamePrefix}:{user.UserName}", user, TimeSpan.FromMinutes(10));
            #endregion

            return _mapper.Map<UserDto>(user);
        }

        public async Task<UserDto> GetUserByPhoneNumberAsync(string phoneNo)
        {
            var userFromCache = await _cache.GetAsync<User>($"{_phonePrefix}:{phoneNo}");

            if (userFromCache is not null)
                return _mapper.Map<UserDto>(userFromCache);

            var filterResult = await _repository.FindByAsync(u => u.PhoneNo == phoneNo);
            var user = filterResult.FirstOrDefault();

            if (user == null)
            {
                throw new NotFoundException(ErrorMessages.User.UserPhoneNumberNotFound);
            }

            #region cache newly accessed user
            await _cache.SetAsync<User>($"{_idPrefix}:{user.Id}", user, TimeSpan.FromMinutes(10));
            await _cache.SetAsync<User>($"{_phonePrefix}:{user.PhoneNo}", user, TimeSpan.FromMinutes(10));
            await _cache.SetAsync<User>($"{_emailPrefix}:{user.Email}", user, TimeSpan.FromMinutes(10));
            await _cache.SetAsync<User>($"{_usernamePrefix}:{user.UserName}", user, TimeSpan.FromMinutes(10));
            #endregion

            return _mapper.Map<UserDto>(user);
        }

        public async Task<UserDto> UpdateUserAsync(UserDto userDto)
        {
            var user = _mapper.Map<User>(userDto);

            var isExist = await _repository.AnyAsync(u => u.Id == user.Id);

            if (!isExist)
            {
                throw new NotFoundException(ErrorMessages.User.UserNotFound);
            }

            #region delete (former) user if it is cached
            await _cache.DeleteAsync($"{_idPrefix}:{user.Id}");
            await _cache.DeleteAsync($"{_phonePrefix}:{user.PhoneNo}");
            await _cache.DeleteAsync($"{_emailPrefix}:{user.Email}");
            await _cache.DeleteAsync($"{_usernamePrefix}:{user.UserName}");
            #endregion

            var updatedUser = await _repository.UpdateAsync(user);

            #region cache newly updated user
            await _cache.SetAsync<User>($"{_idPrefix}:{user.Id}", user, TimeSpan.FromMinutes(10));
            await _cache.SetAsync<User>($"{_phonePrefix}:{user.PhoneNo}", user, TimeSpan.FromMinutes(10));
            await _cache.SetAsync<User>($"{_emailPrefix}:{user.Email}", user, TimeSpan.FromMinutes(10));
            await _cache.SetAsync<User>($"{_usernamePrefix}:{user.UserName}", user, TimeSpan.FromMinutes(10));
            #endregion

            return _mapper.Map<UserDto>(updatedUser);
        }

        public async Task<IEnumerable<UserDto>> UpdateUsersAsync(IEnumerable<UserDto> userDtos)
        {
            // TODO : Add caching on bulk update

            var users = _mapper.Map<List<User>>(userDtos);
            
            // if every id of the provided users is not unique
            // there exist a duplicate key among them
            if (users.GroupBy(u => u.Id).Any(g => g.Count() > 1))
            {
                throw new BadRequestException(ErrorMessages.User.DuplicateUserInBulkUpdate);
            }

            bool allUnique = !users.GroupBy(u => u.UserName).Any(g => g.Count() > 1)
                && !users.GroupBy(u => u.Email).Any(g => g.Count() > 1)
                && !users.GroupBy(u => u.PhoneNo).Any(g => g.Count() > 1);

            if (!allUnique)
            {
                throw new DuplicateEntityException(ErrorMessages.User.IdenticalInfoInBulk);
            }

            foreach (User user in users)
            {
                if (!(await _repository.AnyAsync(u => u.Id == user.Id)))
                {
                    throw new NotFoundException(ErrorMessages.User.UserNotFoundInBulk);
                }
            }

            var updatedUsers = await _repository.UpdateRangeAsync(users);

            return _mapper.Map<List<UserDto>>(updatedUsers);
        }

        public async Task IsExistIdenticalInfo(User user)
        {
            var idExists = await _repository.AnyAsync(u => u.Id == user.Id);
            var userNameExists = await _repository.AnyAsync(u => u.UserName == user.UserName);
            var phoneNoExists = await _repository.AnyAsync(u => u.PhoneNo == user.PhoneNo);
            var emailExists = await _repository.AnyAsync(u => u.Email == user.Email);

            if (idExists)
            {
                throw new DuplicateEntityException(nameof(User), nameof(User.Id), user.Id.ToString());
            }
            else if (userNameExists)
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

        public async Task<bool> IsUserExists(User user)
        {
            var isExists = await _repository.AnyAsync(u => u.Id == user.Id && u.UserName == user.UserName
            && u.PhoneNo == user.PhoneNo && u.Email == user.Email);

            if (isExists)
            {
                return true;
            }

            return false;
        }
    }
}

using AutoMapper;
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
        private readonly IMapper _mapper;

        public UserService(IRepository<User, Guid> repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<UserDto> AddUserAsync(UserDto userDto)
        {
            var user = _mapper.Map<User>(userDto);

            await IsExistIdenticalInfo(user);

            await _repository.AddAsync(user);

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
                throw new DuplicateEntityException(ErrorMessages.IdenticalInfoInBulk);
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
                throw new NotFoundException(ErrorMessages.UserNotFound);
            }

            return await _repository.DeleteAsync(user);
        }

        public async Task<bool> DeleteUserByIdAsync(Guid id)
        {
            var deleteResult = await _repository.DeleteByIdAsync(id);

            if (!deleteResult)
            {
                throw new NotFoundException(ErrorMessages.UserIdNotFound);
            }

            return deleteResult;
        }

        public async Task<bool> DeleteUsersAsync(IEnumerable<UserDto> userDtos)
        {
            var users = _mapper.Map<List<User>>(userDtos);

            foreach (User user in users)
            {
                if (!(await IsUserExists(user)))
                {
                    throw new NotFoundException(ErrorMessages.UserNotFoundInBulk);
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
            var filterResult = await _repository.FindByAsync(u => u.UserName == username);
            var user = filterResult.FirstOrDefault();

            if(user == null)
            {
                throw new NotFoundException(ErrorMessages.UsernameNotFound);
            }

            return _mapper.Map<UserDto>(user);
        }

        public async Task<UserDto> GetUserByEmailAddressAsync(string email)
        {
            var filterResult = await _repository.FindByAsync(u => u.Email == email);
            var user = filterResult.FirstOrDefault();

            if (user == null)
            {
                throw new NotFoundException(ErrorMessages.UserEmailNotFound);
            }

            return _mapper.Map<UserDto>(user);
        }

        public async Task<UserDto> GetUserByIdAsync(Guid id)
        {
            var user = await _repository.GetByIdAsync(id);

            if (user == null)
            {
                throw new NotFoundException(ErrorMessages.UserIdNotFound);
            }

            return _mapper.Map<UserDto>(user);
        }

        public async Task<UserDto> GetUserByPhoneNumberAsync(string phoneNo)
        {
            var filterResult = await _repository.FindByAsync(u => u.PhoneNo == phoneNo);
            var user = filterResult.FirstOrDefault();

            if (user == null)
            {
                throw new NotFoundException(ErrorMessages.UserPhoneNumberNotFound);
            }

            return _mapper.Map<UserDto>(user);
        }

        public async Task<UserDto> UpdateUserAsync(UserDto userDto)
        {
            var user = _mapper.Map<User>(userDto);

            var isExist = await _repository.AnyAsync(u => u.Id == user.Id);

            if (!isExist)
            {
                throw new NotFoundException(ErrorMessages.UserNotFound);
            }

            var updatedUser = await _repository.UpdateAsync(user);

            return _mapper.Map<UserDto>(updatedUser);
        }

        public async Task<IEnumerable<UserDto>> UpdateUsersAsync(IEnumerable<UserDto> userDtos)
        {
            var users = _mapper.Map<List<User>>(userDtos);
            
            // if every id of the provided users is not unique
            // there exist a duplicate key among them
            if (users.GroupBy(u => u.Id).Any(g => g.Count() > 1))
            {
                throw new BadRequestException(ErrorMessages.DuplicateUserInBulkUpdate);
            }

            bool allUnique = !users.GroupBy(u => u.UserName).Any(g => g.Count() > 1)
                && !users.GroupBy(u => u.Email).Any(g => g.Count() > 1)
                && !users.GroupBy(u => u.PhoneNo).Any(g => g.Count() > 1);

            if (!allUnique)
            {
                throw new DuplicateEntityException(ErrorMessages.IdenticalInfoInBulk);
            }

            foreach (User user in users)
            {
                if (!(await _repository.AnyAsync(u => u.Id == user.Id)))
                {
                    throw new NotFoundException(ErrorMessages.UserNotFoundInBulk);
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

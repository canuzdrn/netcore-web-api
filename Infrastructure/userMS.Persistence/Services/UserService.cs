using AutoMapper;
using System.Linq.Expressions;
using userMS.Application.DTOs;
using userMS.Application.Repositories;
using userMS.Application.Services;
using userMS.Domain.Entities;
using userMS.Domain.Exceptions;

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
                throw new BadRequestException("Multiple users cannot share a unique property !");
            }

            foreach (User user in users)
            {
                // ensures id, username, email, phone no to be unique
                await IsExistIdenticalInfo(user);
            }

            await _repository.AddRangeAsync(users);

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
                throw new NotFoundException("Provided user with the given information does not exist !");
            }

            return await _repository.DeleteAsync(user);
        }

        public async Task<bool> DeleteUserByIdAsync(string id)
        {
            var guid = Guid.Parse(id);

            var deleteResult = await _repository.DeleteByIdAsync(guid);

            if (!deleteResult)
            {
                throw new NotFoundException("User with the provided Id is does not exist !");
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
                    throw new NotFoundException("At least one user from the provided user list does not exist !");
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
                throw new NotFoundException("User with the provided username does not exist !");
            }

            return _mapper.Map<UserDto>(user);
        }

        public async Task<UserDto> GetUserByEmailAddressAsync(string email)
        {
            var filterResult = await _repository.FindByAsync(u => u.Email == email);
            var user = filterResult.FirstOrDefault();

            if (user == null)
            {
                throw new NotFoundException("User with the provided email address does not exist !");
            }

            return _mapper.Map<UserDto>(user);
        }

        public async Task<UserDto> GetUserByIdAsync(string id)
        {
            var guid = Guid.Parse(id);

            var user = await _repository.GetByIdAsync(guid);

            if (user == null)
            {
                throw new NotFoundException("User with the provided Id does not exist !");
            }

            return _mapper.Map<UserDto>(user);
        }

        public async Task<UserDto> GetUserByPhoneNumberAsync(string phoneNo)
        {
            var filterResult = await _repository.FindByAsync(u => u.PhoneNo == phoneNo);
            var user = filterResult.FirstOrDefault();

            if (user == null)
            {
                throw new NotFoundException("User with the provided phone number does not exist !");
            }

            return _mapper.Map<UserDto>(user);
        }

        public async Task<UserDto> UpdateUserAsync(UserDto userDto)
        {
            var user = _mapper.Map<User>(userDto);

            var isExist = await _repository.AnyAsync(u => u.Id == user.Id);

            if (!isExist)
            {
                throw new NotFoundException("User tried to be updated does not exist !");
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
                throw new BadRequestException("Cannot update the same user multiple times in a single operation !");
            }

            foreach (User user in users)
            {
                if (!(await _repository.AnyAsync(u => u.Id == user.Id)))
                {
                    throw new NotFoundException("At least one of the provided users does not exist !");
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
                throw new BadRequestException("There exist a user with the same Id !");
            }
            else if (userNameExists)
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

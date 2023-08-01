using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using userMS.Application.Repositories;
using userMS.Application.Services;
using userMS.Domain.Entities;

namespace userMS.Persistence.Services
{
    public class UserService : IUserService
    {
        private readonly IRepository<User, string> _repository;

        public UserService(IRepository<User, string> repository)
        {
            _repository = repository;
        }


        public async Task<User> AddUserAsync(User user)
        {
            // if no error is thrown user can be registered (boolean might be used)
            await IsExistIdenticalInfo(user);

            return await _repository.AddAsync(user);
        }

        public async Task<IEnumerable<User>> AddUsersAsync(IEnumerable<User> users)
        {
            foreach (User user in users)
            {
                await IsExistIdenticalInfo(user);
            }
            return await _repository.AddRangeAsync(users);
        }

        public async Task<bool> DeleteUserAsync(User user)
        {
            // need additional check since there may exist a user
            // with the given id but not with the given user info such as
            // username, email address etc.
            var isExists = await IsUserExists(user);
            if (!isExists)
            {
                return false;
            }

            return await _repository.DeleteAsync(user);
        }

        public async Task<bool> DeleteUserByIdAsync(string id)
        {
            return await _repository.DeleteByIdAsync(id);
        }

        public async Task<bool> DeleteUsersAsync(IEnumerable<User> users)
        {
            foreach (User user in users)
            {
                if (!(await IsUserExists(user)))
                {
                    return false;
                }
            }
            return await _repository.DeleteRangeAsync(users);
        }

        public async Task<IEnumerable<User>> FindByAsync(Expression<Func<User, bool>> predicate)
        {
            return await _repository.FindByAsync(predicate);
        }

        public async Task<IEnumerable<User>> GetAllUsersAsync()
        {
            return await _repository.GetAllAsync();
        }

        public async Task<User> GetUserByUsernameAsync(string username)
        {
            var filterResult = await _repository.FindByAsync(u => u.UserName == username);
            var user = filterResult.FirstOrDefault();
            return user;
        }

        public async Task<User> GetUserByEmailAddressAsync(string email)
        {
            var filterResult = await _repository.FindByAsync(u => u.Email == email);
            var user = filterResult.FirstOrDefault();
            return user;
        }

        public async Task<User> GetUserByIdAsync(string id)
        {
            return await _repository.GetByIdAsync(id);
        }

        public async Task<User> GetUserByPhoneNumberAsync(string phoneNo)
        {
            var filterResult = await _repository.FindByAsync(u => u.PhoneNo == phoneNo);
            var user = filterResult.FirstOrDefault();
            return user;
        }

        public async Task<User> UpdateUserAsync(User user)
        {
            var isExist = await IsUserExists(user);

            if (!isExist)
            {
                return null;
            }

            return await _repository.UpdateAsync(user);
        }

        public async Task<IEnumerable<User>> UpdateUsersAsync(IEnumerable<User> users)
        {
            foreach (User user in users)
            {
                if (!(await IsUserExists(user)))
                {
                    throw new ArgumentException("At least one of the provided users does not exist !");
                }
            }

            return await _repository.UpdateRangeAsync(users);
        }

        public async Task IsExistIdenticalInfo(User user)
        {
            var idExists = await _repository.AnyAsync(u => u.Id == user.Id);
            var userNameExists = await _repository.AnyAsync(u => u.UserName == user.UserName);
            var phoneNoExists = await _repository.AnyAsync(u => u.PhoneNo == user.PhoneNo);
            var emailExists = await _repository.AnyAsync(u => u.Email == user.Email);

            if (idExists)
            {
                throw new ArgumentException("There exist a user with the same Id !");
            }
            else if (userNameExists)
            {
                throw new ArgumentException("There exist a user with the same username !");
            }
            else if (phoneNoExists)
            {
                throw new ArgumentException("There exist a user with the same phone number !");
            }
            else if (emailExists)
            {
                throw new ArgumentException("There exist a user with the same email address !");
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

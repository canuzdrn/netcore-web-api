using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using userMS.Domain.Entities;

namespace userMS.Application.Services
{
    public interface IUserService
    {
        Task<IEnumerable<User>> GetAllUsersAsync();

        Task<User> GetUserByIdAsync(string id);

        Task<User> GetUserByUsernameAsync(string username);

        Task<User> GetUserByEmailAddressAsync(string email);

        Task<User> GetUserByPhoneNumberAsync(string phoneNo);

        Task<User> AddUserAsync(User user);

        Task<IEnumerable<User>> AddUsersAsync(IEnumerable<User> users);

        Task<User> UpdateUserAsync(User user);

        Task<IEnumerable<User>> UpdateUsersAsync(IEnumerable<User> users);

        Task<bool> DeleteUserAsync(User user);

        Task<bool> DeleteUserByIdAsync(string id);

        Task<bool> DeleteUsersAsync(IEnumerable<User> users);

        Task<IEnumerable<User>> FindByAsync(Expression<Func<User, bool>> predicate);

    }
}

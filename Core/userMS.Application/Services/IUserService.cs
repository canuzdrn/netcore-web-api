using System.Linq.Expressions;
using userMS.Application.DTOs;
using userMS.Domain.Entities;

namespace userMS.Application.Services
{
    public interface IUserService
    {
        Task<IEnumerable<UserDto>> GetAllUsersAsync();

        Task<UserDto> GetUserByIdAsync(Guid id);

        Task<UserDto> GetUserByUsernameAsync(string username);

        Task<UserDto> GetUserByEmailAddressAsync(string email);

        Task<UserDto> GetUserByPhoneNumberAsync(string phoneNo);

        Task<UserDto> AddUserAsync(UserDto userDto);

        Task<IEnumerable<UserDto>> AddUsersAsync(IEnumerable<UserDto> userDtos);

        Task<UserDto> UpdateUserAsync(UserDto userDto);

        Task<IEnumerable<UserDto>> UpdateUsersAsync(IEnumerable<UserDto> userDtos);

        Task<bool> DeleteUserAsync(UserDto userDto);

        Task<bool> DeleteUserByIdAsync(Guid id);

        Task<bool> DeleteUsersAsync(IEnumerable<UserDto> userDtos);

        Task<IEnumerable<User>> FindByAsync(Expression<Func<User, bool>> predicate);

    }
}

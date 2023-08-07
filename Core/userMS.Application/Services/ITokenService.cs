using userMS.Application.DTOs;
using userMS.Domain.Entities;

namespace userMS.Application.Services
{
    public interface ITokenService
    {
        public string GenerateToken(LoginUserDto loginDto);
    }
}

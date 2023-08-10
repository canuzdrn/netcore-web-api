using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using userMS.Application.DTOs;
using userMS.Application.Services;
using userMS.Infrastructure.Com;

namespace userMS.Infrastructure.Services
{
    public class TokenService : ITokenService
    {
        private readonly SymmetricSecurityKey _key;
        public TokenService(IOptions<JwtSettings> tokenSettings)
        {
            _key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(tokenSettings.Value.Secret));
        }
        public string GenerateToken(LoginResponseDto loginDto)
        {
            var claims = new List<Claim>
            {
               new Claim(JwtRegisteredClaimNames.NameId,loginDto.UserName)
            };

            var credentials = new SigningCredentials(_key, SecurityAlgorithms.HmacSha256Signature);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.Now.AddDays(1),
                SigningCredentials = credentials
            };

            var tokenHandler = new JwtSecurityTokenHandler();

            var token = tokenHandler.CreateToken(tokenDescriptor);

            return tokenHandler.WriteToken(token);
        }
    }
}

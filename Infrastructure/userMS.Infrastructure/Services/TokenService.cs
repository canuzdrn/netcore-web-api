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
        private readonly AppSettings _options;
        private readonly SymmetricSecurityKey _key;
        public TokenService(IOptions<AppSettings> options)
        {
            _options = options.Value;
            _key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_options.JwtSettings.Secret));
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

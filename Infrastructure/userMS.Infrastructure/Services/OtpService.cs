using Microsoft.AspNetCore.Razor.TagHelpers;
using Microsoft.Extensions.Options;
using OtpNet;
using System.Security.Cryptography;
using userMS.Application.Services;
using userMS.Infrastructure.Com;

namespace userMS.Infrastructure.Services
{
    public class OtpService : IOtpService
    {
        private readonly AppSettings _options;
        public OtpService(IOptions<AppSettings> options)
        {
            _options = options.Value;
        }
        public async Task<string> GenerateTotp()
        {
            byte[] secretKeyBytes = new byte[16]; // 16 bytes = 128 bits
            using (var rng = new RNGCryptoServiceProvider())
            {
                rng.GetBytes(secretKeyBytes);
            }

            var totp = new Totp(secretKeyBytes);

            string totpValue = totp.ComputeTotp();

            return totpValue;
        }
    }
}

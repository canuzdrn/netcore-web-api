using Microsoft.Extensions.Options;
using Twilio;
using Twilio.Rest.Api.V2010.Account;
using userMS.Application.DTOs.Request;
using userMS.Application.Services;
using userMS.Domain.Exceptions;
using userMS.Infrastructure.Com;
using userMS.Infrastructure.Statics;

namespace userMS.Infrastructure.Services
{
    public class SmsService : ISmsService
    {
        private readonly AppSettings _options;
        
        public SmsService(IOptions<AppSettings> options)
        {
            _options = options.Value;
        }

        public async Task<MessageResource> SendSmsAsync(SmsSendRequestDto smsSendRequestDto)
        {
            TwilioClient.Init(_options.TwilioSettings.AccountSid, _options.TwilioSettings.AuthToken);

            var result = await MessageResource.CreateAsync(
                body: smsSendRequestDto.Body,
                from: new Twilio.Types.PhoneNumber(_options.TwilioSettings.PhoneNumber),
                to: new Twilio.Types.PhoneNumber(smsSendRequestDto.To));

            if (result is null) throw new BadRequestException(ErrorMessages.SmsCouldntBeSent);

            return result;
        }
    }
}

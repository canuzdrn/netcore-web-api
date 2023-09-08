using Twilio.Rest.Api.V2010.Account;
using userMS.Application.DTOs.Request;

namespace userMS.Application.Services
{
    public interface ISmsService
    {
        Task<MessageResource> SendSmsAsync(SmsSendRequestDto smsSendRequestDto);
    }
}

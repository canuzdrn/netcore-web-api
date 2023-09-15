namespace userMS.Application.DTOs.Request
{
    public class ExternalProviderOauthLoginRequestDto
    {
        public string AccessToken { get; set; }

        public string ProviderId { get; set; }
    }
}

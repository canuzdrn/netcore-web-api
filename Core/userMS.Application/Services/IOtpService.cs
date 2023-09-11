namespace userMS.Application.Services
{
    public interface IOtpService
    {
        Task<string> GenerateTotp();
    }
}

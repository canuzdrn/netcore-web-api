namespace userMS.Application.Services
{
    public interface IEmailService
    {
        Task SendRegisterEmailAsync(string to);

        Task SendLoginEmailAsync(string to);
    }
}

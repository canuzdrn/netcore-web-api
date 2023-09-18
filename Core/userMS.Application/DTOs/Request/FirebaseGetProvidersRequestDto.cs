namespace userMS.Application.DTOs.Request
{
    public class FirebaseGetProvidersRequestDto
    {
        public string Identifier { get; set; }  // user's email

        public string ContinueUrl { get; set; } = "https://localhost:7010";

    }
}

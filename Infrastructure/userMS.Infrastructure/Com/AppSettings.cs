namespace userMS.Infrastructure.Com
{
    public class AppSettings
    {
        public string RedisHost { get; set; }

        public EmailSettings EmailSettings { get; set; }

        public EmailContent EmailContent { get; set; }

        public JwtSettings JwtSettings { get; set; }
    }
}

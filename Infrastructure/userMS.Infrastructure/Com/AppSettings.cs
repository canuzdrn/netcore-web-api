﻿using System.Globalization;

namespace userMS.Infrastructure.Com
{
    public class AppSettings
    {
        public string RedisHost { get; set; }

        public EmailSettings EmailSettings { get; set; }

        public EmailContent EmailContent { get; set; }

        public JwtSettings JwtSettings { get; set; }

        public TwilioSettings TwilioSettings { get; set; }

        public string FirebaseApiKey { get; set; }

        public string IdentityToolkitBaseUrl { get; set; }
    }
}

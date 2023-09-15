namespace userMS.Infrastructure.Statics
{
    public class RoutingUrls
    {
        public const string BaseRoute = "api/[controller]";

        public static class Auth
        {
            public const string Register = "register";

            public const string LoginIdentifier = "login/identifier";

            public const string LoginPhone = "login/phone";

            public const string VerifyEmailOtp = "otp/verify/email";

            public const string VerifyPhoneOtp = "otp/verify/phone";

            public const string SendEmailOtp = "otp/send/email";

            public const string SendPhoneOtp = "otp/send/phone";

            public const string GoogleOauth = "oauth/signin-google";

            public const string GoogleOauthCallback = "oauth/signin-google-callback";

            public const string GithubOauth = "oauth/signin-github";

            public const string GithubOauthCallback = "oauth/signin-github-callback";

            public const string MicrosoftOauth = "oauth/signin-microsoft";

            public const string MicrosoftOauthCallback = "oauth/signin-microsoft-callback";

        }

        public static class User
        {
            public const string GetAll = "users";

            public const string GetById = "user/id/{id}";
            public const string GetByUsername = "user/username/{username}";
            public const string GetByEmail = "user/email/{email}";
            public const string GetByPhoneNo = "user/phoneNo/{phoneNo}";

            public const string Create = "user";
            public const string BulkCreate = "users/bulk";

            public const string Update = "user";
            public const string BulkUpdate = "users/bulk";

            public const string Delete = "user";
            public const string BulkDelete = "users/bulk";
            public const string DeleteById = "user/id/{id}";
        }
    }
}

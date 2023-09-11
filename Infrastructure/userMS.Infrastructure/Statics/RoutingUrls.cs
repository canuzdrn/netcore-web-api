﻿namespace userMS.Infrastructure.Statics
{
    public class RoutingUrls
    {
        public const string BaseRoute = "api/[controller]";

        public static class Auth
        {
            public const string Register = "register";

            public const string LoginIdentifier = "login/identifier";

            public const string LoginPhone = "login/phone";

            public const string VerifyOtp = "otp/verify";

            public const string SendEmailOtp = "otp/send/email";

            public const string SendPhoneOtp = "otp/send/phone";
        }

        public static class FirebaseAuth
        {
            public const string Register = "firebase/register";

            public const string Login = "firebase/login";
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

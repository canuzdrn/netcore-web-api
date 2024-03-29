﻿namespace userMS.Infrastructure.Statics
{
    public class ErrorMessages
    {
        public static class User
        {
            public const string UserNotFound = "Provided user with the given information does not exist !";
            public const string UserIdNotFound = "User with the provided Id is does not exist !";
            public const string UsernameNotFound = "User with the provided username does not exist !";
            public const string UserEmailNotFound = "User with the provided email address does not exist !";
            public const string UserPhoneNumberNotFound = "User with the provided phone number does not exist !";
            public const string UserNotFoundInBulk = "At least one user from the provided user list does not exist !";
            public const string IdenticalInfoInBulk = "Multiple users cannot share a unique property !";
            public const string DuplicateUserInBulkUpdate = "Cannot update the same user multiple times in a single operation !";
        }
        
        public static class Auth
        {
            public const string IncorrectIdentifierProvided = "Invalid Credentials, user does not exist !";
            public const string IncorrectCredentialsProvided = "Invalid Credentials ";
            public const string IncorrectPasswordProvided = "Invalid Credentials, incorrect password !";
            public const string EmailIsNotVerified = "User's email is not verified, in order to login you need to verify your email !";
            public const string PhoneNumberIsNotVerified = "User's phone number is not verified, in order to login you need to verify your phone number !";
            public const string UserIsAlreadySignedInWithSameProvider = "User has already signed in with the same provider !";
            public const string EmailIsAlreadyVerified = "User's email address is already verified !";
            public const string PhoneIsAlreadyVerified = "User's phone number is already verified !";
        }

        public static class Firebase
        {
            public const string FirebaseLoginError = "Error occured during login to Firebase !";
            public const string FirebaseRegisterError = "Error occured during registering to Firebase !";
            public const string FirebaseCouldNotVerifyPhoneNumber = "Could not verify phone number while authenticating to Firebase !";
            public const string FirebaseCouldNotSignInWithPhoneNumber = "Error occured during signing in via phone number !";
            public const string FirebaseCouldNotRetrieveProviders = "Error occured during retrieving the providers of the user !";
        }
        
        public static class External
        {
            public const string EmailOtpCannotBeSaved = "Error occured during saving the OTP that's been sent to email !";
            public const string PhoneOtpCannotBeSaved = "Error occured during saving the OTP that's been sent to phone number !";
            public const string OtpIsExpired = "Requested OTP is expired !";
            public const string OtpIsIncorrect = "Verification is failed, incorrect OTP is provided !";
            public const string SmsCouldntBeSent = "Error occured during SMS sending process !";
            public const string EmailCouldntBeSent = "Error occured during Email sending process !";
            public const string EmailIsNotvalid = "Provided email is invalid or null !";
            public const string ExternalProviderAuthenticationFailed = "Authentication with external provider is failed !";
            public const string AccessTokenCannotBeRetrieved = "Error occured during the retrieval of the access token !";
        }
    }
}

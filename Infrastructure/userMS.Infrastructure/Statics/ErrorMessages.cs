namespace userMS.Infrastructure.Statics
{
    public class ErrorMessages
    {
        public const string UserNotFound = "Provided user with the given information does not exist !";
        public const string UserIdNotFound = "User with the provided Id is does not exist !";
        public const string UsernameNotFound = "User with the provided username does not exist !";
        public const string UserEmailNotFound = "User with the provided email address does not exist !";
        public const string UserPhoneNumberNotFound = "User with the provided phone number does not exist !";

        public const string UserNotFoundInBulk = "At least one user from the provided user list does not exist !";
        public const string IdenticalInfoInBulk = "Multiple users cannot share a unique property !";

        public const string DuplicateUserInBulkUpdate = "Cannot update the same user multiple times in a single operation !";

        public const string IncorrectIdentifierProvided = "Invalid Credentials, user does not exist !";
        public const string IncorrectCredentialsProvided = "Invalid Credentials ";
        public const string IncorrectPasswordProvided = "Invalid Credentials, incorrect password !";

        public const string FirebaseLoginError = "Error occured during login to Firebase !";
        public const string FirebaseRegisterError = "Error occured during registering to Firebase !";
        public const string FirebaseCouldNotVerifyPhoneNumber = "Could not verify phone number while authenticating to Firebase !";
        public const string FirebaseCouldNotSignInWithPhoneNumber = "Error occured during signing in via phone number !";
    }
}

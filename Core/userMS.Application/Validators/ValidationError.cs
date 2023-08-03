namespace userMS.Application.Validators
{
    // custom error class in order to keep error message in a structure
    public class ValidationError
    {
        public string Message { get; set; }
        public ValidationError(string message) {
            Message = message;
        }
    }
}

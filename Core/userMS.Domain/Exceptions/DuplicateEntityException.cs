namespace userMS.Domain.Exceptions
{
    public class DuplicateEntityException : Exception
    {
        public DuplicateEntityException(string message) :
            base(message)
        {
            
        }

        public DuplicateEntityException(string entityName, string field, string value) :
            base($"Duplicate {entityName} found for {field}: {value}")
        {
            
        }
    }
}

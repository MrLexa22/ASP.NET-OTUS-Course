namespace PromoCodeFactory.WebHost.ExceptionHandling.Exceptions
{
    public class ValidationException : AppException
    {
        public ValidationException(string message)
            : base("VALIDATION_ERROR", message, 400)
        {
        }
    }
}

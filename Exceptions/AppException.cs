namespace ProductDemo.Exceptions
{
    public class AppException : Exception
    {
        public int StatusCode;
        public List<string>? Errors { get; }

        public AppException(string message, int statusCode = 400) : base(message)
        {
            StatusCode = statusCode;
        }

        public AppException(List<string> errors, string message = "Validation failed", int statusCode = 400) : base(message)
        {
            StatusCode = statusCode;
            Errors = errors;
        }
    }
}
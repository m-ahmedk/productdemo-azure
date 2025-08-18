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

    }
}
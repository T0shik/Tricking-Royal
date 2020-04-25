namespace Battles.Application.ViewModels
{
    public class Response
    {
        protected Response(string message, bool success = default)
        {
            Message = message;
            Success = success;
        }

        public string Message { get; } 
        public bool Success { get; }

        public static Response Ok(string message) => new Response(message, true);
        public static Response<T> Ok<T>(string message, T value) => new Response<T>(message, value);
        public static Response<T> Ok<T>(T value) => new Response<T>("", value);
        public static Response Fail(string message) => new Response(message);
        public static Response<T> Fail<T>(string message) => new Response<T>(message);
    }
}
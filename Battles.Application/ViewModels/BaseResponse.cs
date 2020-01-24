namespace Battles.Application.ViewModels
{
    public class BaseResponse
    {
        public BaseResponse(string message, bool success = default)
        {
            Message = message;
            Success = success;
        }

        public string Message { get; } 
        public bool Success { get; }

        public static BaseResponse Ok(string message) => new BaseResponse(message, true);
        public static BaseResponse<T> Ok<T>(string message, T value) => new BaseResponse<T>(message, value);
        public static BaseResponse Fail(string message) => new BaseResponse(message);
        public static BaseResponse<T> Fail<T>(string message) => new BaseResponse<T>(message);

    }
}
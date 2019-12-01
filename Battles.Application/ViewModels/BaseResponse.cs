namespace Battles.Application.ViewModels
{
    public class BaseResponse
    {
        public BaseResponse(string message, bool success)
        {
            Message = message;
            Success = success;
        }

        public string Message { get;  }
        public bool Success { get; }

        public static BaseResponse Create(string message) => new BaseResponse(message, true);
        public static BaseResponse Fail(string message) => new BaseResponse(message, false);
    }
}
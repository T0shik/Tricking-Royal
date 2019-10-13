namespace Battles.Application.ViewModels
{
    public class BaseResponse
    {
        public BaseResponse(string message, bool success)
        {
            Message = message;
            Success = success;
        }

        public string Message { get; set; }
        public bool Success { get; set; }
    }
}
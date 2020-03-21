namespace Battles.Application.ViewModels
{
    public class BaseResponse<T> : BaseResponse
    {
        public BaseResponse(string message)
            : base(message) { }

        public BaseResponse(string message, T value)
            : base(message, true)
        {
            Value = value;
        }

        public T Value { get; }
    }
}
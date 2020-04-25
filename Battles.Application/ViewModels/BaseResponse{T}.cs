namespace Battles.Application.ViewModels
{
    public class Response<T> : Response
    {
        public Response(string message)
            : base(message) { }

        public Response(string message, T value)
            : base(message, true)
        {
            Value = value;
        }

        public T Value { get; }
    }
}
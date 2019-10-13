namespace Battles.Cdn.ViewModels
{
    public class Response
    {
        public Response(string error)
        {
            this.Error = error;
            this.Success = false;
        }

        public Response(string video, string thumb)
        {
            this.Video = video;
            this.Thumb = thumb;
            this.Success = true;
        }

        public bool Success { get; private set; }
        public string Video { get; private set; }
        public string Thumb { get; private set; }
        public string Error { get; private set; }
    }
}

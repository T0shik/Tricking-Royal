namespace Battles.Application.SubServices.VideoConversion
{
    public class VideoConversionResult
    {
        public VideoConversionResult(string video, string thumb)
        {
            Video = video;
            Thumb = thumb;
        }

        public string Video { get; }
        public string Thumb { get; }
    }
}
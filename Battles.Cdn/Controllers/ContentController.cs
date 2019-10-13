using Battles.Cdn.FileServices;
using Microsoft.AspNetCore.Mvc;

namespace Battles.Cdn.Controllers
{
    public class ContentController : Controller
    {
        private readonly FileStreams _fileStreams;

        public ContentController(FileStreams fileStreams)
        {
            _fileStreams = fileStreams;
        }

        [HttpGet("static/{fileName}")]
        [ResponseCache(CacheProfileName = "Monthly6")]
        public FileStreamResult GetStaticImage(string fileName) =>
            new FileStreamResult(_fileStreams.StaticImageStream(fileName), "image/png");

        [HttpGet("image/img/{id}/{fileName}")]
        [ResponseCache(CacheProfileName = "Monthly6")]
        public FileStreamResult GetUserImage(string id, string fileName) =>
            new FileStreamResult(_fileStreams.UserImageStream(id, fileName), "image/jpg");

        [HttpGet("image/thumb/{id}/{fileName}")]
        [ResponseCache(CacheProfileName = "Monthly6")]
        public FileStreamResult GetVideoThumbnail(string id, string fileName) =>
            new FileStreamResult(_fileStreams.ThumbImageStream(id, fileName), "image/jpg");

        [HttpGet("video/{id}/{fileName}")]
        [ResponseCache(CacheProfileName = "Monthly6")]
        public FileStreamResult GetVideo(string id, string fileName) =>
            new FileStreamResult(_fileStreams.VideoStream(id, fileName), "video/mp4")
            {
                EnableRangeProcessing = true
            };
    }
}
using System.IO;
using Battles.Cdn.Settings;
using Microsoft.AspNetCore.Hosting;

namespace Battles.Cdn.FileServices
{
    public class FileStreams
    {
        private readonly string _matchVideos;
        private readonly string _userImages;
        private readonly string _staticImages;

        public FileStreams(
            IHostingEnvironment env,
            FilePaths filePaths)
        {
            _matchVideos = env.IsProduction() ? filePaths.Videos : Path.Combine(env.WebRootPath, filePaths.Videos);
            _userImages = env.IsProduction()
                              ? filePaths.UserImages
                              : Path.Combine(env.WebRootPath, filePaths.UserImages);
            _staticImages = Path.Combine(env.WebRootPath, filePaths.StaticImages);
        }

        public FileStream StaticImageStream(string fileName) =>
            FileStream(Path.Combine(_staticImages, fileName));

        public FileStream UserImageStream(string id, string fileName) =>
            FileStream(Path.Combine(_userImages, id, fileName));

        public FileStream VideoStream(string id, string fileName) =>
            FileStream(Path.Combine(_matchVideos, id, fileName));

        public FileStream ThumbImageStream(string id, string fileName) =>
            FileStream(Path.Combine(_matchVideos, id, fileName));

        private static FileStream FileStream(string path) =>
            new FileStream(path, FileMode.Open, FileAccess.Read);
    }
}
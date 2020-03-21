using System.IO;
using System.Threading.Tasks;
using Battles.Shared;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;

namespace Battles.Cdn.FileServices
{
    public class VideoManager : BaseFileManager
    {
        private readonly string _matchVideos;

        public VideoManager(
            FilePaths filePaths,
            IHostingEnvironment env)
        {
            _matchVideos = env.IsProduction() ? filePaths.Videos : Path.Combine(env.WebRootPath, filePaths.Videos);
        }

        public async Task<string> SaveAsync(string id, IFormFile video)
        {
            var savePath = Path.Combine(_matchVideos, id);

            if (!Directory.Exists(savePath))
            {
                Directory.CreateDirectory(savePath);
            }

            var outputName = $"init_{CreateFileName()}{GetFileMime(video.FileName)}";
            var outputFile = Path.Combine(savePath, outputName);

            using (var fileStream = new FileStream(outputFile, FileMode.Create))
            {
                await video.CopyToAsync(fileStream);
            }

            return outputName;
        }
    }
}
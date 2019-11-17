using System;
using System.IO;
using System.Threading.Tasks;
using Battles.Shared;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using PhotoSauce.MagicScaler;

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

        private static void OptimizeThumb(string tempThumbPath, string thumbPath)
        {
            using (var output = new FileStream(thumbPath, FileMode.Create))
            {
                MagicImageProcessor.ProcessImage(tempThumbPath, output, ThumbImageSettings);
            }

            File.Delete(tempThumbPath);
        }

        private static ProcessImageSettings ThumbImageSettings =>
            new ProcessImageSettings()
            {
                Height = 320,
                Width = 480,
                ResizeMode = CropScaleMode.Stretch,
                SaveFormat = FileFormat.Jpeg,
                JpegQuality = 90
            };
    }
}
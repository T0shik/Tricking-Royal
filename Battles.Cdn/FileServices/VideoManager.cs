using System;
using System.IO;
using System.Threading.Tasks;
using Battles.Cdn.ViewModels;
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
        private readonly ILogger<VideoManager> _logger;

        public VideoManager(
            FilePaths filePaths,
            IHostingEnvironment env,
            ILogger<VideoManager> logger)
        {
            _matchVideos = env.IsProduction() ? filePaths.Videos : Path.Combine(env.WebRootPath, filePaths.Videos);

            _logger = logger;
        }

        public async Task<(bool Complete, Response Response)> SaveInitVideoAsync(string id, IFormFile video)
        {
            try
            {
                var savePath = Path.Combine(_matchVideos, id);

                if (!Directory.Exists(savePath))
                {
                    Directory.CreateDirectory(savePath);
                }

                var fileName = CreateFileName();
                var outputName = $"init_{fileName}{GetFileMime(video.FileName)}";
                var outputFile = Path.Combine(savePath, outputName);

                using (var fileStream = new FileStream(outputFile, FileMode.Create))
                {
                    await video.CopyToAsync(fileStream);
                }

                return (true, new Response(outputName, ""));
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message);

                return (false, new Response(e.Message));
            }
        }

        

        public bool RemoveFile(string id, string fileName)
        {
            return string.IsNullOrEmpty(fileName) && TryRemoveFile(Path.Combine(_matchVideos, id, fileName));
        }

        private static void OptimizeThumb(string tempThumbPath, string thumbPath)
        {
            using (var output = new FileStream(thumbPath, FileMode.Create))
            {
                MagicImageProcessor.ProcessImage(tempThumbPath, output, ThumbImageSettings);
            }

            File.Delete(tempThumbPath);
        }

        private static ProcessImageSettings ThumbImageSettings => new ProcessImageSettings()
        {
            Height = 320,
            Width = 480,
            ResizeMode = CropScaleMode.Stretch,
            SaveFormat = FileFormat.Jpeg,
            JpegQuality = 90
        };
    }
}
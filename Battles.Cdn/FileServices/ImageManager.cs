using System;
using System.IO;
using System.Linq;
using Battles.Cdn.Settings;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using PhotoSauce.MagicScaler;
using static System.String;

namespace Battles.Cdn.FileServices
{
    public class ImageManager : BaseFileManager
    {
        private readonly IHostingEnvironment _env;
        private readonly ILogger<ImageManager> _logger;
        private readonly string _userImages;
        private string _staticImages;

        public ImageManager(
            IHostingEnvironment env,
            FilePaths filePaths,
            ILogger<ImageManager> logger)
        {
            _env = env;
            _logger = logger;
            _userImages = Path.Combine(_env.WebRootPath, filePaths.UserImages);
            _staticImages = Path.Combine(_env.WebRootPath, filePaths.StaticImages);
        }

        public (bool Saved, string FileName) SaveImage(string id, IFormFile picture)
        {
            try
            {
                var settings = new ProcessImageSettings()
                {
                    Height = 75,
                    Width = 75,
                    ResizeMode = CropScaleMode.Crop,
                    SaveFormat = FileFormat.Jpeg,
                    JpegQuality = 100
                };

                var savePath = Path.Combine(_env.ContentRootPath, _userImages, id);
                if (!Directory.Exists(savePath)) Directory.CreateDirectory(savePath);

                var fileName = $"img_{CreateFileName()}.jpg";
                var finalPath = Path.Combine(savePath, fileName);

                var currentImage = Directory.GetFiles(savePath).FirstOrDefault();

                using (var output = new FileStream(finalPath, FileMode.Create))
                {
                    MagicImageProcessor.ProcessImage(picture.OpenReadStream(), output, settings);
                }

                if (!IsNullOrEmpty(currentImage))
                {
                    File.Delete(currentImage);
                }

                return (true, fileName);
            }
            catch (Exception e)
            {
                _logger.LogError(e, e.Message);

                return (false, $"Error: {e.Message}");
            }
        }
    }
}
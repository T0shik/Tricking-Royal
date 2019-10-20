using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Battles.Cdn.Settings;
using Battles.Cdn.ViewModels;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using PhotoSauce.MagicScaler;
using Xabe.FFmpeg;
using Xabe.FFmpeg.Enums;

namespace Battles.Cdn.FileServices
{
    public class VideoManager : BaseFileManager
    {
        private readonly string _matchVideos;
        private readonly string _ffmpegPath;
        private readonly ILogger<VideoManager> _logger;

        public VideoManager(
            FilePaths filePaths,
            IHostingEnvironment env,
            ILogger<VideoManager> logger)
        {
            _matchVideos = env.IsProduction() ? filePaths.Videos : Path.Combine(env.WebRootPath, filePaths.Videos);
            _ffmpegPath = env.IsProduction()
                              ? filePaths.VideoEditingExecutables
                              : Path.Combine(env.ContentRootPath, filePaths.VideoEditingExecutables);
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

        public async Task<(bool Complete, Response Response)> TrimVideoAsync(string id,
                                                                             string video,
                                                                             double start,
                                                                             double end)
        {
            try
            {
                var savePath = Path.Combine(_matchVideos, id);
                var inputFile = Path.Combine(savePath, video);

                var fileName = CreateFileName();
                var outputName = $"c_{fileName}.mp4";
                var thumbName = $"thumb_{fileName}.jpg";

                var convertSuccess = await ConvertWithXabe(inputFile, savePath, outputName, thumbName, start, end);

                TryRemoveFile(inputFile);

                return !convertSuccess
                           ? (false, new Response("Conversion Failed"))
                           : (true, new Response(outputName, thumbName));
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message);
                return (false, new Response(e.Message));
            }
        }

        private async Task<bool> ConvertWithXabe(
            string inputFile,
            string savePath,
            string outputName,
            string thumbName,
            double start,
            double end)
        {
            var outputPath = Path.Combine(savePath, outputName);
            var thumbPath = Path.Combine(savePath, thumbName);
            var tempThumbName = $"temp_{thumbName}";
            var tempThumbPath = Path.Combine(savePath, tempThumbName);

            FFmpeg.ExecutablesPath = _ffmpegPath;

            var mediaInfo = await MediaInfo.Get(inputFile);
            var videoStream = mediaInfo.VideoStreams.First();
            var audioStream = mediaInfo.AudioStreams.First();

            var startSpan = TimeSpan.FromSeconds(start);
            var endSpan = TimeSpan.FromSeconds(end);
            var duration = endSpan.TotalSeconds == 0 ? mediaInfo.Duration : endSpan - startSpan;

            try
            {
                videoStream
                    .SetSize(VideoSize.Hvga)
                    .SetCodec(VideoCodec.H264)
                    .Split(startSpan, duration);

                var conversion = Conversion.New()
                                           .AddStream(videoStream)
                                           .AddStream(audioStream)
                                           .SetOutput(outputPath)
                                           .UseMultiThread(false)
                                           .SetPreset(ConversionPreset.Fast);

                await conversion.Start();
                await Conversion.Snapshot(outputPath, tempThumbPath, new TimeSpan(0)).Start();

                OptimizeThumb(tempThumbPath, thumbPath);

                return true;
            }
            catch (Exception e)
            {
                _logger.LogError(
                                 $"{e.Message} - input: {inputFile} - start: {startSpan} - end: {endSpan} - duration: {duration}");
                return false;
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
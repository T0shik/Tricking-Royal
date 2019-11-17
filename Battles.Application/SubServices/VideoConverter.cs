using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Battles.Shared;
using Microsoft.AspNetCore.Hosting;
using Xabe.FFmpeg;
using Xabe.FFmpeg.Enums;

namespace Battles.Application.SubServices
{
    public class VideoConverter
    {
        private string _matchVideos;

        public VideoConverter(
            IHostingEnvironment env,
            FilePaths filePaths
        )
        {
            _matchVideos = env.IsProduction() ? filePaths.Videos : Path.Combine(env.WebRootPath, filePaths.Videos);
            var ffmpegPath = env.IsProduction()
                                 ? filePaths.VideoEditingExecutables
                                 : Path.Combine(env.ContentRootPath, filePaths.VideoEditingExecutables);

            FFmpeg.ExecutablesPath = ffmpegPath;
        }

        public async Task<(bool Complete, Response Response)> TrimVideoAsync(
            string id,
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
    }
}
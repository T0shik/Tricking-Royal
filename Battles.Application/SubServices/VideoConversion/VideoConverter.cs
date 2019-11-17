using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Battles.Shared;
using Microsoft.AspNetCore.Hosting;
using Xabe.FFmpeg;
using Xabe.FFmpeg.Enums;

namespace Battles.Application.SubServices.VideoConversion
{
    public class VideoConverter : BaseFileManager
    {
        private readonly string _matchVideos;

        public VideoConverter(
            IHostingEnvironment env,
            FilePaths filePaths)
        {
            _matchVideos = env.IsProduction() ? filePaths.Videos : Path.Combine(env.WebRootPath, filePaths.Videos);
            var ffmpegPath = env.IsProduction()
                                 ? filePaths.VideoEditingExecutables
                                 : Path.Combine(env.ContentRootPath, filePaths.VideoEditingExecutables);

            FFmpeg.ExecutablesPath = ffmpegPath;
        }

        public async Task<VideoConversionResult> TrimVideoAsync(
            string id,
            string video,
            double start,
            double end)
        {
            var savePath = Path.Combine(_matchVideos, id);
            var inputFile = Path.Combine(savePath, video);

            var fileName = CreateFileName();
            var outputName = $"c_{fileName}.mp4";
            var thumbName = $"thumb_{fileName}.jpg";
            var outputFile = Path.Combine(savePath, outputName);

            await ConvertWithXabe(savePath, inputFile, outputFile, thumbName, start, end);

            TryRemoveFile(inputFile);
            
            return new VideoConversionResult(outputName, thumbName);
        }

        private async Task ConvertWithXabe(
            string savePath,
            string inputFile,
            string outputPath,
            string thumbName,
            double start,
            double end)
        {
            var thumbPath = Path.Combine(savePath, thumbName);
            var tempThumbName = $"temp_{thumbName}";
            var tempThumbPath = Path.Combine(savePath, tempThumbName);

            var mediaInfo = await MediaInfo.Get(inputFile);
            var videoStream = mediaInfo.VideoStreams.First();
            var audioStream = mediaInfo.AudioStreams.First();

            var startSpan = TimeSpan.FromSeconds(start);
            var endSpan = TimeSpan.FromSeconds(end);
            var duration = endSpan.TotalSeconds == 0 ? mediaInfo.Duration : endSpan - startSpan;

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
            TryRemoveFile(tempThumbPath);
        }

        private void OptimizeThumb(string tempThumbPath, string thumbPath)
        {
            throw new NotImplementedException();
        }
    }
}
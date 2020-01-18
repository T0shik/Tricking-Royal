using System;
using System.Linq.Expressions;
using Battles.Models;

// ReSharper disable MemberCanBePrivate.Global

namespace Battles.Application.ViewModels.Matches
{
    public class VideoViewModel
    {
        public int VideoIndex { get; set; }
        public int UserIndex { get; set; }
        public string Video { get; set; }
        public string Thumb { get; set; }
        public bool Empty { get; set; }

        public static readonly Expression<Func<Video, VideoViewModel>> Projection =
            video => ProjectionFunction(video);

        public static Func<Video, VideoViewModel> ProjectionFunction =>
            video => new VideoViewModel
            {
                VideoIndex = video.VideoIndex,
                UserIndex = video.UserIndex,
                Video = video.VideoPath,
                Thumb = video.ThumbPath,
                Empty = video.Empty
            };
    }
}
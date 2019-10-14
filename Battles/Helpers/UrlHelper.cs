using System.IO;

namespace Battles.Helpers
{
    public static class CdnUrlHelper
    {
        //todo look at these paths: video -> videos
        public static string CreateVideoUrl(string cdnLink, params string[] path) =>
            $"{cdnLink}/video/{Path.Combine(path)}";

        public static string CreateThumbUrl(string cdnLink, params string[] path) =>
            $"{cdnLink}/image/thumb/{Path.Combine(path)}";

        public static string CreateImageUrl(string cdnLink, params string[] path) =>
            $"{cdnLink}/image/img/{Path.Combine(path)}";
    }
}

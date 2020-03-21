using Newtonsoft.Json;

namespace Battles.Application.Extensions
{
    public static class GenericExtensions
    {
        public static string ToJson(this object @this) => JsonConvert.SerializeObject(@this);
    }
}
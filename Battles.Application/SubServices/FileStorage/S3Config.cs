namespace Battles.Application.SubServices.FileStorage
{
    public class S3Config
    {
        public string AccessKey { get; set; }
        public string SecretKey { get; set; }
        public string ServiceUrl { get; set; }
        public string Bucket { get; set; }
        public TYPE Type { get; set; }

    }
}
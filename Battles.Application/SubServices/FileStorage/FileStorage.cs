using System;
using System.IO;
using System.Threading.Tasks;
using Amazon.S3;
using Amazon.S3.Transfer;
using Microsoft.Extensions.Options;

namespace Battles.Application.SubServices.FileStorage
{
    class FileStorage : IFileStorage
    {
        private readonly string _accessKey;
        private readonly string _secretKey;
        private readonly AmazonS3Config _config;
        private const string c_root = "https://aw-test-bucket.eu-central-1.linodeobjects.com/";

        public FileStorage(S3Config config)
        {
            _config = new AmazonS3Config {ServiceURL = config.ServiceUrl};
            _accessKey = config.AccessKey;
            _secretKey = config.SecretKey;
        }

        public async Task<string> Save(byte[] file, params string[] filePath)
        {
            var key = string.Join('/', filePath);
            await S3Client(client =>
            {
                using (var ms = new MemoryStream(file))
                {
                    var uploadRequest = new TransferUtilityUploadRequest
                    {
                        InputStream = ms,
                        Key = key,
                        BucketName = "aw-test-bucket",
                        CannedACL = S3CannedACL.PublicRead,
                    };

                    var fileTransferUtility = new TransferUtility(client);
                    return fileTransferUtility.UploadAsync(uploadRequest);
                }
            });

            return string.Concat(c_root, key);
        }

        public Task<byte[]> Pop(string fileName)
        {
            throw new System.NotImplementedException();
        }

        public Task Sweep()
        {
            throw new System.NotImplementedException();
        }

        private Task S3Client(Func<AmazonS3Client, Task> action)
        {
            using (var client = new AmazonS3Client(_accessKey, _secretKey, _config))
                return action(client);
        }
    }
}
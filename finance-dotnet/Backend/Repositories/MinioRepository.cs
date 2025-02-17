using finance_dotnet.Backend.Extensions;
using Minio;
using Minio.DataModel.Args;
using System.Diagnostics;

namespace finance_dotnet.Backend.Repositories
{
    public class MinioRepository : IMinioRepository
    {
        readonly IMinioClient _client;
        readonly string? _storageUrl;
        readonly ILogger<MinioRepository> _logger;
        public MinioRepository(IMinioClient client, IConfiguration configuration, ILogger<MinioRepository> logger)
        {
            _client = client;
            _storageUrl = configuration.GetConnectionString("Minio");
            _logger = logger;
        }


        public async Task<string> PutObjectAsync(string bucketName, IFormFile file)
        {
            try
            {
                var bucketExistArgs = new BucketExistsArgs().WithBucket(bucketName);

                bool bucketExists = await _client.BucketExistsAsync(bucketExistArgs);
                if (bucketExists == false)
                {
                    var bucketMakeArgs = new MakeBucketArgs().WithBucket(bucketName);
                    await _client.MakeBucketAsync(bucketMakeArgs);
                }

                string fileName = $"{file.FileName}";
                string objectName = $"{fileName}";

                using var stream = file.OpenReadStream();
                await _client.PutObjectAsync(new PutObjectArgs()
                    .WithBucket(bucketName)
                    .WithObject(objectName)
                    .WithStreamData(stream)
                    .WithObjectSize(file.Length)
                    .WithContentType(file.ContentType));

                return $"{_storageUrl}/{bucketName}/{objectName}";
            }
            catch(Exception ex)
            {
                // log
                var traceId = Activity.Current?.TraceId.ToString();
                _logger.LogLokiError(ex, traceId);
                return string.Empty;
            }
        }

        public async Task<Stream?> GetObjectAsync(string bucketName, string fileName)
        {
            try
            {
                var memoryStream = new MemoryStream();

                await _client.GetObjectAsync(new GetObjectArgs()
                    .WithBucket(bucketName)
                    .WithObject(fileName)
                    .WithCallbackStream(stream =>
                    {
                        stream.CopyTo(memoryStream);
                        memoryStream.Seek(0, SeekOrigin.Begin);
                    }));

                return memoryStream;
            }
            catch(Exception ex)
            {
                // log
                var traceId = Activity.Current?.TraceId.ToString();
                _logger.LogLokiError(ex, traceId);
                return default;
            }
        }

        public async Task<bool> DeleteObjectAsync(string bucketName, string fileName)
        {
            try
            {
                await _client.RemoveObjectAsync(new RemoveObjectArgs()
                    .WithBucket(bucketName)
                    .WithObject(fileName));

                return true;
            }
            catch(Exception ex)
            {

                // log
                var traceId = Activity.Current?.TraceId.ToString();
                _logger.LogLokiError(ex, traceId);
                return false;
            }
        }

    }
}

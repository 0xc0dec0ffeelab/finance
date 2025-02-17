namespace finance_dotnet.Backend.Repositories
{
    public interface IMinioRepository
    {
        Task<string> PutObjectAsync(string bucketName, IFormFile file);
        Task<Stream?> GetObjectAsync(string bucketName, string fileName);
        Task<bool> DeleteObjectAsync(string bucketName, string fileName);
    }
}

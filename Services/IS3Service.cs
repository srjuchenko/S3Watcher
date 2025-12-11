using Amazon.S3.Model;

namespace S3Watcher.Services
{
  public interface IS3Service
  {
    Task<List<S3Object>> GetNewFilesAsync(string bucketName, string[] extensions);
    Task DownloadFileAsync(string bucketName, string key, string targetPath);
    Task DeleteFileAsync(string bucketName, string key);
  }
}
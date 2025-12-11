using Amazon.S3;
using Amazon.S3.Model;
using Amazon.S3.Transfer;

namespace S3Watcher.Services
{
  public class S3Service : IS3Service
  {
    private readonly IAmazonS3 _s3Client;
    private readonly ILogger<S3Service> _logger;

    public S3Service(IAmazonS3 s3Client, ILogger<S3Service> logger)
    {
      _s3Client = s3Client;
      _logger = logger;
    }

    public async Task<List<S3Object>> GetNewFilesAsync(string bucketName, string[] extensions)
    {
      try
      {
        var request = new ListObjectsV2Request { BucketName = bucketName };
        var response = await _s3Client.ListObjectsV2Async(request);

        if (response.S3Objects == null)
        {
          _logger.LogWarning("No objects found in the bucket.");
          return new List<S3Object>();
        }

        return response.S3Objects
            .Where(obj => extensions.Any(ext => obj.Key.EndsWith(ext, StringComparison.OrdinalIgnoreCase)))
            .ToList();
      }
      catch (Exception ex)
      {
        _logger.LogError(ex, "Error listing files from S3");
        return new List<S3Object>();
      }
    }

    public async Task DownloadFileAsync(string bucketName, string key, string targetPath)
    {
      try
      {
        Directory.CreateDirectory(Path.GetDirectoryName(targetPath)!);

        var transferUtility = new TransferUtility(_s3Client);
        await transferUtility.DownloadAsync(targetPath, bucketName, key);
        _logger.LogInformation($"Downloaded {key} to {targetPath}");
      }
      catch (Exception ex)
      {
        _logger.LogError(ex, $"Error downloading file {key}");
        throw;
      }
    }

    public async Task DeleteFileAsync(string bucketName, string key)
    {
      try
      {
        await _s3Client.DeleteObjectAsync(bucketName, key);
        _logger.LogInformation($"Deleted {key} from S3");
      }
      catch (Exception ex)
      {
        _logger.LogError(ex, $"Error deleting file {key}");
      }
    }
  }
}
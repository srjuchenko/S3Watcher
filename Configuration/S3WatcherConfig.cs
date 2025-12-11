namespace S3Watcher.Configuration
{
  public class S3WatcherConfig
  {
    public string BucketName { get; set; } = string.Empty;
    public string TargetDirectory { get; set; } = string.Empty;
    public string[] AllowedExtensions { get; set; } = Array.Empty<string>();
    public bool DeleteAfterDownload { get; set; }
    public int PollingIntervalSeconds { get; set; } = 30;
    public int MaxConcurrentDownloads { get; set; } = 5;
  }
}
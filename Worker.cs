using Microsoft.Extensions.Options;
using S3Watcher.Configuration;
using S3Watcher.Services;

namespace S3Watcher
{
    public class Worker : BackgroundService
    {
        private readonly IS3Service _s3Service;
        private readonly S3WatcherConfig _config;
        private readonly ILogger<Worker> _logger;

        public Worker(IS3Service s3Service, IOptions<S3WatcherConfig> config, ILogger<Worker> logger)
        {
            _s3Service = s3Service;
            _config = config.Value;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                var files = await _s3Service.GetNewFilesAsync(_config.BucketName, _config.AllowedExtensions);

                if (files.Any())
                {
                    _logger.LogInformation($"Found {files.Count} files. Starting parallel download...");

                    var parallelOptions = new ParallelOptions
                    {
                        MaxDegreeOfParallelism = _config.MaxConcurrentDownloads,
                        CancellationToken = stoppingToken
                    };

                    await Parallel.ForEachAsync(files, parallelOptions, async (file, token) =>
                    {
                        await ProcessSingleFileAsync(file.Key, token);
                    });
                }

                await Task.Delay(TimeSpan.FromSeconds(_config.PollingIntervalSeconds), stoppingToken);
            }
        }

        private async Task ProcessSingleFileAsync(string key, CancellationToken token)
        {
            var localPath = Path.Combine(_config.TargetDirectory, key);

            if (File.Exists(localPath))
            {
                _logger.LogInformation($"File {localPath} already exists. Skipping download.");
                return;
            }

            try
            {
                _logger.LogInformation($"Starting download: {key}");
                await _s3Service.DownloadFileAsync(_config.BucketName, key, localPath);

                if (_config.DeleteAfterDownload)
                {
                    await _s3Service.DeleteFileAsync(_config.BucketName, key);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Failed to process {key}");
            }
        }
    }
}
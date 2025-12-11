using Amazon.S3;
using S3Watcher;
using S3Watcher.Configuration;
using S3Watcher.Services;

IHost host = Host.CreateDefaultBuilder(args)
    .ConfigureServices((hostContext, services) =>
    {
        services.Configure<S3WatcherConfig>(hostContext.Configuration.GetSection("S3WatcherConfig"));

        // This automatically picks up credentials from appsettings or local AWS profile
        services.AddAWSService<IAmazonS3>();

        // 3. Register our Custom S3 Logic
        services.AddSingleton<IS3Service, S3Service>();

        // 4. Register the Worker (The Watcher)
        services.AddHostedService<Worker>();
    })
    .Build();

await host.RunAsync();
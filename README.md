# S3Watcher Microservice

A lightweight .NET 6.0 microservice that monitors an AWS S3 bucket and automatically downloads new files to a local directory or PVC.

## Features
- **Parallel Processing:** Downloads multiple large files simultaneously.
- **Configurable Filtering:** Choose which file extensions to watch (e.g., `.txt`, `.csv`).
- **Auto-Delete:** Optional setting to delete files from S3 after successful download.
- **Dockerized:** Ready for deployment on local machines or Kubernetes.

## Prerequisites
1. **Docker** installed and running.
2. **AWS Credentials** configured on your host machine (usually in `~/.aws/credentials`).
3. An **S3 Bucket** created in AWS.

---

## Configuration

### 1. App Settings (`appsettings.json`)
Ensure the `TargetDirectory` is set to the **container's internal path**, not your Mac path.

```json
{
  "S3WatcherConfig": {
    "BucketName": "your-bucket-name",
    "TargetDirectory": "/app/downloads",
    "AllowedExtensions": [ ".txt", ".csv", ".json" ],
    "DeleteAfterDownload": false,
    "PollingIntervalSeconds": 10,
    "MaxConcurrentDownloads": 5
  }
}
```

### 2. Run Script (`run-watcher.sh`)
Open the `run-watcher.sh` file found in the root directory. You must edit the variables at the top to match your local environment.

**Key variable to change:**
- `LOCAL_DOWNLOAD_PATH`: This is the folder on your **Mac/PC** where you want the downloaded files to appear.

```bash
# Example configuration in run-watcher.sh
LOCAL_DOWNLOAD_PATH="/Users/yourname/Downloads/S3Target"
AWS_PROFILE_NAME="default"
```

---

## How to Build & Run

### Step 1: Build the Docker Image
Open your terminal in the project root folder and run the build command. This packages your .NET code into a lightweight container.

```bash
docker build -t s3watcher:v1 .
```

### Step 2: Run the Service
We use the provided shell script `run-watcher.sh` to handle the volume mapping (connecting your local folders and AWS credentials to the container).

1. Make the script executable (you only need to do this once):

   ```bash
   chmod +x run-watcher.sh
   ```
2. Start the watcher:

   ```bash
   ./run-watcher.sh 
   ```
   ### The script will automatically stop and remove any existing container named my-s3-watcher before starting a new one.

## Monitoring & Logs

Since the service runs in "detached" mode (`-d`), it will run in the background. To see what the microservice is doing (checking for files, downloading, errors), view the Docker logs:

```bash
docker logs -f my-s3-watcher
```
### (Press Ctrl+C to exit the logs view. The service will keep running in the background.)

## Troubleshooting

### Error: "No credentials found"
The container cannot see your AWS keys.
- **Check File:** Ensure you have a file at `~/.aws/credentials` on your machine.
- **Check Profile:** Ensure the `AWS_PROFILE` variable in `run-watcher.sh` matches the profile name in brackets inside your credentials file (e.g., `[default]`).

### Error: "Access Denied"
The AWS user has keys, but no permission.
- Ensure your IAM User has the following permissions policy for your bucket:
  - `s3:ListBucket`
  - `s3:GetObject`
  - `s3:DeleteObject` (Only required if `DeleteAfterDownload` is set to `true`)

### Error: "DirectoryNotFoundException"
The container cannot find the target folder.
- Ensure `TargetDirectory` in `appsettings.json` is set to the **Linux path** `/app/downloads`.
- Do **not** use your Mac path (e.g., `/Users/...`) inside `appsettings.json`.


#!/bin/bash

# --- CONFIGURATION START ---

# 1. The name of your Docker Image
IMAGE_NAME="s3watcher:v1"

# 2. The name for the running container
CONTAINER_NAME="my-s3-watcher"

# 3. PATH to your AWS folder on your Mac
# (Usually /Users/yourname/.aws)
AWS_CREDENTIALS_PATH="$HOME/.aws"

# 4. PATH to where you want the files to appear on your Mac
# (Replace the path below with your actual folder path)
LOCAL_DOWNLOAD_PATH="/Users/srjuchenko/Downloads/S3Target"

# 5. The AWS Profile to use (from your credentials file)
AWS_PROFILE_NAME="default"

# --- CONFIGURATION END ---

echo "Stopping old container if it exists..."
docker stop $CONTAINER_NAME 2>/dev/null || true
docker rm $CONTAINER_NAME 2>/dev/null || true

echo "Starting S3Watcher..."
docker run -d \
  --name $CONTAINER_NAME \
  -v "$AWS_CREDENTIALS_PATH:/root/.aws" \
  -v "$LOCAL_DOWNLOAD_PATH:/app/downloads" \
  -e AWS_PROFILE=$AWS_PROFILE_NAME \
  $IMAGE_NAME

echo "Success! Container is running."
echo "You can view logs by running: docker logs -f $CONTAINER_NAME"
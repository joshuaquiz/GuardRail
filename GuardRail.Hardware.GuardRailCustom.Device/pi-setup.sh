#!/bin/bash
# GuardRail Raspberry Pi Setup Script
# This script:
# 1. Sets a random hostname
# 2. Installs .NET 9
# 3. Configures the GuardRail.Hardware.GuardRailCustom.Device project as a startup service

# Exit on error
set -e

# Default values
GIT_REPO=""
LOCAL_PATH=""
DEPLOY_METHOD="local"
BRANCH="main"

# Parse command line arguments
while [[ $# -gt 0 ]]; do
  case $1 in
    --git)
      DEPLOY_METHOD="git"
      GIT_REPO="$2"
      shift 2
      ;;
    --branch)
      BRANCH="$2"
      shift 2
      ;;
    --local)
      DEPLOY_METHOD="local"
      LOCAL_PATH="$2"
      shift 2
      ;;
    --help)
      echo "Usage: $0 [options]"
      echo "Options:"
      echo "  --git REPO_URL    Clone from git repository"
      echo "  --branch BRANCH   Git branch to use (default: main)"
      echo "  --local PATH      Use local project files"
      echo "  --help            Show this help message"
      exit 0
      ;;
    *)
      echo "Unknown option: $1"
      exit 1
      ;;
  esac
done

echo "Starting GuardRail Raspberry Pi setup..."

# Generate a random hostname with prefix 'guardrail-'
RANDOM_SUFFIX=$(cat /dev/urandom | tr -dc 'a-z0-9' | fold -w 6 | head -n 1)
NEW_HOSTNAME="guardrail-$RANDOM_SUFFIX"

echo "Setting hostname to: $NEW_HOSTNAME"
sudo hostnamectl set-hostname $NEW_HOSTNAME
echo "127.0.1.1 $NEW_HOSTNAME" | sudo tee -a /etc/hosts

# Update system
echo "Updating system packages..."
sudo apt-get update
sudo apt-get upgrade -y

# Install dependencies
echo "Installing dependencies..."
sudo apt-get install -y curl libunwind8 gettext apt-transport-https git

# Install .NET 9
echo "Installing .NET 9..."
# Add Microsoft package repository
sudo curl -sSL https://dot.net/v1/dotnet-install.sh | bash /dev/stdin --channel STS
echo 'export DOTNET_ROOT=$HOME/.dotnet' >> ~/.bashrc
echo 'export PATH=$PATH:$HOME/.dotnet' >> ~/.bashrc

# Verify installation
$HOME/.dotnet/dotnet --version

# Create directory for GuardRail
echo "Setting up GuardRail project..."
GUARDRAIL_DIR="/home/pi/guardrail"
mkdir -p $GUARDRAIL_DIR
rm -rf $GUARDRAIL_DIR

# Deploy the project based on the selected method
if [ "$DEPLOY_METHOD" = "git" ]; then
  if [ -z "$GIT_REPO" ]; then
    echo "Error: Git repository URL not provided. Use --git REPO_URL"
    exit 1
  fi
  echo "Cloning repository from $GIT_REPO (branch: $BRANCH)..."
  git clone -b $BRANCH $GIT_REPO $GUARDRAIL_DIR
elif [ "$DEPLOY_METHOD" = "local" ]; then
  if [ -n "$LOCAL_PATH" ]; then
    echo "Copying project from local path: $LOCAL_PATH"
    cp -r $LOCAL_PATH/* $GUARDRAIL_DIR/
  else
    echo "Warning: No local path provided. Assuming project files will be copied manually."
  fi
fi

# Create the systemd service file
echo "Creating systemd service..."
sudo bash -c 'cat > /etc/systemd/system/guardrail-device.service << EOL
[Unit]
Description=GuardRail Device Service
After=network.target

[Service]
WorkingDirectory=/home/pi/guardrail/GuardRail.Hardware.GuardRailCustom.Device
ExecStart=/usr/bin/dotnet /home/pi/guardrail/GuardRail.Hardware.GuardRailCustom.Device/bin/Release/net9.0/GuardRail.Hardware.GuardRailCustom.Device.dll
Restart=always
# Restart service after 10 seconds if it crashes
RestartSec=10
KillSignal=SIGINT
SyslogIdentifier=guardrail-device
User=joshua
Environment=ASPNETCORE_ENVIRONMENT=Production
Environment=DOTNET_PRINT_TELEMETRY_MESSAGE=false

[Install]
WantedBy=multi-user.target
EOL'

# Build the project
echo "Building the GuardRail.Hardware.GuardRailCustom.Device project..."
cd $GUARDRAIL_DIR

# Check if the project file exists
if [ -f "$GUARDRAIL_DIR/GuardRail.Hardware.GuardRailCustom.Device/GuardRail.Hardware.GuardRailCustom.Device.csproj" ]; then
  $HOME/.dotnet/dotnet publish "$GUARDRAIL_DIR/GuardRail.Hardware.GuardRailCustom.Device/GuardRail.Hardware.GuardRailCustom.Device.csproj" -c Release
else
  echo "Error: Project file not found. Please check the project path."
  exit 1
fi

# Create appsettings.Production.json if it doesn't exist
if [ ! -f "$GUARDRAIL_DIR/GuardRail.Hardware.GuardRailCustom.Device/bin/Release/net9.0/appsettings.Production.json" ]; then
  echo "Creating Production settings file..."
  cp "$GUARDRAIL_DIR/GuardRail.Hardware.GuardRailCustom.Device/bin/Release/net9.0/appsettings.json" \
     "$GUARDRAIL_DIR/GuardRail.Hardware.GuardRailCustom.Device/bin/Release/net9.0/appsettings.Production.json"
fi

# Enable and start the service
echo "Enabling and starting the GuardRail service..."
sudo systemctl enable guardrail-device.service
sudo systemctl start guardrail-device.service
sudo systemctl status guardrail-device.service

# Configure the service to start on boot
echo "Configuring service to start on boot..."
sudo systemctl enable guardrail-device.service

echo "Setup complete! GuardRail device is now running as a service."
echo "Hostname: $NEW_HOSTNAME"
echo "Service status can be checked with: sudo systemctl status guardrail-device.service"
echo "Project location: $GUARDRAIL_DIR"

# Make the script executable
chmod +x $GUARDRAIL_DIR/pi-setup.sh

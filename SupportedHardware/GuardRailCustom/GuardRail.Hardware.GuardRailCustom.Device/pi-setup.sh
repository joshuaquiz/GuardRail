#!/bin/bash
# GuardRail Raspberry Pi Setup Script
# This script:
# 1. Sets a random hostname
# 2. Installs .NET 9
# 3. Configures the GuardRail.Hardware.GuardRailCustom.Device project as a startup service

# Exit on error
set -e

# Default values
USERNAME="gruser"

# Parse command line arguments
while [[ $# -gt 0 ]]; do
  case $1 in
    --username)
      USERNAME="$2"
      shift 2
      ;;
    --help)
      echo "Usage: $0 [options]"
      echo "Options:"
      echo "  --username USERNAME  Specify the username (default: pi)"
      echo "  --help              Show this help message"
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
echo 'export GUARDRAIL_DIR=$HOME/guardrail' >> ~/.bashrc

# Set environment variables for current session
export DOTNET_ROOT=$HOME/.dotnet
export PATH=$PATH:$HOME/.dotnet
export GUARDRAIL_DIR=$HOME/guardrail

# Create directory for GuardRail
echo "Setting up GuardRail project..."

# Create the systemd service file
echo "Creating systemd service..."
sudo bash -c "cat > /etc/systemd/system/guardrail-device.service << EOL
[Unit]
Description=GuardRail Device Service
After=network.target

[Service]
User=root
WorkingDirectory=$GUARDRAIL_DIR
ExecStart=dotnet $GUARDRAIL_DIR/GuardRail.Hardware.GuardRailCustom.Device.dll
Restart=always
# Restart service after 10 seconds if it crashes
RestartSec=10
KillSignal=SIGINT
SyslogIdentifier=guardrail-device
Environment=ASPNETCORE_ENVIRONMENT=Production
Environment=DOTNET_PRINT_TELEMETRY_MESSAGE=false

[Install]
WantedBy=multi-user.target
EOL"

# Enable and start the service
echo "Enabling and starting the GuardRail service..."
sudo systemctl enable guardrail-device.service
sudo systemctl start guardrail-device.service
sudo systemctl status guardrail-device.service

echo "Setup complete! GuardRail device is now running as a service."
echo "Hostname: $NEW_HOSTNAME"
echo "Service status can be checked with: sudo systemctl status guardrail-device.service"
echo "Project location: $GUARDRAIL_DIR"

# GuardRail Raspberry Pi Setup

This script automates the setup of a Raspberry Pi for the GuardRail.Hardware.GuardRailCustom.Device project. It performs the following tasks:

1. Sets a random hostname with the prefix "guardrail-"
2. Installs .NET 9 SDK and runtime
3. Deploys the GuardRail project (from Git or local files)
4. Builds the GuardRail.Hardware.GuardRailCustom.Device project
5. Configures it as a systemd service to run on startup

## Prerequisites

- Raspberry Pi with Raspberry Pi OS (Debian-based)
- Internet connection for package installation
- User with sudo privileges

## Usage

1. Copy the `pi-setup.sh` script to your Raspberry Pi
2. Make it executable: `chmod +x pi-setup.sh`
3. Run the script with one of the following options:

### Option 1: Deploy from Git repository

```bash
./pi-setup.sh --git https://github.com/yourusername/GuardRail.git
```

You can specify a branch:

```bash
./pi-setup.sh --git https://github.com/yourusername/GuardRail.git --branch develop
```

### Option 2: Deploy from local files

If you've already copied the project files to the Raspberry Pi:

```bash
./pi-setup.sh --local /path/to/local/project
```

### Option 3: Manual deployment

If you plan to copy the files manually after running the script:

```bash
./pi-setup.sh
```

## Service Management

After installation, you can manage the GuardRail service using standard systemd commands:

- Check status: `sudo systemctl status guardrail-device.service`
- Stop service: `sudo systemctl stop guardrail-device.service`
- Start service: `sudo systemctl start guardrail-device.service`
- Restart service: `sudo systemctl restart guardrail-device.service`
- View logs: `sudo journalctl -u guardrail-device.service`

## Project Location

The GuardRail project is installed to `/home/pi/guardrail/`.

## Troubleshooting

If the service fails to start, check the logs:

```bash
sudo journalctl -u guardrail-device.service -f
```

Common issues:
- Missing dependencies: Make sure all required packages are installed
- Permission issues: Ensure the service user (pi) has access to the project directory
- .NET version mismatch: Verify that .NET 9 is properly installed

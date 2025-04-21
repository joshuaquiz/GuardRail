# Deploy to Raspberry Pi Script

This PowerShell script automates the process of deploying the GuardRail project to a Raspberry Pi. It handles:

1. Connecting to the Raspberry Pi via SSH
2. Copying the setup script to the Pi
3. Making the script executable
4. Executing the setup script with appropriate parameters
5. Optionally building and copying the project files

## Prerequisites

- Windows machine with PowerShell
- PuTTY tools installed and available in PATH (plink and pscp)
- .NET 9 SDK installed on the Windows machine (if using the build option)
- Raspberry Pi with SSH enabled
- User account on the Raspberry Pi with sudo privileges

## Installation

1. Download PuTTY tools from https://www.putty.org/ if not already installed
2. Ensure plink and pscp are in your PATH

## Usage

### Basic Usage

```powershell
.\deploy-to-pi.ps1 -PiHostname "raspberrypi.local" -PiUsername "pi" -PiPassword "raspberry"
```

### Using SSH Key Authentication

```powershell
.\deploy-to-pi.ps1 -PiHostname "raspberrypi.local" -PiUsername "pi" -SshKeyPath "C:\path\to\private_key.ppk"
```

### Deploy from Git Repository

```powershell
.\deploy-to-pi.ps1 -PiHostname "raspberrypi.local" -PiUsername "pi" -PiPassword "raspberry" -DeployMethod "git" -GitRepo "https://github.com/yourusername/GuardRail.git" -GitBranch "main"
```

### Build and Deploy Project Files

```powershell
.\deploy-to-pi.ps1 -PiHostname "raspberrypi.local" -PiUsername "pi" -PiPassword "raspberry" -BuildAndCopy
```

### Deploy from Local Directory

```powershell
.\deploy-to-pi.ps1 -PiHostname "raspberrypi.local" -PiUsername "pi" -PiPassword "raspberry" -DeployMethod "local" -LocalProjectPath "C:\path\to\project"
```

## Parameters

| Parameter | Description | Required |
|-----------|-------------|----------|
| PiHostname | Hostname or IP address of the Raspberry Pi | Yes |
| PiUsername | Username for SSH login | Yes |
| PiPassword | Password for SSH login | Yes (if SshKeyPath not provided) |
| SshKeyPath | Path to SSH private key file (.ppk) | Yes (if PiPassword not provided) |
| SetupScriptPath | Path to the pi-setup.sh script | No (default: .\GuardRail.Hardware.GuardRailCustom.Device\pi-setup.sh) |
| DeployMethod | Deployment method: "git" or "local" | No (default: "local") |
| GitRepo | Git repository URL | No (required if DeployMethod is "git") |
| GitBranch | Git branch to use | No (default: "main") |
| LocalProjectPath | Path to local project files | No (required if DeployMethod is "local" and not using BuildAndCopy) |
| BuildAndCopy | Switch to build the project and copy the published files | No |

## Examples

### Example 1: Deploy using password authentication and build the project

```powershell
.\deploy-to-pi.ps1 -PiHostname "192.168.1.100" -PiUsername "pi" -PiPassword "raspberry" -BuildAndCopy
```

### Example 2: Deploy from a Git repository using SSH key authentication

```powershell
.\deploy-to-pi.ps1 -PiHostname "raspberrypi.local" -PiUsername "pi" -SshKeyPath "C:\Users\YourName\.ssh\id_rsa.ppk" -DeployMethod "git" -GitRepo "https://github.com/yourusername/GuardRail.git"
```

## Troubleshooting

- **SSH Connection Issues**: Ensure SSH is enabled on the Raspberry Pi and the hostname/IP is correct
- **Authentication Failures**: Verify username and password or SSH key
- **PuTTY Tools Not Found**: Make sure plink and pscp are installed and in your PATH
- **Build Failures**: Check that .NET 9 SDK is installed on your Windows machine
- **Permission Issues**: Ensure the Pi user has sudo privileges

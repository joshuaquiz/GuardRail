param (
    [Parameter(Mandatory=$false)]
    [string]$PiHostname = "raspberrypi",
    
    [Parameter(Mandatory=$false)]
    [string]$PiUsername = "joshua",
    
    [Parameter(Mandatory=$false)]
    [string]$PiPassword = "asdf"
)

# Check if plink and pscp are available
function Test-Command {
    param ($Command)
    $oldPreference = $ErrorActionPreference
    $ErrorActionPreference = 'stop'
    try {
        if (Get-Command $Command) { return $true }
    } catch {
        Write-Host "$Command is not available. Please install PuTTY tools." -ForegroundColor Red
        return $false
    } finally {
        $ErrorActionPreference = $oldPreference
    }
}

if (-not (Test-Command "ssh") -or -not (Test-Command "pscp")) {
    Write-Host "This script requires PuTTY tools (ssh and pscp). Please install them and add them to your PATH." -ForegroundColor Red
    Write-Host "You can download PuTTY from: https://www.putty.org/" -ForegroundColor Yellow
    exit 1
}

# Build and copy the project if requested
Write-Host "Building the GuardRail project..." -ForegroundColor Cyan

try {
    # Create a temporary directory to prepare files for copying
    $tempDir = Join-Path $env:TEMP "GuardRailDeploy"
    if (Test-Path $tempDir) {
        Remove-Item -Path $tempDir -Recurse -Force
    }
    New-Item -ItemType Directory -Path $tempDir | Out-Null

    # Build the project
    dotnet publish "GuardRail.Hardware.GuardRailCustom.Device.csproj" -c Release -o $tempDir
    
    if ($LASTEXITCODE -ne 0) {
        Write-Host "Failed to build the project." -ForegroundColor Red
        exit 1
    }
    
    # Set the local project path to the temp directory
    $LocalProjectPath = $tempDir

    # Prepare the setup command based on deployment method
    # For local deployment, we'll create a directory on the Pi and copy files there
    $remoteProjectPath = "/home/$PiUsername/guardrail"
    
    Write-Host "Copying project files to Raspberry Pi..." -ForegroundColor Cyan

    # Step 1: Copy the setup script to the Raspberry Pi
    Write-Host "Copying setup script to Raspberry Pi..." -ForegroundColor Cyan
    pscp -pw $PiPassword "./pi-setup.sh" "$PiUsername@$PiHostname`:/home/$PiUsername/pi-setup.sh"
    
    if ($LASTEXITCODE -ne 0) {
        Write-Host "Failed to copy setup script to Raspberry Pi." -ForegroundColor Red
        exit 1
    }

    # Create the remote directory
    $mkdirCommand = "mkdir -p $remoteProjectPath"
    ssh -o PreferredAuthentications=password -o PubkeyAuthentication=no -v -p 22 $PiUsername@$PiHostname $mkdirCommand
    
    if ($LASTEXITCODE -ne 0) {
        Write-Host "Failed to create directory on Raspberry Pi." -ForegroundColor Red
        exit 1
    }

    # Copy the project files
    pscp -pw $PiPassword -r "$LocalProjectPath/*" "$PiUsername@$PiHostname`:$remoteProjectPath"
    
    if ($LASTEXITCODE -ne 0) {
        Write-Host "Failed to copy project files to Raspberry Pi." -ForegroundColor Red
        exit 1
    }

    # Step 2: Make the script executable and running it
    Write-Host "Making script executable and running it..." -ForegroundColor Cyan
    ssh -o PreferredAuthentications=password -o PubkeyAuthentication=no -v -p 22 $PiUsername@$PiHostname "sudo chmod +x /home/$PiUsername/pi-setup.sh && sudo /home/$PiUsername/pi-setup.sh --username $PiUsername"
    
    if ($LASTEXITCODE -ne 0) {
        Write-Host "Failed to execute setup script on Raspberry Pi." -ForegroundColor Red
        exit 1
    }
    
    Write-Host "Deployment completed successfully!" -ForegroundColor Green
    
} finally {
    if ($tempDir -and (Test-Path $tempDir)) {
        Remove-Item -Path $tempDir -Recurse -Force
    }
}

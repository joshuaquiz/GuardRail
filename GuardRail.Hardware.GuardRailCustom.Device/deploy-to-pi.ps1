param (
    [Parameter(Mandatory=$true)]
    [string]$PiHostname,
    
    [Parameter(Mandatory=$true)]
    [string]$PiUsername,
    
    [Parameter(Mandatory=$false)]
    [string]$PiPassword,
    
    [Parameter(Mandatory=$false)]
    [string]$SshKeyPath,
    
    [Parameter(Mandatory=$false)]
    [string]$SetupScriptPath = ".\GuardRail.Hardware.GuardRailCustom.Device\pi-setup.sh",
    
    [Parameter(Mandatory=$false)]
    [string]$DeployMethod = "local",
    
    [Parameter(Mandatory=$false)]
    [string]$GitRepo,
    
    [Parameter(Mandatory=$false)]
    [string]$GitBranch = "main",
    
    [Parameter(Mandatory=$false)]
    [string]$LocalProjectPath,
    
    [Parameter(Mandatory=$false)]
    [switch]$BuildAndCopy
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

if (-not (Test-Command "plink") -or -not (Test-Command "pscp")) {
    Write-Host "This script requires PuTTY tools (plink and pscp). Please install them and add them to your PATH." -ForegroundColor Red
    Write-Host "You can download PuTTY from: https://www.putty.org/" -ForegroundColor Yellow
    exit 1
}

# Validate parameters
if (-not $PiPassword -and -not $SshKeyPath) {
    Write-Host "Either PiPassword or SshKeyPath must be provided." -ForegroundColor Red
    exit 1
}

# Prepare authentication parameters
$authParams = ""
if ($SshKeyPath) {
    $authParams = "-i `"$SshKeyPath`""
} else {
    # Create a temporary file to store the password
    $tempPwFile = [System.IO.Path]::GetTempFileName()
    Set-Content -Path $tempPwFile -Value $PiPassword -NoNewline
    $authParams = "-pw $PiPassword"
}

# Check if the setup script exists
if (-not (Test-Path $SetupScriptPath)) {
    Write-Host "Setup script not found at: $SetupScriptPath" -ForegroundColor Red
    exit 1
}

# Build and copy the project if requested
if ($BuildAndCopy) {
    Write-Host "Building the GuardRail project..." -ForegroundColor Cyan
    
    # Determine the solution directory
    $solutionDir = (Get-Location).Path
    
    # Build the project
    dotnet publish "$solutionDir\GuardRail.Hardware.GuardRailCustom.Device\GuardRail.Hardware.GuardRailCustom.Device.csproj" -c Release
    
    if ($LASTEXITCODE -ne 0) {
        Write-Host "Failed to build the project." -ForegroundColor Red
        exit 1
    }
    
    # Create a temporary directory to prepare files for copying
    $tempDir = Join-Path $env:TEMP "GuardRailDeploy"
    if (Test-Path $tempDir) {
        Remove-Item -Path $tempDir -Recurse -Force
    }
    New-Item -ItemType Directory -Path $tempDir | Out-Null
    
    # Copy the published files
    $publishDir = "$solutionDir\GuardRail.Hardware.GuardRailCustom.Device\bin\Release\net9.0\publish"
    Copy-Item -Path "$publishDir\*" -Destination $tempDir -Recurse
    
    # Set the local project path to the temp directory
    $LocalProjectPath = $tempDir
    $DeployMethod = "local"
}

# Prepare the setup command based on deployment method
$setupCommand = "./pi-setup.sh"
if ($DeployMethod -eq "git" -and $GitRepo) {
    $setupCommand += " --git `"$GitRepo`""
    if ($GitBranch) {
        $setupCommand += " --branch `"$GitBranch`""
    }
} elseif ($DeployMethod -eq "local" -and $LocalProjectPath) {
    # For local deployment, we'll create a directory on the Pi and copy files there
    $remoteProjectPath = "/home/pi/guardrail_source"
    
    Write-Host "Copying project files to Raspberry Pi..." -ForegroundColor Cyan
    
    # Create the remote directory
    $mkdirCommand = "mkdir -p $remoteProjectPath"
    plink $authParams "$PiUsername@$PiHostname" $mkdirCommand
    
    if ($LASTEXITCODE -ne 0) {
        Write-Host "Failed to create directory on Raspberry Pi." -ForegroundColor Red
        exit 1
    }
    
    # Copy the project files
    pscp $authParams -r "$LocalProjectPath\*" "$PiUsername@$PiHostname`:$remoteProjectPath"
    
    if ($LASTEXITCODE -ne 0) {
        Write-Host "Failed to copy project files to Raspberry Pi." -ForegroundColor Red
        exit 1
    }
    
    $setupCommand += " --local `"$remoteProjectPath`""
}

try {
    # Step 1: Copy the setup script to the Raspberry Pi
    Write-Host "Copying setup script to Raspberry Pi..." -ForegroundColor Cyan
    pscp $authParams $SetupScriptPath "$PiUsername@$PiHostname`:/home/$PiUsername/pi-setup.sh"
    
    if ($LASTEXITCODE -ne 0) {
        Write-Host "Failed to copy setup script to Raspberry Pi." -ForegroundColor Red
        exit 1
    }
    
    # Step 2: Make the script executable and run it
    Write-Host "Making script executable and running it..." -ForegroundColor Cyan
    plink $authParams "$PiUsername@$PiHostname" "chmod +x /home/$PiUsername/pi-setup.sh && $setupCommand"
    
    if ($LASTEXITCODE -ne 0) {
        Write-Host "Failed to execute setup script on Raspberry Pi." -ForegroundColor Red
        exit 1
    }
    
    Write-Host "Deployment completed successfully!" -ForegroundColor Green
    
} finally {
    # Clean up temporary files
    if ($tempPwFile -and (Test-Path $tempPwFile)) {
        Remove-Item -Path $tempPwFile -Force
    }
    
    if ($BuildAndCopy -and $tempDir -and (Test-Path $tempDir)) {
        Remove-Item -Path $tempDir -Recurse -Force
    }
}

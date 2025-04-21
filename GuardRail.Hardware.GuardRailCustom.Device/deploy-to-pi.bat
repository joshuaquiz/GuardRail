@echo off
REM GuardRail Raspberry Pi Deployment Batch File
REM This batch file provides a simple interface to the deploy-to-pi.ps1 script

echo GuardRail Raspberry Pi Deployment Tool
echo =====================================

set /p PI_HOSTNAME=Enter Raspberry Pi hostname or IP: 
set /p PI_USERNAME=Enter username (default: pi): 
if "%PI_USERNAME%"=="" set PI_USERNAME=pi

set /p AUTH_METHOD=Use password or SSH key? (p/k, default: p): 
if "%AUTH_METHOD%"=="" set AUTH_METHOD=p

if /i "%AUTH_METHOD%"=="p" (
    set /p PI_PASSWORD=Enter password: 
    set AUTH_PARAM=-PiPassword "%PI_PASSWORD%"
) else (
    set /p SSH_KEY_PATH=Enter path to SSH key (.ppk): 
    set AUTH_PARAM=-SshKeyPath "%SSH_KEY_PATH%"
)

set /p DEPLOY_METHOD=Choose deployment method (1=Build and copy, 2=Git repository, 3=Local files): 

if "%DEPLOY_METHOD%"=="1" (
    set DEPLOY_PARAMS=-BuildAndCopy
) else if "%DEPLOY_METHOD%"=="2" (
    set /p GIT_REPO=Enter Git repository URL: 
    set /p GIT_BRANCH=Enter Git branch (default: main): 
    if "%GIT_BRANCH%"=="" set GIT_BRANCH=main
    set DEPLOY_PARAMS=-DeployMethod git -GitRepo "%GIT_REPO%" -GitBranch "%GIT_BRANCH%"
) else if "%DEPLOY_METHOD%"=="3" (
    set /p LOCAL_PATH=Enter path to local project files: 
    set DEPLOY_PARAMS=-DeployMethod local -LocalProjectPath "%LOCAL_PATH%"
) else (
    echo Invalid deployment method selected.
    exit /b 1
)

echo.
echo Deploying to Raspberry Pi...
echo.

powershell -ExecutionPolicy Bypass -File deploy-to-pi.ps1 -PiHostname "%PI_HOSTNAME%" -PiUsername "%PI_USERNAME%" %AUTH_PARAM% %DEPLOY_PARAMS%

if %ERRORLEVEL% NEQ 0 (
    echo Deployment failed with error code %ERRORLEVEL%
    exit /b %ERRORLEVEL%
)

echo.
echo Deployment completed successfully!
echo.

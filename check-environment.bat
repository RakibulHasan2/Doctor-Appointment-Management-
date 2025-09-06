@echo off
echo ================================================
echo Environment Check for Doctor Appointment API
echo ================================================
echo.

echo Checking system requirements...
echo.

echo [1] Checking .NET 8.0 SDK...
dotnet --version 2>nul
if %errorlevel% equ 0 (
    echo ✅ .NET SDK found: 
    dotnet --version
) else (
    echo ❌ .NET 8.0 SDK not found
    echo Download from: https://dotnet.microsoft.com/en-us/download/dotnet/8.0
)
echo.

echo [2] Checking Git...
git --version 2>nul
if %errorlevel% equ 0 (
    echo ✅ Git found: 
    git --version
) else (
    echo ❌ Git not found
    echo Download from: https://git-scm.com/downloads
)
echo.

echo [3] Checking if project exists...
if exist "DoctorAppointmentAPI.csproj" (
    echo ✅ Project file found
) else (
    echo ❌ Project file not found
    echo Make sure you're in the correct directory
)
echo.

echo [4] Checking MongoDB connection (if project exists)...
if exist "appsettings.json" (
    echo ✅ Configuration file found
    echo Please verify MongoDB connection string in appsettings.json
) else (
    echo ⚠️  Configuration file not found
)
echo.

echo [5] Checking NuGet packages...
if exist "obj" (
    echo ✅ Build artifacts found (packages likely restored)
) else (
    echo ⚠️  No build artifacts found
    echo Run 'dotnet restore' to install packages
)
echo.

echo ================================================
echo Environment Check Complete
echo ================================================
echo.
echo Next steps:
echo 1. If any items show ❌, install the missing software
echo 2. If all checks pass ✅, run 'start.bat' to launch the API
echo 3. For detailed setup, see DEVELOPER_SETUP.md
echo.

pause

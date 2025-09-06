@echo off
echo ================================================
echo Doctor Appointment Management System API
echo Quick Setup Script for Windows
echo ================================================
echo.

echo [1/6] Checking .NET 8.0 installation...
dotnet --version
if %errorlevel% neq 0 (
    echo ERROR: .NET 8.0 SDK not found!
    echo Please download and install .NET 8.0 SDK from:
    echo https://dotnet.microsoft.com/en-us/download/dotnet/8.0
    pause
    exit /b 1
)
echo ✅ .NET 8.0 SDK found!
echo.

echo [2/6] Restoring NuGet packages...
dotnet restore
if %errorlevel% neq 0 (
    echo ERROR: Failed to restore packages!
    pause
    exit /b 1
)
echo ✅ Packages restored successfully!
echo.

echo [3/6] Building the project...
dotnet build
if %errorlevel% neq 0 (
    echo ERROR: Build failed!
    pause
    exit /b 1
)
echo ✅ Project built successfully!
echo.

echo [4/6] Checking MongoDB connection...
echo Please ensure MongoDB is running on:
echo - Local: mongodb://localhost:27017
echo - OR configured in appsettings.json for MongoDB Atlas
echo.

echo [5/6] Starting the API server...
echo The API will be available at:
echo - HTTP: http://localhost:5289
echo - Swagger UI: http://localhost:5289/swagger
echo.
echo Press Ctrl+C to stop the server
echo.

echo [6/6] Launching API server...
dotnet run

pause

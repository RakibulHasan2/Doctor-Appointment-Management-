# 🚀 Developer Setup Guide - Doctor Appointment Management System

## 📋 What You Need Before Starting

### 1. Prerequisites Checklist
Before you start, make sure you have these installed on your machine:

- ✅ **Visual Studio Code** (or Visual Studio 2022)
- ✅ **.NET 8.0 SDK** - [Download here](https://dotnet.microsoft.com/en-us/download/dotnet/8.0)
- ✅ **MongoDB** - Choose one option:
  - **Option A**: [MongoDB Community Server](https://www.mongodb.com/try/download/community) (Local installation)
  - **Option B**: [MongoDB Atlas](https://cloud.mongodb.com/) (Cloud - Free tier available)
- ✅ **Git** - [Download here](https://git-scm.com/downloads)

### 2. Recommended VS Code Extensions
Install these extensions for the best development experience:
- **C# for Visual Studio Code** (Microsoft)
- **MongoDB for VS Code** (MongoDB)
- **REST Client** (or **Thunder Client** for API testing)
- **GitLens** (Git supercharged)

## 🛠️ Step-by-Step Setup Instructions

### Step 1: Clone the Repository
```bash
# Open terminal/command prompt and run:
git clone https://github.com/RakibulHasan2/Doctor-Appointment-Management-.git

# Navigate to the backend API folder
cd Doctor-Appointment-Management-/backend/DoctorAppointmentAPI
```

### Step 2: Verify .NET Installation
```bash
# Check if .NET 8.0 is installed
dotnet --version

# You should see something like: 8.0.xxx
# If not, download and install .NET 8.0 SDK
```

### Step 3: Install Project Dependencies
```bash
# Restore all NuGet packages
dotnet restore

# This will download all required packages including:
# - MongoDB.Driver
# - AutoMapper
# - BCrypt.Net-Next
# - And others...
```

### Step 4: Setup MongoDB Database

#### Option A: Local MongoDB Setup
1. **Install MongoDB Community Server**
   - Download from [MongoDB Download Center](https://www.mongodb.com/try/download/community)
   - Install with default settings
   - MongoDB will run on `mongodb://localhost:27017` by default

2. **Verify MongoDB is running**
   ```bash
   # Open command prompt/terminal and run:
   mongosh
   
   # You should see MongoDB shell if installation is successful
   # Type 'exit' to close the shell
   ```

#### Option B: MongoDB Atlas (Cloud) Setup
1. **Create a free account** at [MongoDB Atlas](https://cloud.mongodb.com/)
2. **Create a new cluster** (free tier M0 is sufficient)
3. **Create a database user** with username/password
4. **Get your connection string** (looks like: `mongodb+srv://username:password@cluster.mongodb.net/`)

### Step 5: Configure Database Connection
1. **Open the project in VS Code**
   ```bash
   code .
   ```

2. **Update `appsettings.json`**
   ```json
   {
     "ConnectionStrings": {
       "MongoDB": "YOUR_MONGODB_CONNECTION_STRING_HERE"
     },
     "Logging": {
       "LogLevel": {
         "Default": "Information",
         "Microsoft.AspNetCore": "Warning"
       }
     },
     "AllowedHosts": "*"
   }
   ```

   **Examples:**
   - **Local MongoDB**: `"mongodb://localhost:27017/DoctorAppointmentDB"`
   - **MongoDB Atlas**: `"mongodb+srv://yourusername:yourpassword@yourcluster.mongodb.net/DoctorAppointmentDB"`

### Step 6: Build the Project
```bash
# Build the project to check for errors
dotnet build

# You should see: "Build succeeded. 0 Warning(s) 0 Error(s)"
```

### Step 7: Run the Application
```bash
# Start the API server
dotnet run

# You should see output like:
# Building...
# info: Microsoft.Hosting.Lifetime[14]
#       Now listening on: http://localhost:5289
# info: Microsoft.Hosting.Lifetime[0]
#       Application started. Press Ctrl+C to shut down.
```

### Step 8: Test the API
Once the server is running, you can test it:

1. **Open your browser** and go to: `http://localhost:5289/swagger`
   - This opens the Swagger UI where you can test all API endpoints

2. **Test a simple endpoint**:
   - In Swagger, try the `GET /api/specialties` endpoint
   - Click "Try it out" then "Execute"
   - You should get a response (even if it's an empty array)

## 🧪 Quick Test Workflow

### Test the Complete System
1. **Create a User (Patient)**
   ```bash
   POST http://localhost:5289/api/users/register
   Content-Type: application/json

   {
     "fullName": "John Doe",
     "email": "john@example.com",
     "password": "Test123!",
     "phoneNumber": "1234567890",
     "role": "Patient"
   }
   ```

2. **Create a Doctor User**
   ```bash
   POST http://localhost:5289/api/users/register
   Content-Type: application/json

   {
     "fullName": "Dr. Smith",
     "email": "drsmith@example.com",
     "password": "Test123!",
     "phoneNumber": "0987654321",
     "role": "Doctor"
   }
   ```

3. **Create a Specialty**
   ```bash
   POST http://localhost:5289/api/specialties
   Content-Type: application/json

   {
     "name": "Cardiology",
     "description": "Heart and cardiovascular system"
   }
   ```

4. **Create Doctor Profile**
   ```bash
   POST http://localhost:5289/api/doctors
   Content-Type: application/json

   {
     "userId": "USER_ID_FROM_STEP_2",
     "specialtyId": "SPECIALTY_ID_FROM_STEP_3",
     "licenseNumber": "DOC12345",
     "experience": 5,
     "qualification": "MD Cardiology",
     "consultationFee": 150.00
   }
   ```

## 📁 Project Structure Overview

```
DoctorAppointmentAPI/
├── Controllers/              # API endpoints
│   ├── UsersController.cs   # User management
│   ├── DoctorsController.cs # Doctor management  
│   ├── AppointmentsController.cs # Appointments
│   └── SpecialtiesController.cs # Medical specialties
├── Services/                # Business logic
├── Models/                  # Database entities
├── DTOs/                    # Data transfer objects
├── Configuration/           # App configuration
├── Documentation/           # API docs and guides
├── appsettings.json        # Configuration file
└── Program.cs              # Application entry point
```

## 🔧 Development Tools & Tips

### 1. Database Viewing
- **MongoDB Compass**: GUI tool for viewing your database
- **VS Code MongoDB Extension**: View collections directly in VS Code

### 2. API Testing Tools
- **Swagger UI**: Built-in at `http://localhost:5289/swagger`
- **Postman**: Import `Documentation/Postman_Collection.json`
- **VS Code REST Client**: Use `.http` files in the project

### 3. Debugging
```bash
# Run in debug mode
dotnet run --configuration Debug

# View detailed logs
dotnet run --verbosity detailed
```

## 🚨 Common Issues & Solutions

### Issue 1: "Connection refused" or MongoDB connection errors
**Solution**: 
- Make sure MongoDB is running locally, OR
- Check your MongoDB Atlas connection string is correct
- Verify network access in MongoDB Atlas settings

### Issue 2: ".NET 8.0 not found"
**Solution**: 
- Download and install .NET 8.0 SDK from Microsoft
- Restart your terminal/VS Code after installation

### Issue 3: Port already in use
**Solution**:
```bash
# Kill process using port 5289
netstat -ano | findstr :5289
taskkill /PID <process_id> /F

# Or change port in launchSettings.json
```

### Issue 4: Package restore failures
**Solution**:
```bash
# Clear NuGet cache and restore
dotnet nuget locals all --clear
dotnet restore
```

## 📞 Need Help?

### Check These Resources:
1. **API Documentation**: `Documentation/API_Documentation.md`
2. **Database Setup**: `Documentation/Database_Setup.md`
3. **Test Scenarios**: `Documentation/Test_Scenarios.md`

### Project Features Summary:
- ✅ User registration and management (Patient, Doctor, Admin roles)
- ✅ Doctor profile creation and approval workflow
- ✅ Doctor rejection functionality with reasons
- ✅ Appointment booking and management
- ✅ Medical specialty management
- ✅ Real-time availability checking
- ✅ Complete CRUD operations for all entities
- ✅ Input validation and error handling
- ✅ Swagger documentation
- ✅ MongoDB integration with proper data modeling

## 🎉 You're All Set!

Once you complete these steps, you'll have a fully functional Doctor Appointment Management System API running on your machine. The system includes 40+ API endpoints and supports the complete workflow from user registration to appointment booking and management.

Happy coding! 🚀

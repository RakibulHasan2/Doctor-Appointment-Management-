# ✅ Pre-Setup Checklist

Before running this Doctor Appointment Management System API, make sure you have completed the following:

## Required Software

- [ ] **.NET 8.0 SDK** installed
  - Download: https://dotnet.microsoft.com/en-us/download/dotnet/8.0
  - Verify: Run `dotnet --version` in terminal (should show 8.0.xxx)

- [ ] **MongoDB** setup (choose one):
  - [ ] **Option A**: MongoDB Community Server installed locally
    - Download: https://www.mongodb.com/try/download/community
    - Default connection: `mongodb://localhost:27017`
  - [ ] **Option B**: MongoDB Atlas account created (cloud)
    - Sign up: https://cloud.mongodb.com/
    - Create cluster and get connection string

- [ ] **Visual Studio Code** (recommended) or Visual Studio 2022
  - Download: https://code.visualstudio.com/

- [ ] **Git** for version control
  - Download: https://git-scm.com/downloads

## VS Code Extensions (Recommended)

- [ ] C# for Visual Studio Code (Microsoft)
- [ ] MongoDB for VS Code (MongoDB)
- [ ] REST Client (Huachao Mao) - for testing APIs
- [ ] GitLens (GitKraken) - for Git features

## Configuration Steps

- [ ] Clone the repository
- [ ] Update `appsettings.json` with your MongoDB connection string
- [ ] Run `dotnet restore` to install packages
- [ ] Run `dotnet build` to verify everything compiles

## Quick Start Options

### Option 1: Automated Setup (Recommended)
**Windows Users:**
```bash
# Double-click start.bat or run in command prompt:
start.bat
```

**Linux/Mac Users:**
```bash
# Make executable and run:
chmod +x start.sh
./start.sh
```

### Option 2: Manual Setup
```bash
# Clone repository
git clone https://github.com/RakibulHasan2/Doctor-Appointment-Management-.git
cd Doctor-Appointment-Management-/backend/DoctorAppointmentAPI

# Install dependencies
dotnet restore

# Build project
dotnet build

# Run application
dotnet run
```

## Verification

After setup, verify the API is working:

- [ ] API server starts without errors
- [ ] Swagger UI accessible at: http://localhost:5289/swagger
- [ ] Can create a specialty: `POST /api/specialties`
- [ ] Can register a user: `POST /api/users/register`

## Database Connection Examples

**Local MongoDB:**
```json
{
  "ConnectionStrings": {
    "MongoDB": "mongodb://localhost:27017/DoctorAppointmentDB"
  }
}
```

**MongoDB Atlas:**
```json
{
  "ConnectionStrings": {
    "MongoDB": "mongodb+srv://username:password@cluster.mongodb.net/DoctorAppointmentDB"
  }
}
```

## Test Data (Optional)

After setup, you can create test data:

1. **Create Specialty**: Cardiology, Neurology, etc.
2. **Register Admin User**: To manage doctor approvals
3. **Register Doctor User**: To create doctor profiles
4. **Register Patient User**: To book appointments

## Need Help?

- 📖 Read the full setup guide: `DEVELOPER_SETUP.md`
- 📚 Check API documentation: `Documentation/API_Documentation.md`
- 🔧 Database setup details: `Documentation/Database_Setup.md`
- 🧪 Test scenarios: `Documentation/Test_Scenarios.md`

## Project Features

This API provides:
- ✅ User management (Patient, Doctor, Admin roles)
- ✅ Doctor profile creation and approval workflow
- ✅ Doctor rejection functionality
- ✅ Appointment booking and management
- ✅ Medical specialty management
- ✅ 40+ REST API endpoints
- ✅ MongoDB integration
- ✅ Swagger documentation
- ✅ Input validation and error handling

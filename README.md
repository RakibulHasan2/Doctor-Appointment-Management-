# Doctor Appointment Management System API

## 🏥 Overview

A comprehensive RESTful API for managing doctor appointments, built with ASP.NET Core 8.0 and MongoDB. This system provides complete functionality for patients to book appointments, doctors to manage their schedules, and administrators to oversee the entire system.

## ✨ Features

### 👥 User Management
- Patient, Doctor, and Admin role management
- Secure password hashing with BCrypt
- User profile management and updates
- Account activation/deactivation

### 🩺 Doctor Management
- Doctor profile creation and management
- Medical specialty assignment
- Doctor approval workflow for administrators
- Availability schedule management
- License validation and tracking

### 📅 Appointment System
- Real-time appointment booking
- Appointment status management (Pending, Approved, Rejected, Cancelled, Completed)
- Time slot availability checking
- Appointment search and filtering
- Conflict resolution and validation

### 🏷️ Specialty Management
- Medical specialty categorization
- Dynamic specialty creation and management
- Doctor-specialty associations

## 🛠️ Technical Stack

- **Framework**: ASP.NET Core 8.0
- **Database**: MongoDB
- **Documentation**: Swagger/OpenAPI 3.0
- **Mapping**: AutoMapper
- **Security**: BCrypt.Net for password hashing
- **CORS**: Configured for cross-origin requests

## 📁 Project Structure

```
DoctorAppointmentAPI/
├── Controllers/                 # API endpoint controllers
│   ├── UsersController.cs      # User management endpoints
│   ├── DoctorsController.cs    # Doctor management endpoints
│   ├── AppointmentsController.cs # Appointment management endpoints
│   └── SpecialtiesController.cs # Specialty management endpoints
├── Services/                   # Business logic layer
│   ├── MongoDbService.cs      # Database connection service
│   ├── DoctorService.cs       # Doctor business logic
│   ├── AppointmentService.cs  # Appointment business logic
│   └── SpecialtyService.cs    # Specialty business logic
├── Models/                     # Data models
│   ├── User.cs                # User entity model
│   ├── Doctor.cs              # Doctor entity model
│   ├── Appointment.cs         # Appointment entity model
│   └── Specialty.cs           # Specialty entity model
├── DTOs/                       # Data Transfer Objects
│   ├── AuthDTOs.cs            # Authentication related DTOs
│   ├── DoctorDTOs.cs          # Doctor related DTOs
│   └── AppointmentDTOs.cs     # Appointment related DTOs
├── Configuration/              # Configuration classes
│   ├── AppSettings.cs         # Application settings
│   └── MappingProfile.cs      # AutoMapper configuration
└── Documentation/              # Project documentation
    ├── API_Documentation.md   # Complete API reference
    ├── Database_Setup.md      # Database setup guide
    ├── Postman_Collection.json # Postman API collection
    ├── Test_Scenarios.md      # Comprehensive test scenarios
    └── Deployment_Guide.md    # Deployment instructions
```

## 🚀 Quick Start

### Prerequisites
- .NET 8.0 SDK
- MongoDB (local or cloud instance)
- Visual Studio 2022 or VS Code

### Installation

1. **Clone the repository**
   ```bash
   git clone <repository-url>
   cd DoctorAppointmentAPI
   ```

2. **Install dependencies**
   ```bash
   dotnet restore
   ```

3. **Configure MongoDB connection**
   Update `appsettings.json` with your MongoDB connection string:
   ```json
   {
     "ConnectionStrings": {
       "MongoDB": "mongodb://localhost:27017/DoctorAppointmentDB"
     }
   }
   ```

4. **Run the application**
   ```bash
   dotnet run
   ```

5. **Access the API**
   - Swagger Documentation: https://localhost:7001/swagger
   - API Base URL: https://localhost:7001/api

### Database Setup

The application automatically creates the necessary MongoDB collections. For sample data and detailed setup instructions, see [Database_Setup.md](Documentation/Database_Setup.md).

## 📚 Documentation

### Complete Documentation Suite

| Document | Description |
|----------|-------------|
| **[API Documentation](Documentation/API_Documentation.md)** | Complete API reference with all endpoints, parameters, and examples |
| **[Database Setup](Documentation/Database_Setup.md)** | MongoDB configuration, sample data, and performance optimization |
| **[Postman Collection](Documentation/Postman_Collection.json)** | Ready-to-use Postman collection with all API endpoints |
| **[Test Scenarios](Documentation/Test_Scenarios.md)** | Comprehensive testing guide with 100+ test cases |
| **[Deployment Guide](Documentation/Deployment_Guide.md)** | Production deployment instructions for various environments |

### API Endpoints Overview

#### 🔐 User Management
```
POST   /api/users/register          # Register new user
POST   /api/users/login             # User login
GET    /api/users                   # Get all users
GET    /api/users/{id}              # Get user by ID
PUT    /api/users/{id}              # Update user
PATCH  /api/users/{id}/change-password # Change password
DELETE /api/users/{id}              # Delete user
```

#### 👨‍⚕️ Doctor Management
```
POST   /api/doctors                 # Create doctor profile
GET    /api/doctors                 # Get all doctors
GET    /api/doctors/{id}            # Get doctor by ID
PUT    /api/doctors/{id}            # Update doctor profile
DELETE /api/doctors/{id}            # Delete doctor
PATCH  /api/doctors/{id}/approve    # Approve doctor
GET    /api/doctors/pending-approval # Get pending approvals
GET    /api/doctors/specialty/{specialtyId} # Get doctors by specialty
```

#### 📅 Appointment Management
```
POST   /api/appointments            # Book appointment
GET    /api/appointments            # Get all appointments
GET    /api/appointments/{id}       # Get appointment by ID
PUT    /api/appointments/{id}       # Update appointment
DELETE /api/appointments/{id}       # Delete appointment
PATCH  /api/appointments/{id}/status # Update appointment status
PATCH  /api/appointments/{id}/cancel # Cancel appointment
GET    /api/appointments/patient/{patientId} # Get patient appointments
GET    /api/appointments/doctor/{doctorId}   # Get doctor appointments
```

#### 🏷️ Specialty Management
```
POST   /api/specialties             # Create specialty
GET    /api/specialties             # Get all specialties
GET    /api/specialties/{id}        # Get specialty by ID
PUT    /api/specialties/{id}        # Update specialty
DELETE /api/specialties/{id}        # Delete specialty
```

## 🧪 Testing

### Using Postman
1. Import the [Postman Collection](Documentation/Postman_Collection.json)
2. Set up environment variables:
   - `baseUrl`: https://localhost:7001
   - `userId`: (set after registration/login)
   - `doctorId`: (set after doctor profile creation)

### Manual Testing
Follow the comprehensive test scenarios in [Test_Scenarios.md](Documentation/Test_Scenarios.md) which includes:
- 100+ test cases
- Integration testing workflows
- Performance testing guidelines
- Security testing scenarios

### Sample Test Flow
1. **Register a patient**: `POST /api/users/register`
2. **Register a doctor**: `POST /api/users/register` (role: "Doctor")
3. **Create doctor profile**: `POST /api/doctors`
4. **Approve doctor**: `PATCH /api/doctors/{id}/approve`
5. **Book appointment**: `POST /api/appointments`
6. **Approve appointment**: `PATCH /api/appointments/{id}/status`

## 🚀 Deployment

### Development Environment
```bash
dotnet run --environment Development
```

### Production Deployment
Comprehensive deployment guides are available for:
- **Windows Server** with IIS
- **Linux Server** with Nginx/Apache
- **Docker** containerization
- **Azure App Service**
- **AWS Elastic Beanstalk**

See [Deployment_Guide.md](Documentation/Deployment_Guide.md) for detailed instructions.

### Docker Deployment
```bash
# Build and run with Docker Compose
docker-compose up --build -d

# The API will be available at:
# HTTP: http://localhost:8080
# HTTPS: https://localhost:8443
```

## 🔧 Configuration

### Application Settings
```json
{
  "ConnectionStrings": {
    "MongoDB": "mongodb://localhost:27017/DoctorAppointmentDB"
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

### Environment Variables
```bash
export ASPNETCORE_ENVIRONMENT=Production
export ConnectionStrings__MongoDB="mongodb://server:27017/DoctorAppointmentDB"
```

## 🎯 Key Features Explained

### 1. Role-Based System
- **Patients**: Book appointments, view their appointment history
- **Doctors**: Manage profiles, set availability, handle appointments
- **Admins**: Approve doctors, manage system-wide settings

### 2. Appointment Workflow
```
Patient Request → Doctor Approval → Appointment Confirmed → Completion/Cancellation
```

### 3. Doctor Approval Process
```
Doctor Registration → Profile Creation → Admin Approval → Active Doctor
```

### 4. Time Slot Management
- Prevents double-booking
- Validates doctor availability
- Handles appointment conflicts

## 🔐 Security Features

- **Password Security**: BCrypt hashing with salt
- **Input Validation**: Comprehensive DTO validation
- **CORS Protection**: Configured for specific origins
- **SQL Injection Prevention**: MongoDB native protection
- **Error Handling**: Secure error responses without sensitive data exposure

## 📊 Performance Considerations

### Database Optimization
- Indexed collections for fast queries
- Connection pooling
- Efficient aggregation pipelines

### Caching Strategy
- In-memory caching for frequently accessed data
- Specialty data caching
- Doctor availability caching

### Scalability
- Stateless API design
- Horizontal scaling ready
- Load balancer compatible

## 🤝 API Integration Examples

### Registration Example
```bash
curl -X POST "https://localhost:7001/api/users/register" \
  -H "Content-Type: application/json" \
  -d '{
    "name": "John Doe",
    "email": "john.doe@example.com",
    "password": "SecurePassword123",
    "confirmPassword": "SecurePassword123",
    "role": "Patient",
    "phone": "+1234567890"
  }'
```

### Appointment Booking Example
```bash
curl -X POST "https://localhost:7001/api/appointments?patientId=patient123" \
  -H "Content-Type: application/json" \
  -d '{
    "doctorId": "doctor456",
    "appointmentDate": "2025-09-15T00:00:00Z",
    "startTime": "10:00:00",
    "endTime": "10:30:00",
    "reasonForVisit": "Regular checkup"
  }'
```

## 🐛 Troubleshooting

### Common Issues

1. **MongoDB Connection Failed**
   - Verify MongoDB is running
   - Check connection string in appsettings.json
   - Ensure network connectivity

2. **Port Already in Use**
   ```bash
   netstat -tulpn | grep :7001
   # Kill process if needed
   ```

3. **CORS Issues**
   - Verify allowed origins in Program.cs
   - Check request headers

### Debug Mode
```bash
dotnet run --environment Development --verbosity detailed
```

## 📈 Monitoring and Logging

### Application Logs
Logs are configured to output to:
- Console (Development)
- File system (Production)
- Application Insights (Azure)

### Health Checks
- Database connectivity
- Application status
- Performance metrics

## 🔄 Version History

### v1.0.0 (Current)
- Complete user management system
- Doctor registration and approval workflow
- Comprehensive appointment booking system
- Specialty management
- Full API documentation
- Production-ready deployment guides

## 🤝 Contributing

1. Fork the repository
2. Create a feature branch
3. Make your changes
4. Add tests for new functionality
5. Submit a pull request

## 📄 License

This project is licensed under the MIT License - see the LICENSE file for details.

## 📞 Support

For support and questions:
- Create an issue in the repository
- Check the [API Documentation](Documentation/API_Documentation.md)
- Review [Test Scenarios](Documentation/Test_Scenarios.md)
- Follow the [Deployment Guide](Documentation/Deployment_Guide.md)

## 🎯 Roadmap

### Future Enhancements
- [ ] Real-time notifications
- [ ] Payment integration
- [ ] Mobile app support
- [ ] Advanced reporting
- [ ] Multi-language support
- [ ] Telemedicine features

---

**🏥 Ready to revolutionize healthcare appointment management!**

Start with the [Quick Start](#-quick-start) guide and explore the comprehensive documentation for a complete understanding of the system.

namespace DoctorAppointmentAPI.Configuration
{
    public class MongoDbSettings
    {
        public string ConnectionString { get; set; } = string.Empty;
        public string DatabaseName { get; set; } = string.Empty;
        public string UsersCollectionName { get; set; } = "Users";
        public string DoctorsCollectionName { get; set; } = "Doctors";
        public string SpecialtiesCollectionName { get; set; } = "Specialties";
        public string AppointmentsCollectionName { get; set; } = "Appointments";
    }

    public class ApiSettings
    {
        public string Version { get; set; } = "v1";
        public string Title { get; set; } = "Doctor Appointment Management API";
        public string Description { get; set; } = "API for managing doctor appointments";
        public bool EnableSwagger { get; set; } = true;
        public string AllowedOrigins { get; set; } = string.Empty;
    }
}

using DoctorAppointmentAPI.Configuration;
using DoctorAppointmentAPI.Models;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace DoctorAppointmentAPI.Services
{
    public interface IMongoDbService
    {
        IMongoCollection<User> Users { get; }
        IMongoCollection<Doctor> Doctors { get; }
        IMongoCollection<Specialty> Specialties { get; }
        IMongoCollection<Appointment> Appointments { get; }
    }

    public class MongoDbService : IMongoDbService
    {
        private readonly IMongoDatabase _database;

        public MongoDbService(IOptions<MongoDbSettings> mongoDbSettings)
        {
            var client = new MongoClient(mongoDbSettings.Value.ConnectionString);
            _database = client.GetDatabase(mongoDbSettings.Value.DatabaseName);
        }

        public IMongoCollection<User> Users => 
            _database.GetCollection<User>("Users");

        public IMongoCollection<Doctor> Doctors => 
            _database.GetCollection<Doctor>("Doctors");

        public IMongoCollection<Specialty> Specialties => 
            _database.GetCollection<Specialty>("Specialties");

        public IMongoCollection<Appointment> Appointments => 
            _database.GetCollection<Appointment>("Appointments");
    }
}

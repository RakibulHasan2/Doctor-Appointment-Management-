using AutoMapper;
using DoctorAppointmentAPI.DTOs;
using DoctorAppointmentAPI.Models;
using MongoDB.Driver;

namespace DoctorAppointmentAPI.Services
{
    public interface IDoctorService
    {
        Task<IEnumerable<DoctorDto>> GetAllDoctorsAsync();
        Task<DoctorDto?> GetDoctorByIdAsync(string id);
        Task<DoctorDto?> GetDoctorByUserIdAsync(string userId);
        Task<IEnumerable<DoctorDto>> GetDoctorsBySpecialtyAsync(string specialtyId);
        Task<DoctorDto> CreateDoctorAsync(CreateDoctorDto createDoctorDto);
        Task<DoctorDto?> UpdateDoctorAsync(string id, UpdateDoctorDto updateDoctorDto);
        Task<bool> DeleteDoctorAsync(string id);
        Task<bool> ApproveDoctorAsync(string id);
        Task<bool> RejectDoctorAsync(string id, string reason);
        Task<bool> UpdateAvailabilityAsync(string id, List<DoctorAvailabilityDto> availability);
        Task<IEnumerable<AvailableSlotDto>> GetAvailableSlotsAsync(string doctorId, DateTime date);
    }

    public class DoctorService : IDoctorService
    {
        private readonly IMongoDbService _mongoDbService;
        private readonly IMapper _mapper;
        private readonly ILogger<DoctorService> _logger;

        public DoctorService(
            IMongoDbService mongoDbService,
            IMapper mapper,
            ILogger<DoctorService> logger)
        {
            _mongoDbService = mongoDbService;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<IEnumerable<DoctorDto>> GetAllDoctorsAsync()
        {
            var doctors = await _mongoDbService.Doctors
                .Find(d => d.IsActive)
                .ToListAsync();

            var doctorDtos = new List<DoctorDto>();

            foreach (var doctor in doctors)
            {
                var doctorDto = _mapper.Map<DoctorDto>(doctor);
                
                // Load user and specialty data
                var user = await _mongoDbService.Users
                    .Find(u => u.Id == doctor.UserId)
                    .FirstOrDefaultAsync();
                
                var specialty = await _mongoDbService.Specialties
                    .Find(s => s.Id == doctor.SpecialtyId)
                    .FirstOrDefaultAsync();

                if (user != null)
                    doctorDto.User = _mapper.Map<UserDto>(user);
                
                if (specialty != null)
                    doctorDto.Specialty = _mapper.Map<SpecialtyDto>(specialty);

                doctorDtos.Add(doctorDto);
            }

            return doctorDtos;
        }

        public async Task<DoctorDto?> GetDoctorByIdAsync(string id)
        {
            var doctor = await _mongoDbService.Doctors
                .Find(d => d.Id == id && d.IsActive)
                .FirstOrDefaultAsync();

            if (doctor == null)
                return null;

            var doctorDto = _mapper.Map<DoctorDto>(doctor);

            // Load user and specialty data
            var user = await _mongoDbService.Users
                .Find(u => u.Id == doctor.UserId)
                .FirstOrDefaultAsync();
            
            var specialty = await _mongoDbService.Specialties
                .Find(s => s.Id == doctor.SpecialtyId)
                .FirstOrDefaultAsync();

            if (user != null)
                doctorDto.User = _mapper.Map<UserDto>(user);
            
            if (specialty != null)
                doctorDto.Specialty = _mapper.Map<SpecialtyDto>(specialty);

            return doctorDto;
        }

        public async Task<DoctorDto?> GetDoctorByUserIdAsync(string userId)
        {
            var doctor = await _mongoDbService.Doctors
                .Find(d => d.UserId == userId && d.IsActive)
                .FirstOrDefaultAsync();

            if (doctor == null)
                return null;

            return await GetDoctorByIdAsync(doctor.Id!);
        }

        public async Task<IEnumerable<DoctorDto>> GetDoctorsBySpecialtyAsync(string specialtyId)
        {
            var doctors = await _mongoDbService.Doctors
                .Find(d => d.SpecialtyId == specialtyId && d.IsActive && d.IsApproved)
                .ToListAsync();

            var doctorDtos = new List<DoctorDto>();

            foreach (var doctor in doctors)
            {
                var doctorDto = await GetDoctorByIdAsync(doctor.Id!);
                if (doctorDto != null)
                    doctorDtos.Add(doctorDto);
            }

            return doctorDtos;
        }

        public async Task<DoctorDto> CreateDoctorAsync(CreateDoctorDto createDoctorDto)
        {
            // Verify user exists and is a doctor
            var user = await _mongoDbService.Users
                .Find(u => u.Id == createDoctorDto.UserId)
                .FirstOrDefaultAsync();

            if (user == null || user.Role != UserRole.Doctor)
                throw new ArgumentException("User not found or not a doctor");

            // Verify specialty exists
            var specialty = await _mongoDbService.Specialties
                .Find(s => s.Id == createDoctorDto.SpecialtyId && s.IsActive)
                .FirstOrDefaultAsync();

            if (specialty == null)
                throw new ArgumentException("Specialty not found");

            // Check if doctor already exists for this user
            var existingDoctor = await _mongoDbService.Doctors
                .Find(d => d.UserId == createDoctorDto.UserId)
                .FirstOrDefaultAsync();

            if (existingDoctor != null)
                throw new ArgumentException("Doctor profile already exists for this user");

            var doctor = _mapper.Map<Doctor>(createDoctorDto);
            doctor.CreatedAt = DateTime.UtcNow;
            doctor.UpdatedAt = DateTime.UtcNow;

            await _mongoDbService.Doctors.InsertOneAsync(doctor);

            return await GetDoctorByIdAsync(doctor.Id!) ?? 
                   throw new Exception("Failed to retrieve created doctor");
        }

        public async Task<DoctorDto?> UpdateDoctorAsync(string id, UpdateDoctorDto updateDoctorDto)
        {
            var doctor = await _mongoDbService.Doctors
                .Find(d => d.Id == id)
                .FirstOrDefaultAsync();

            if (doctor == null)
                return null;

            if (updateDoctorDto.SpecialtyId != null)
            {
                var specialty = await _mongoDbService.Specialties
                    .Find(s => s.Id == updateDoctorDto.SpecialtyId && s.IsActive)
                    .FirstOrDefaultAsync();

                if (specialty == null)
                    throw new ArgumentException("Specialty not found");

                doctor.SpecialtyId = updateDoctorDto.SpecialtyId;
            }

            if (updateDoctorDto.LicenseNumber != null)
                doctor.LicenseNumber = updateDoctorDto.LicenseNumber;

            if (updateDoctorDto.Experience.HasValue)
                doctor.Experience = updateDoctorDto.Experience.Value;

            if (updateDoctorDto.Qualification != null)
                doctor.Qualification = updateDoctorDto.Qualification;

            if (updateDoctorDto.ConsultationFee.HasValue)
                doctor.ConsultationFee = updateDoctorDto.ConsultationFee.Value;

            if (updateDoctorDto.Availability != null)
                doctor.Availability = _mapper.Map<List<DoctorAvailability>>(updateDoctorDto.Availability);

            doctor.UpdatedAt = DateTime.UtcNow;

            await _mongoDbService.Doctors.ReplaceOneAsync(d => d.Id == id, doctor);

            return await GetDoctorByIdAsync(id);
        }

        public async Task<bool> DeleteDoctorAsync(string id)
        {
            var doctor = await _mongoDbService.Doctors
                .Find(d => d.Id == id)
                .FirstOrDefaultAsync();

            if (doctor == null)
                return false;

            doctor.IsActive = false;
            doctor.UpdatedAt = DateTime.UtcNow;

            await _mongoDbService.Doctors.ReplaceOneAsync(d => d.Id == id, doctor);

            return true;
        }

        public async Task<bool> ApproveDoctorAsync(string id)
        {
            var doctor = await _mongoDbService.Doctors
                .Find(d => d.Id == id)
                .FirstOrDefaultAsync();

            if (doctor == null)
                return false;

            doctor.IsApproved = true;
            doctor.IsRejected = false;
            doctor.RejectionReason = null;
            doctor.RejectedAt = null;
            doctor.UpdatedAt = DateTime.UtcNow;

            await _mongoDbService.Doctors.ReplaceOneAsync(d => d.Id == id, doctor);

            return true;
        }

        public async Task<bool> RejectDoctorAsync(string id, string reason)
        {
            var doctor = await _mongoDbService.Doctors
                .Find(d => d.Id == id)
                .FirstOrDefaultAsync();

            if (doctor == null)
                return false;

            doctor.IsApproved = false;
            doctor.IsRejected = true;
            doctor.RejectionReason = reason;
            doctor.RejectedAt = DateTime.UtcNow;
            doctor.UpdatedAt = DateTime.UtcNow;

            await _mongoDbService.Doctors.ReplaceOneAsync(d => d.Id == id, doctor);

            return true;
        }

        public async Task<bool> UpdateAvailabilityAsync(string id, List<DoctorAvailabilityDto> availability)
        {
            var doctor = await _mongoDbService.Doctors
                .Find(d => d.Id == id)
                .FirstOrDefaultAsync();

            if (doctor == null)
                return false;

            doctor.Availability = _mapper.Map<List<DoctorAvailability>>(availability);
            doctor.UpdatedAt = DateTime.UtcNow;

            await _mongoDbService.Doctors.ReplaceOneAsync(d => d.Id == id, doctor);

            return true;
        }

        public async Task<IEnumerable<AvailableSlotDto>> GetAvailableSlotsAsync(string doctorId, DateTime date)
        {
            var doctor = await _mongoDbService.Doctors
                .Find(d => d.Id == doctorId && d.IsActive && d.IsApproved)
                .FirstOrDefaultAsync();

            if (doctor == null)
                return new List<AvailableSlotDto>();

            var dayOfWeek = date.DayOfWeek;
            var availability = doctor.Availability.Where(a => a.DayOfWeek == dayOfWeek && a.IsAvailable);

            var bookedAppointments = await _mongoDbService.Appointments
                .Find(a => a.DoctorId == doctorId && 
                          a.AppointmentDate.Date == date.Date && 
                          (a.Status == AppointmentStatus.Approved || a.Status == AppointmentStatus.Pending))
                .ToListAsync();

            var availableSlots = new List<AvailableSlotDto>();

            foreach (var slot in availability)
            {
                var slotStart = slot.StartTime;
                var slotEnd = slot.EndTime;
                var duration = TimeSpan.FromMinutes(30); // 30-minute slots

                while (slotStart.Add(duration) <= slotEnd)
                {
                    var isBooked = bookedAppointments.Any(a => 
                        a.StartTime <= slotStart && a.EndTime > slotStart);

                    availableSlots.Add(new AvailableSlotDto
                    {
                        Date = date,
                        StartTime = slotStart,
                        EndTime = slotStart.Add(duration),
                        IsAvailable = !isBooked
                    });

                    slotStart = slotStart.Add(duration);
                }
            }

            return availableSlots.OrderBy(s => s.StartTime);
        }
    }
}

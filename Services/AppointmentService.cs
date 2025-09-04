using AutoMapper;
using DoctorAppointmentAPI.DTOs;
using DoctorAppointmentAPI.Models;
using MongoDB.Driver;

namespace DoctorAppointmentAPI.Services
{
    public interface IAppointmentService
    {
        Task<IEnumerable<AppointmentDto>> GetAppointmentsAsync(string? userId = null, UserRole? userRole = null);
        Task<AppointmentDto?> GetAppointmentByIdAsync(string id);
        Task<AppointmentDto> CreateAppointmentAsync(CreateAppointmentDto createAppointmentDto, string patientId);
        Task<AppointmentDto?> UpdateAppointmentAsync(string id, UpdateAppointmentDto updateAppointmentDto);
        Task<AppointmentDto?> UpdateAppointmentStatusAsync(string id, AppointmentStatusUpdateDto statusUpdateDto, string? userId = null);
        Task<bool> CancelAppointmentAsync(string id, string reason, string? userId = null);
        Task<IEnumerable<AppointmentDto>> SearchAppointmentsAsync(AppointmentSearchDto searchDto);
        Task<IEnumerable<AppointmentDto>> GetDoctorAppointmentsAsync(string doctorId);
    }

    public class AppointmentService : IAppointmentService
    {
        private readonly IMongoDbService _mongoDbService;
        private readonly IMapper _mapper;
        private readonly ILogger<AppointmentService> _logger;

        public AppointmentService(
            IMongoDbService mongoDbService,
            IMapper mapper,
            ILogger<AppointmentService> logger)
        {
            _mongoDbService = mongoDbService;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<IEnumerable<AppointmentDto>> GetAppointmentsAsync(string? userId = null, UserRole? userRole = null)
        {
            var filterBuilder = Builders<Appointment>.Filter;
            var filter = filterBuilder.Empty;

            // Apply role-based filtering
            if (userRole.HasValue && !string.IsNullOrEmpty(userId))
            {
                switch (userRole.Value)
                {
                    case UserRole.Patient:
                        filter = filterBuilder.Eq(a => a.PatientId, userId);
                        break;
                    case UserRole.Doctor:
                        // Get doctor ID from user ID
                        var doctor = await _mongoDbService.Doctors
                            .Find(d => d.UserId == userId)
                            .FirstOrDefaultAsync();
                        if (doctor != null)
                            filter = filterBuilder.Eq(a => a.DoctorId, doctor.Id);
                        break;
                    case UserRole.Admin:
                        // Admins can see all appointments
                        break;
                }
            }

            var appointments = await _mongoDbService.Appointments
                .Find(filter)
                .SortByDescending(a => a.CreatedAt)
                .ToListAsync();

            return await PopulateAppointmentDetailsAsync(appointments);
        }

        public async Task<AppointmentDto?> GetAppointmentByIdAsync(string id)
        {
            var appointment = await _mongoDbService.Appointments
                .Find(a => a.Id == id)
                .FirstOrDefaultAsync();

            if (appointment == null)
                return null;

            var appointments = await PopulateAppointmentDetailsAsync(new[] { appointment });
            return appointments.FirstOrDefault();
        }

        public async Task<AppointmentDto> CreateAppointmentAsync(CreateAppointmentDto createAppointmentDto, string patientId)
        {
            // Validate doctor exists and is approved
            var doctor = await _mongoDbService.Doctors
                .Find(d => d.Id == createAppointmentDto.DoctorId && d.IsActive && d.IsApproved)
                .FirstOrDefaultAsync();

            if (doctor == null)
                throw new ArgumentException("Doctor not found or not approved");

            // Check for appointment conflicts
            var existingAppointment = await _mongoDbService.Appointments
                .Find(a => a.DoctorId == createAppointmentDto.DoctorId &&
                          a.AppointmentDate.Date == createAppointmentDto.AppointmentDate.Date &&
                          ((a.StartTime < createAppointmentDto.EndTime && a.EndTime > createAppointmentDto.StartTime)) &&
                          (a.Status == AppointmentStatus.Approved || a.Status == AppointmentStatus.Pending))
                .FirstOrDefaultAsync();

            if (existingAppointment != null)
                throw new ArgumentException("Time slot is already booked");

            // Check if the requested time is within doctor's availability
            var dayOfWeek = createAppointmentDto.AppointmentDate.DayOfWeek;
            var availability = doctor.Availability.FirstOrDefault(a => 
                a.DayOfWeek == dayOfWeek && 
                a.IsAvailable &&
                a.StartTime <= createAppointmentDto.StartTime &&
                a.EndTime >= createAppointmentDto.EndTime);

            if (availability == null)
                throw new ArgumentException("Doctor is not available at the requested time");

            var appointment = _mapper.Map<Appointment>(createAppointmentDto);
            appointment.PatientId = patientId;
            appointment.ConsultationFee = doctor.ConsultationFee;
            appointment.CreatedAt = DateTime.UtcNow;
            appointment.UpdatedAt = DateTime.UtcNow;

            await _mongoDbService.Appointments.InsertOneAsync(appointment);

            return await GetAppointmentByIdAsync(appointment.Id!) ?? 
                   throw new Exception("Failed to retrieve created appointment");
        }

        public async Task<AppointmentDto?> UpdateAppointmentAsync(string id, UpdateAppointmentDto updateAppointmentDto)
        {
            var appointment = await _mongoDbService.Appointments
                .Find(a => a.Id == id)
                .FirstOrDefaultAsync();

            if (appointment == null)
                return null;

            // Only allow updates if appointment is pending
            if (appointment.Status != AppointmentStatus.Pending)
                throw new InvalidOperationException("Can only update pending appointments");

            if (updateAppointmentDto.AppointmentDate.HasValue)
                appointment.AppointmentDate = updateAppointmentDto.AppointmentDate.Value;

            if (updateAppointmentDto.StartTime.HasValue)
                appointment.StartTime = updateAppointmentDto.StartTime.Value;

            if (updateAppointmentDto.EndTime.HasValue)
                appointment.EndTime = updateAppointmentDto.EndTime.Value;

            if (updateAppointmentDto.ReasonForVisit != null)
                appointment.ReasonForVisit = updateAppointmentDto.ReasonForVisit;

            if (updateAppointmentDto.Notes != null)
                appointment.Notes = updateAppointmentDto.Notes;

            appointment.UpdatedAt = DateTime.UtcNow;

            await _mongoDbService.Appointments.ReplaceOneAsync(a => a.Id == id, appointment);

            return await GetAppointmentByIdAsync(id);
        }

        public async Task<AppointmentDto?> UpdateAppointmentStatusAsync(string id, AppointmentStatusUpdateDto statusUpdateDto, string? userId = null)
        {
            var appointment = await _mongoDbService.Appointments
                .Find(a => a.Id == id)
                .FirstOrDefaultAsync();

            if (appointment == null)
                return null;

            appointment.Status = statusUpdateDto.Status;
            appointment.UpdatedAt = DateTime.UtcNow;

            if (!string.IsNullOrEmpty(statusUpdateDto.Notes))
                appointment.Notes = statusUpdateDto.Notes;

            if (statusUpdateDto.Status == AppointmentStatus.Approved && userId != null)
            {
                appointment.ApprovedAt = DateTime.UtcNow;
                appointment.ApprovedBy = userId;
            }
            else if (statusUpdateDto.Status == AppointmentStatus.Cancelled)
            {
                appointment.CancelledAt = DateTime.UtcNow;
                appointment.CancellationReason = statusUpdateDto.CancellationReason;
            }

            await _mongoDbService.Appointments.ReplaceOneAsync(a => a.Id == id, appointment);

            return await GetAppointmentByIdAsync(id);
        }

        public async Task<bool> CancelAppointmentAsync(string id, string reason, string? userId = null)
        {
            var appointment = await _mongoDbService.Appointments
                .Find(a => a.Id == id)
                .FirstOrDefaultAsync();

            if (appointment == null)
                return false;

            // Check if user has permission to cancel this appointment
            if (userId != null)
            {
                var user = await _mongoDbService.Users
                    .Find(u => u.Id == userId)
                    .FirstOrDefaultAsync();

                if (user == null)
                    return false;

                if (user.Role == UserRole.Patient && appointment.PatientId != userId)
                    throw new UnauthorizedAccessException("You can only cancel your own appointments");

                if (user.Role == UserRole.Doctor)
                {
                    var doctor = await _mongoDbService.Doctors
                        .Find(d => d.UserId == userId)
                        .FirstOrDefaultAsync();

                    if (doctor == null || appointment.DoctorId != doctor.Id)
                        throw new UnauthorizedAccessException("You can only cancel your own appointments");
                }
            }

            appointment.Status = AppointmentStatus.Cancelled;
            appointment.CancelledAt = DateTime.UtcNow;
            appointment.CancellationReason = reason;
            appointment.UpdatedAt = DateTime.UtcNow;

            await _mongoDbService.Appointments.ReplaceOneAsync(a => a.Id == id, appointment);

            return true;
        }

        public async Task<IEnumerable<AppointmentDto>> SearchAppointmentsAsync(AppointmentSearchDto searchDto)
        {
            var filterBuilder = Builders<Appointment>.Filter;
            var filters = new List<FilterDefinition<Appointment>>();

            if (!string.IsNullOrEmpty(searchDto.PatientId))
                filters.Add(filterBuilder.Eq(a => a.PatientId, searchDto.PatientId));

            if (!string.IsNullOrEmpty(searchDto.DoctorId))
                filters.Add(filterBuilder.Eq(a => a.DoctorId, searchDto.DoctorId));

            if (searchDto.FromDate.HasValue)
                filters.Add(filterBuilder.Gte(a => a.AppointmentDate, searchDto.FromDate.Value));

            if (searchDto.ToDate.HasValue)
                filters.Add(filterBuilder.Lte(a => a.AppointmentDate, searchDto.ToDate.Value));

            if (searchDto.Status.HasValue)
                filters.Add(filterBuilder.Eq(a => a.Status, searchDto.Status.Value));

            var filter = filters.Count > 0 ? filterBuilder.And(filters) : filterBuilder.Empty;

            var appointments = await _mongoDbService.Appointments
                .Find(filter)
                .SortByDescending(a => a.CreatedAt)
                .Skip((searchDto.Page - 1) * searchDto.PageSize)
                .Limit(searchDto.PageSize)
                .ToListAsync();

            return await PopulateAppointmentDetailsAsync(appointments);
        }

        private async Task<IEnumerable<AppointmentDto>> PopulateAppointmentDetailsAsync(IEnumerable<Appointment> appointments)
        {
            var appointmentDtos = new List<AppointmentDto>();

            foreach (var appointment in appointments)
            {
                var appointmentDto = _mapper.Map<AppointmentDto>(appointment);

                // Load patient data
                var patient = await _mongoDbService.Users
                    .Find(u => u.Id == appointment.PatientId)
                    .FirstOrDefaultAsync();

                // Load doctor data
                var doctor = await _mongoDbService.Doctors
                    .Find(d => d.Id == appointment.DoctorId)
                    .FirstOrDefaultAsync();

                if (patient != null)
                    appointmentDto.Patient = _mapper.Map<UserDto>(patient);

                if (doctor != null)
                {
                    appointmentDto.Doctor = _mapper.Map<DoctorDto>(doctor);

                    // Load doctor's user data
                    var doctorUser = await _mongoDbService.Users
                        .Find(u => u.Id == doctor.UserId)
                        .FirstOrDefaultAsync();

                    if (doctorUser != null)
                        appointmentDto.Doctor.User = _mapper.Map<UserDto>(doctorUser);

                    // Load specialty data
                    var specialty = await _mongoDbService.Specialties
                        .Find(s => s.Id == doctor.SpecialtyId)
                        .FirstOrDefaultAsync();

                    if (specialty != null)
                        appointmentDto.Doctor.Specialty = _mapper.Map<SpecialtyDto>(specialty);
                }

                appointmentDtos.Add(appointmentDto);
            }

            return appointmentDtos;
        }

        public async Task<IEnumerable<AppointmentDto>> GetDoctorAppointmentsAsync(string doctorId)
        {
            var appointments = await _mongoDbService.Appointments
                .Find(a => a.DoctorId == doctorId)
                .ToListAsync();

            var appointmentDtos = new List<AppointmentDto>();

            foreach (var appointment in appointments)
            {
                var appointmentDto = _mapper.Map<AppointmentDto>(appointment);

                // Load patient data
                var patient = await _mongoDbService.Users
                    .Find(u => u.Id == appointment.PatientId)
                    .FirstOrDefaultAsync();

                if (patient != null)
                    appointmentDto.Patient = _mapper.Map<UserDto>(patient);

                // Load doctor data
                var doctor = await _mongoDbService.Doctors
                    .Find(d => d.Id == appointment.DoctorId)
                    .FirstOrDefaultAsync();

                if (doctor != null)
                {
                    appointmentDto.Doctor = _mapper.Map<DoctorDto>(doctor);

                    // Load doctor user data
                    var doctorUser = await _mongoDbService.Users
                        .Find(u => u.Id == doctor.UserId)
                        .FirstOrDefaultAsync();

                    if (doctorUser != null)
                        appointmentDto.Doctor.User = _mapper.Map<UserDto>(doctorUser);

                    // Load specialty data
                    var specialty = await _mongoDbService.Specialties
                        .Find(s => s.Id == doctor.SpecialtyId)
                        .FirstOrDefaultAsync();

                    if (specialty != null)
                        appointmentDto.Doctor.Specialty = _mapper.Map<SpecialtyDto>(specialty);
                }

                appointmentDtos.Add(appointmentDto);
            }

            return appointmentDtos;
        }
    }
}

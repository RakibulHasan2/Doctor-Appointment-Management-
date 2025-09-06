using DoctorAppointmentAPI.Models;
using System.Text.Json.Serialization;

namespace DoctorAppointmentAPI.DTOs
{
    // Doctor DTOs
    public class DoctorDto
    {
        public string? Id { get; set; }
        public string? UserId { get; set; }
        public string? SpecialtyId { get; set; }
        public required string LicenseNumber { get; set; }
        public int Experience { get; set; }
        public string? Qualification { get; set; }
        public decimal ConsultationFee { get; set; }
        public List<DoctorAvailabilityDto> Availability { get; set; } = new();
        public bool IsApproved { get; set; }
        public bool IsRejected { get; set; }
        public string? RejectionReason { get; set; }
        public DateTime? RejectedAt { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }

        // Navigation properties
        public UserDto? User { get; set; }
        public SpecialtyDto? Specialty { get; set; }
    }

    public class CreateDoctorDto
    {
        public required string UserId { get; set; }
        public required string SpecialtyId { get; set; }
        public required string LicenseNumber { get; set; }
        public int Experience { get; set; }
        public string? Qualification { get; set; }
        public decimal ConsultationFee { get; set; }
        public List<DoctorAvailabilityDto> Availability { get; set; } = new();
    }

    public class UpdateDoctorDto
    {
        public string? SpecialtyId { get; set; }
        public string? LicenseNumber { get; set; }
        public int? Experience { get; set; }
        public string? Qualification { get; set; }
        public decimal? ConsultationFee { get; set; }
        public List<DoctorAvailabilityDto>? Availability { get; set; }
    }

    public class DoctorAvailabilityDto
    {
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public DayOfWeek DayOfWeek { get; set; }
        public TimeSpan StartTime { get; set; }
        public TimeSpan EndTime { get; set; }
        public bool IsAvailable { get; set; } = true;
    }

    // Specialty DTOs
    public class SpecialtyDto
    {
        public string? Id { get; set; }
        public required string Name { get; set; }
        public string? Description { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    public class CreateSpecialtyDto
    {
        public required string Name { get; set; }
        public string? Description { get; set; }
    }

    public class UpdateSpecialtyDto
    {
        public string? Name { get; set; }
        public string? Description { get; set; }
        public bool? IsActive { get; set; }
    }
}

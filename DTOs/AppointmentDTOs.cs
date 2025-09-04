using DoctorAppointmentAPI.Models;

namespace DoctorAppointmentAPI.DTOs
{
    // Appointment DTOs
    public class AppointmentDto
    {
        public string? Id { get; set; }
        public required string PatientId { get; set; }
        public required string DoctorId { get; set; }
        public DateTime AppointmentDate { get; set; }
        public TimeSpan StartTime { get; set; }
        public TimeSpan EndTime { get; set; }
        public AppointmentStatus Status { get; set; }
        public string? ReasonForVisit { get; set; }
        public string? Notes { get; set; }
        public decimal ConsultationFee { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? CancelledAt { get; set; }
        public string? CancellationReason { get; set; }
        public DateTime? ApprovedAt { get; set; }
        public string? ApprovedBy { get; set; }

        // Navigation properties
        public UserDto? Patient { get; set; }
        public DoctorDto? Doctor { get; set; }
    }

    public class CreateAppointmentDto
    {
        public required string DoctorId { get; set; }
        public DateTime AppointmentDate { get; set; }
        public TimeSpan StartTime { get; set; }
        public TimeSpan EndTime { get; set; }
        public string? ReasonForVisit { get; set; }
    }

    public class UpdateAppointmentDto
    {
        public DateTime? AppointmentDate { get; set; }
        public TimeSpan? StartTime { get; set; }
        public TimeSpan? EndTime { get; set; }
        public string? ReasonForVisit { get; set; }
        public string? Notes { get; set; }
    }

    public class AppointmentStatusUpdateDto
    {
        public required AppointmentStatus Status { get; set; }
        public string? Notes { get; set; }
        public string? CancellationReason { get; set; }
    }

    public class AvailableSlotDto
    {
        public DateTime Date { get; set; }
        public TimeSpan StartTime { get; set; }
        public TimeSpan EndTime { get; set; }
        public bool IsAvailable { get; set; }
    }

    public class AppointmentSearchDto
    {
        public string? PatientId { get; set; }
        public string? DoctorId { get; set; }
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
        public AppointmentStatus? Status { get; set; }
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 10;
    }
}

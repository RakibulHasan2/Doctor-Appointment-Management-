using DoctorAppointmentAPI.Models;

namespace DoctorAppointmentAPI.DTOs
{
    public class ApiResponseDto<T>
    {
        public bool Success { get; set; }
        public string? Message { get; set; }
        public T? Data { get; set; }
        public string? ErrorCode { get; set; }
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;
    }

    public class PaginatedResponseDto<T>
    {
        public IEnumerable<T> Data { get; set; } = new List<T>();
        public int TotalCount { get; set; }
        public int Page { get; set; }
        public int PageSize { get; set; }
        public int TotalPages => (int)Math.Ceiling((double)TotalCount / PageSize);
        public bool HasNextPage => Page < TotalPages;
        public bool HasPreviousPage => Page > 1;
    }

    public class DashboardStatsDto
    {
        public int TotalPatients { get; set; }
        public int TotalDoctors { get; set; }
        public int TotalSpecialties { get; set; }
        public int TotalAppointments { get; set; }
        public int PendingAppointments { get; set; }
        public int ApprovedAppointments { get; set; }
        public int TodayAppointments { get; set; }
        public int ThisWeekAppointments { get; set; }
    }

    public class DoctorProfileDto
    {
        public DoctorDto? DoctorInfo { get; set; }
        public UserDto? UserInfo { get; set; }
        public SpecialtyDto? Specialty { get; set; }
        public IEnumerable<AppointmentDto> RecentAppointments { get; set; } = new List<AppointmentDto>();
        public int TotalAppointments { get; set; }
        public int CompletedAppointments { get; set; }
        public decimal AverageRating { get; set; }
    }

    public class PatientProfileDto
    {
        public UserDto? UserInfo { get; set; }
        public IEnumerable<AppointmentDto> AppointmentHistory { get; set; } = new List<AppointmentDto>();
        public IEnumerable<AppointmentDto> UpcomingAppointments { get; set; } = new List<AppointmentDto>();
        public int TotalAppointments { get; set; }
        public DateTime? LastAppointmentDate { get; set; }
    }

    public class BookingRequestDto
    {
        public string? PatientId { get; set; }
        public string? DoctorId { get; set; }
        public DateTime PreferredDate { get; set; }
        public TimeSpan PreferredTime { get; set; }
        public string? ReasonForVisit { get; set; }
        public string? Notes { get; set; }
    }

    public class TimeSlotDto
    {
        public TimeSpan StartTime { get; set; }
        public TimeSpan EndTime { get; set; }
        public bool IsAvailable { get; set; }
        public string? SlotType { get; set; } // "Available", "Booked", "Break"
    }

    public class DoctorScheduleDto
    {
        public string? DoctorId { get; set; }
        public DateTime Date { get; set; }
        public DayOfWeek DayOfWeek { get; set; }
        public IEnumerable<TimeSlotDto> TimeSlots { get; set; } = new List<TimeSlotDto>();
        public bool IsWorkingDay { get; set; }
    }
}

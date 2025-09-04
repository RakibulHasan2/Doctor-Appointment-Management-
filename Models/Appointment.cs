using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace DoctorAppointmentAPI.Models
{
    public class Appointment
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }

        [BsonElement("patientId")]
        [BsonRepresentation(BsonType.ObjectId)]
        public required string PatientId { get; set; }

        [BsonElement("doctorId")]
        [BsonRepresentation(BsonType.ObjectId)]
        public required string DoctorId { get; set; }

        [BsonElement("appointmentDate")]
        public DateTime AppointmentDate { get; set; }

        [BsonElement("startTime")]
        public TimeSpan StartTime { get; set; }

        [BsonElement("endTime")]
        public TimeSpan EndTime { get; set; }

        [BsonElement("status")]
        public AppointmentStatus Status { get; set; } = AppointmentStatus.Pending;

        [BsonElement("reasonForVisit")]
        public string? ReasonForVisit { get; set; }

        [BsonElement("notes")]
        public string? Notes { get; set; }

        [BsonElement("consultationFee")]
        public decimal ConsultationFee { get; set; }

        [BsonElement("createdAt")]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [BsonElement("updatedAt")]
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        [BsonElement("cancelledAt")]
        public DateTime? CancelledAt { get; set; }

        [BsonElement("cancellationReason")]
        public string? CancellationReason { get; set; }

        [BsonElement("approvedAt")]
        public DateTime? ApprovedAt { get; set; }

        [BsonElement("approvedBy")]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? ApprovedBy { get; set; }

        // Navigation properties (not stored in DB)
        [BsonIgnore]
        public User? Patient { get; set; }

        [BsonIgnore]
        public Doctor? Doctor { get; set; }
    }

    public enum AppointmentStatus
    {
        Pending,
        Approved,
        Rejected,
        Cancelled,
        Completed,
        NoShow
    }
}

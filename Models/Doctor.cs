using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace DoctorAppointmentAPI.Models
{
    public class Doctor
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }

        [BsonElement("userId")]
        [BsonRepresentation(BsonType.ObjectId)]
        public required string UserId { get; set; }

        [BsonElement("specialtyId")]
        [BsonRepresentation(BsonType.ObjectId)]
        public required string SpecialtyId { get; set; }

        [BsonElement("licenseNumber")]
        public required string LicenseNumber { get; set; }

        [BsonElement("experience")]
        public int Experience { get; set; } // Years of experience

        [BsonElement("qualification")]
        public string? Qualification { get; set; }

        [BsonElement("consultationFee")]
        public decimal ConsultationFee { get; set; }

        [BsonElement("availability")]
        public List<DoctorAvailability> Availability { get; set; } = new();

        [BsonElement("isApproved")]
        public bool IsApproved { get; set; } = false;

        [BsonElement("isRejected")]
        public bool IsRejected { get; set; } = false;

        [BsonElement("rejectionReason")]
        public string? RejectionReason { get; set; }

        [BsonElement("rejectedAt")]
        public DateTime? RejectedAt { get; set; }

        [BsonElement("isActive")]
        public bool IsActive { get; set; } = true;

        [BsonElement("createdAt")]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [BsonElement("updatedAt")]
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        // Navigation properties (not stored in DB)
        [BsonIgnore]
        public User? User { get; set; }

        [BsonIgnore]
        public Specialty? Specialty { get; set; }
    }

    public class DoctorAvailability
    {
        [BsonElement("dayOfWeek")]
        public DayOfWeek DayOfWeek { get; set; }

        [BsonElement("startTime")]
        public TimeSpan StartTime { get; set; }

        [BsonElement("endTime")]
        public TimeSpan EndTime { get; set; }

        [BsonElement("isAvailable")]
        public bool IsAvailable { get; set; } = true;
    }
}

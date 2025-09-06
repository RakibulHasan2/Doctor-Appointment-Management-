# Doctor Rejection Functionality Implementation

## Overview
This document describes the complete implementation of the doctor rejection functionality in the Doctor Appointment Management System API.

## What Was Implemented

### 1. Doctor Model Updates
**File:** `Models/Doctor.cs`

Added the following properties to support rejection workflow:
```csharp
public bool IsRejected { get; set; } = false;
public string? RejectionReason { get; set; }
public DateTime? RejectedAt { get; set; }
```

### 2. DTO Updates
**File:** `DTOs/DoctorDTOs.cs`

Updated `DoctorDto` to include rejection fields:
```csharp
public bool IsRejected { get; set; }
public string? RejectionReason { get; set; }
public DateTime? RejectedAt { get; set; }
```

**Existing DTO:** `RejectDoctorDto` in `DTOs/CommonDTOs.cs`
```csharp
public class RejectDoctorDto
{
    public required string Reason { get; set; }
}
```

### 3. Service Layer Updates
**File:** `Services/DoctorService.cs`

#### Interface Addition:
```csharp
Task<bool> RejectDoctorAsync(string id, string reason);
```

#### Implementation:
```csharp
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
```

#### ApprovalMethod Enhancement:
Updated `ApproveDoctorAsync` to reset rejection fields when approving:
```csharp
doctor.IsApproved = true;
doctor.IsRejected = false;
doctor.RejectionReason = null;
doctor.RejectedAt = null;
```

### 4. Controller Updates
**File:** `Controllers/DoctorsController.cs`

#### Fixed Pending Approval Logic:
```csharp
[HttpGet("pending-approval")]
public async Task<ActionResult<IEnumerable<DoctorDto>>> GetPendingApprovalDoctors()
{
    try
    {
        var doctors = await _doctorService.GetAllDoctorsAsync();
        var pendingDoctors = doctors.Where(d => !d.IsApproved && !d.IsRejected);
        return Ok(pendingDoctors);
    }
    // ... error handling
}
```

#### Added Rejected Doctors Endpoint:
```csharp
[HttpGet("rejected")]
public async Task<ActionResult<IEnumerable<DoctorDto>>> GetRejectedDoctors()
{
    try
    {
        var doctors = await _doctorService.GetAllDoctorsAsync();
        var rejectedDoctors = doctors.Where(d => d.IsRejected);
        return Ok(rejectedDoctors);
    }
    // ... error handling
}
```

#### Implemented Complete Rejection Endpoint:
```csharp
[HttpPatch("{id}/reject")]
public async Task<ActionResult> RejectDoctor(string id, [FromBody] RejectDoctorDto rejectRequest)
{
    try
    {
        // Validate input
        if (!IsValidObjectId(id))
        {
            return BadRequest(new { message = "Invalid doctor ID format" });
        }

        if (string.IsNullOrWhiteSpace(rejectRequest?.Reason))
        {
            return BadRequest(new { message = "Rejection reason is required" });
        }

        var success = await _doctorService.RejectDoctorAsync(id, rejectRequest.Reason);

        if (!success)
        {
            return NotFound(new { message = "Doctor not found" });
        }

        return Ok(new { message = "Doctor rejected successfully" });
    }
    catch (Exception ex)
    {
        _logger.LogError(ex, "Error rejecting doctor with ID: {DoctorId}", id);
        return StatusCode(500, new { message = "An error occurred while rejecting the doctor" });
    }
}
```

### 5. Mapping Configuration
**File:** `Configuration/MappingProfile.cs`

Updated AutoMapper configuration to handle rejection fields:
```csharp
CreateMap<CreateDoctorDto, Doctor>()
    .ForMember(dest => dest.IsRejected, opt => opt.MapFrom(src => false))
    .ForMember(dest => dest.RejectionReason, opt => opt.Ignore())
    .ForMember(dest => dest.RejectedAt, opt => opt.Ignore())
    // ... other mappings
```

### 6. API Documentation
**File:** `Documentation/API_Documentation.md`

Added complete documentation for the rejection endpoint:
```http
PATCH /api/doctors/{id}/reject
Content-Type: application/json

{
  "reason": "Invalid medical license"
}
```

### 7. Test Files
**File:** `DoctorAppointmentAPI.http`

Added HTTP test for the rejection functionality:
```http
PATCH {{DoctorAppointmentAPI_HostAddress}}/api/doctors/{doctorId}/reject
Content-Type: application/json

{
  "reason": "Invalid medical license"
}
```

## API Usage

### Reject a Doctor
```bash
curl -X PATCH "http://localhost:5289/api/doctors/{doctorId}/reject" \
     -H "Content-Type: application/json" \
     -d '{"reason": "Invalid medical license"}'
```

### Response Examples

**Success Response (200 OK):**
```json
{
  "message": "Doctor rejected successfully"
}
```

**Error Responses:**
- **400 Bad Request:** Invalid doctor ID format or missing reason
- **404 Not Found:** Doctor not found
- **500 Internal Server Error:** Server error

## Doctor State Management

### Doctor States
1. **Pending:** `IsApproved = false, IsRejected = false` (neither approved nor rejected)
2. **Approved:** `IsApproved = true, IsRejected = false`
3. **Rejected:** `IsApproved = false, IsRejected = true`

### State Transitions
- **Pending → Approved:** Sets `IsApproved = true`, clears rejection fields
- **Pending → Rejected:** Sets `IsRejected = true`, records reason and timestamp
- **Rejected → Approved:** Sets `IsApproved = true`, clears rejection fields
- **Approved → Rejected:** Sets `IsRejected = true`, records reason and timestamp

### API Endpoints for Different States

#### Get Pending Doctors (Corrected Logic)
```http
GET /api/doctors/pending-approval
```
Returns doctors where `IsApproved = false AND IsRejected = false`

#### Get Approved Doctors
```http
GET /api/doctors/approved
```
Returns doctors where `IsApproved = true`

#### Get Rejected Doctors
```http
GET /api/doctors/rejected
```
Returns doctors where `IsRejected = true`

## Database Fields

When a doctor is rejected, the following fields are updated:
- `IsApproved`: Set to `false`
- `IsRejected`: Set to `true`
- `RejectionReason`: Set to the provided reason
- `RejectedAt`: Set to current UTC timestamp
- `UpdatedAt`: Set to current UTC timestamp

## Validation

The rejection endpoint includes:
1. **ObjectId Validation:** Ensures valid MongoDB ObjectId format
2. **Reason Validation:** Ensures rejection reason is provided and not empty
3. **Doctor Existence:** Verifies doctor exists before processing
4. **Error Handling:** Comprehensive error handling with appropriate HTTP status codes

## Testing

To test the functionality:
1. Start the API server: `dotnet run`
2. Create a doctor profile (if none exists)
3. Use the HTTP test file or cURL to reject the doctor
4. Verify the doctor's status in the database

## Integration Notes

This implementation is fully integrated with:
- ✅ MongoDB document updates
- ✅ AutoMapper field mapping
- ✅ API documentation
- ✅ Error handling and logging
- ✅ Input validation
- ✅ State management consistency

The doctor rejection functionality is now complete and ready for production use.

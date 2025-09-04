using DoctorAppointmentAPI.DTOs;
using DoctorAppointmentAPI.Services;
using Microsoft.AspNetCore.Mvc;

namespace DoctorAppointmentAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DoctorsController : ControllerBase
    {
        private readonly IDoctorService _doctorService;
        private readonly ILogger<DoctorsController> _logger;

        public DoctorsController(IDoctorService doctorService, ILogger<DoctorsController> logger)
        {
            _doctorService = doctorService;
            _logger = logger;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<DoctorDto>>> GetDoctors([FromQuery] bool? isApproved = null)
        {
            try
            {
                var doctors = await _doctorService.GetAllDoctorsAsync();
                
                if (isApproved.HasValue)
                {
                    doctors = doctors.Where(d => d.IsApproved == isApproved.Value);
                }
                
                return Ok(doctors);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving doctors");
                return StatusCode(500, new { message = "An error occurred while retrieving doctors" });
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<DoctorDto>> GetDoctor(string id)
        {
            try
            {
                var doctor = await _doctorService.GetDoctorByIdAsync(id);
                if (doctor == null)
                    return NotFound(new { message = "Doctor not found" });

                return Ok(doctor);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving doctor with ID: {DoctorId}", id);
                return StatusCode(500, new { message = "An error occurred while retrieving the doctor" });
            }
        }

        [HttpGet("user/{userId}")]
        public async Task<ActionResult<DoctorDto>> GetDoctorByUserId(string userId)
        {
            try
            {
                var doctor = await _doctorService.GetDoctorByUserIdAsync(userId);
                if (doctor == null)
                    return NotFound(new { message = "Doctor profile not found for this user" });

                return Ok(doctor);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving doctor for user ID: {UserId}", userId);
                return StatusCode(500, new { message = "An error occurred while retrieving the doctor profile" });
            }
        }

        [HttpGet("specialty/{specialtyId}")]
        public async Task<ActionResult<IEnumerable<DoctorDto>>> GetDoctorsBySpecialty(string specialtyId)
        {
            try
            {
                var doctors = await _doctorService.GetDoctorsBySpecialtyAsync(specialtyId);
                return Ok(doctors);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving doctors for specialty ID: {SpecialtyId}", specialtyId);
                return StatusCode(500, new { message = "An error occurred while retrieving doctors for this specialty" });
            }
        }

        [HttpGet("pending-approval")]
        public async Task<ActionResult<IEnumerable<DoctorDto>>> GetPendingApprovalDoctors()
        {
            try
            {
                var doctors = await _doctorService.GetAllDoctorsAsync();
                var pendingDoctors = doctors.Where(d => !d.IsApproved);
                return Ok(pendingDoctors);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving pending approval doctors");
                return StatusCode(500, new { message = "An error occurred while retrieving pending approval doctors" });
            }
        }

        [HttpGet("{doctorId}/available-slots")]
        public async Task<ActionResult<IEnumerable<TimeSlotDto>>> GetAvailableSlots(string doctorId, [FromQuery] DateTime date)
        {
            try
            {
                if (date.Date < DateTime.Today)
                {
                    return BadRequest(new { message = "Cannot check availability for past dates" });
                }

                var slots = await _doctorService.GetAvailableSlotsAsync(doctorId, date);
                return Ok(slots);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving available slots for doctor ID: {DoctorId} on date: {Date}", doctorId, date);
                return StatusCode(500, new { message = "An error occurred while retrieving available slots" });
            }
        }

        [HttpPost]
        public async Task<ActionResult<DoctorDto>> CreateDoctor([FromBody] CreateDoctorDto createDoctorRequest)
        {
            try
            {
                var doctor = await _doctorService.CreateDoctorAsync(createDoctorRequest);
                return CreatedAtAction(nameof(GetDoctor), new { id = doctor.Id }, doctor);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating doctor profile for user ID: {UserId}", createDoctorRequest.UserId);
                return StatusCode(500, new { message = "An error occurred while creating the doctor profile" });
            }
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<DoctorDto>> UpdateDoctor(string id, [FromBody] UpdateDoctorDto updateDoctorRequest)
        {
            try
            {
                var doctor = await _doctorService.UpdateDoctorAsync(id, updateDoctorRequest);
                if (doctor == null)
                    return NotFound(new { message = "Doctor not found" });

                return Ok(doctor);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating doctor with ID: {DoctorId}", id);
                return StatusCode(500, new { message = "An error occurred while updating the doctor" });
            }
        }

        [HttpPatch("{id}/approve")]
        public async Task<ActionResult> ApproveDoctor(string id)
        {
            try
            {
                var success = await _doctorService.ApproveDoctorAsync(id);
                if (!success)
                    return NotFound(new { message = "Doctor not found" });

                return Ok(new { message = "Doctor approved successfully" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error approving doctor with ID: {DoctorId}", id);
                return StatusCode(500, new { message = "An error occurred while approving the doctor" });
            }
        }

        [HttpPatch("{id}/reject")]
        public ActionResult RejectDoctor(string id, [FromBody] RejectDoctorDto rejectRequest)
        {
            try
            {
                // Since RejectDoctorAsync doesn't exist, let's use a different approach
                // For now, we'll just return a not implemented message
                return BadRequest(new { message = "Doctor rejection functionality not implemented yet" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error rejecting doctor with ID: {DoctorId}", id);
                return StatusCode(500, new { message = "An error occurred while rejecting the doctor" });
            }
        }

        [HttpPatch("{id}/availability")]
        public async Task<ActionResult> UpdateAvailability(string id, [FromBody] List<DoctorAvailabilityDto> availability)
        {
            try
            {
                var success = await _doctorService.UpdateAvailabilityAsync(id, availability);
                if (!success)
                    return NotFound(new { message = "Doctor not found" });

                return Ok(new { message = "Availability updated successfully" });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating availability for doctor with ID: {DoctorId}", id);
                return StatusCode(500, new { message = "An error occurred while updating doctor availability" });
            }
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteDoctor(string id)
        {
            try
            {
                var success = await _doctorService.DeleteDoctorAsync(id);
                if (!success)
                    return NotFound(new { message = "Doctor not found" });

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting doctor with ID: {DoctorId}", id);
                return StatusCode(500, new { message = "An error occurred while deleting the doctor" });
            }
        }
    }
}

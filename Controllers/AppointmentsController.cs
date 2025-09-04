using DoctorAppointmentAPI.DTOs;
using DoctorAppointmentAPI.Models;
using DoctorAppointmentAPI.Services;
using Microsoft.AspNetCore.Mvc;

namespace DoctorAppointmentAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AppointmentsController : ControllerBase
    {
        private readonly IAppointmentService _appointmentService;
        private readonly ILogger<AppointmentsController> _logger;

        public AppointmentsController(IAppointmentService appointmentService, ILogger<AppointmentsController> logger)
        {
            _appointmentService = appointmentService;
            _logger = logger;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<AppointmentDto>>> GetAppointments([FromQuery] string? userId = null, [FromQuery] UserRole? userRole = null)
        {
            try
            {
                var appointments = await _appointmentService.GetAppointmentsAsync(userId, userRole);
                return Ok(appointments);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving appointments");
                return StatusCode(500, new { message = "An error occurred while retrieving appointments" });
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<AppointmentDto>> GetAppointment(string id)
        {
            try
            {
                var appointment = await _appointmentService.GetAppointmentByIdAsync(id);
                if (appointment == null)
                    return NotFound(new { message = "Appointment not found" });

                return Ok(appointment);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving appointment with ID: {AppointmentId}", id);
                return StatusCode(500, new { message = "An error occurred while retrieving the appointment" });
            }
        }

        [HttpPost]
        public async Task<ActionResult<AppointmentDto>> CreateAppointment([FromBody] CreateAppointmentDto createAppointmentDto, [FromQuery] string patientId)
        {
            try
            {
                var appointment = await _appointmentService.CreateAppointmentAsync(createAppointmentDto, patientId);
                return CreatedAtAction(nameof(GetAppointment), new { id = appointment.Id }, appointment);
            }
            catch (ArgumentException ex)
            {
                _logger.LogWarning("Appointment creation failed: {Message}", ex.Message);
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating appointment");
                return StatusCode(500, new { message = "An error occurred while creating the appointment" });
            }
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<AppointmentDto>> UpdateAppointment(string id, [FromBody] UpdateAppointmentDto updateAppointmentDto)
        {
            try
            {
                var updatedAppointment = await _appointmentService.UpdateAppointmentAsync(id, updateAppointmentDto);
                if (updatedAppointment == null)
                    return NotFound(new { message = "Appointment not found" });

                return Ok(updatedAppointment);
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogWarning("Appointment update failed: {Message}", ex.Message);
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating appointment with ID: {AppointmentId}", id);
                return StatusCode(500, new { message = "An error occurred while updating the appointment" });
            }
        }

        [HttpPatch("{id}/status")]
        public async Task<ActionResult<AppointmentDto>> UpdateAppointmentStatus(string id, [FromBody] AppointmentStatusUpdateDto statusUpdateDto, [FromQuery] string? userId = null)
        {
            try
            {
                var updatedAppointment = await _appointmentService.UpdateAppointmentStatusAsync(id, statusUpdateDto, userId);
                if (updatedAppointment == null)
                    return NotFound(new { message = "Appointment not found" });

                return Ok(updatedAppointment);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating appointment status for ID: {AppointmentId}", id);
                return StatusCode(500, new { message = "An error occurred while updating the appointment status" });
            }
        }

        [HttpPatch("{id}/cancel")]
        public async Task<ActionResult> CancelAppointment(string id, [FromBody] CancelAppointmentRequest request, [FromQuery] string? userId = null)
        {
            try
            {
                var result = await _appointmentService.CancelAppointmentAsync(id, request.Reason ?? "No reason provided", userId);

                if (!result)
                    return NotFound(new { message = "Appointment not found" });

                return Ok(new { message = "Appointment cancelled successfully" });
            }
            catch (UnauthorizedAccessException ex)
            {
                _logger.LogWarning("Unauthorized appointment cancellation attempt: {Message}", ex.Message);
                return Forbid(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error cancelling appointment with ID: {AppointmentId}", id);
                return StatusCode(500, new { message = "An error occurred while cancelling the appointment" });
            }
        }

        [HttpGet("search")]
        public async Task<ActionResult<IEnumerable<AppointmentDto>>> SearchAppointments([FromQuery] AppointmentSearchDto searchDto)
        {
            try
            {
                var appointments = await _appointmentService.SearchAppointmentsAsync(searchDto);
                return Ok(appointments);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error searching appointments");
                return StatusCode(500, new { message = "An error occurred while searching appointments" });
            }
        }

        [HttpGet("patient/{patientId}")]
        public async Task<ActionResult<IEnumerable<AppointmentDto>>> GetPatientAppointments(string patientId)
        {
            try
            {
                var appointments = await _appointmentService.GetAppointmentsAsync(patientId, UserRole.Patient);
                return Ok(appointments);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving appointments for patient: {PatientId}", patientId);
                return StatusCode(500, new { message = "An error occurred while retrieving patient appointments" });
            }
        }

        [HttpGet("doctor/{doctorId}")]
        public async Task<ActionResult<IEnumerable<AppointmentDto>>> GetDoctorAppointments(string doctorId)
        {
            try
            {
                var appointments = await _appointmentService.GetDoctorAppointmentsAsync(doctorId);
                return Ok(appointments);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving appointments for doctor: {DoctorId}", doctorId);
                return StatusCode(500, new { message = "An error occurred while retrieving doctor appointments" });
            }
        }

        [HttpGet("pending")]
        public async Task<ActionResult<IEnumerable<AppointmentDto>>> GetPendingAppointments()
        {
            try
            {
                var searchDto = new AppointmentSearchDto
                {
                    Status = AppointmentStatus.Pending,
                    Page = 1,
                    PageSize = 100
                };
                var appointments = await _appointmentService.SearchAppointmentsAsync(searchDto);
                return Ok(appointments);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving pending appointments");
                return StatusCode(500, new { message = "An error occurred while retrieving pending appointments" });
            }
        }

        [HttpGet("approved")]
        public async Task<ActionResult<IEnumerable<AppointmentDto>>> GetApprovedAppointments()
        {
            try
            {
                var searchDto = new AppointmentSearchDto
                {
                    Status = AppointmentStatus.Approved,
                    Page = 1,
                    PageSize = 100
                };
                var appointments = await _appointmentService.SearchAppointmentsAsync(searchDto);
                return Ok(appointments);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving approved appointments");
                return StatusCode(500, new { message = "An error occurred while retrieving approved appointments" });
            }
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteAppointment(string id)
        {
            try
            {
                var result = await _appointmentService.CancelAppointmentAsync(id, "Appointment deleted", null);
                if (!result)
                    return NotFound(new { message = "Appointment not found" });

                return Ok(new { message = "Appointment deleted successfully" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting appointment with ID: {AppointmentId}", id);
                return StatusCode(500, new { message = "An error occurred while deleting the appointment" });
            }
        }
    }

    public class CancelAppointmentRequest
    {
        public string? Reason { get; set; }
    }
}

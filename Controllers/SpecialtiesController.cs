using DoctorAppointmentAPI.DTOs;
using DoctorAppointmentAPI.Services;
using Microsoft.AspNetCore.Mvc;

namespace DoctorAppointmentAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SpecialtiesController : ControllerBase
    {
        private readonly ISpecialtyService _specialtyService;
        private readonly ILogger<SpecialtiesController> _logger;

        public SpecialtiesController(ISpecialtyService specialtyService, ILogger<SpecialtiesController> logger)
        {
            _specialtyService = specialtyService;
            _logger = logger;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<SpecialtyDto>>> GetSpecialties()
        {
            try
            {
                var specialties = await _specialtyService.GetAllSpecialtiesAsync();
                return Ok(specialties);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving specialties");
                return StatusCode(500, new { message = "An error occurred while retrieving specialties" });
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<SpecialtyDto>> GetSpecialty(string id)
        {
            try
            {
                var specialty = await _specialtyService.GetSpecialtyByIdAsync(id);
                if (specialty == null)
                    return NotFound(new { message = "Specialty not found" });

                return Ok(specialty);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving specialty with ID: {SpecialtyId}", id);
                return StatusCode(500, new { message = "An error occurred while retrieving the specialty" });
            }
        }

        [HttpPost]
        public async Task<ActionResult<SpecialtyDto>> CreateSpecialty([FromBody] CreateSpecialtyDto createSpecialtyDto)
        {
            try
            {
                var specialty = await _specialtyService.CreateSpecialtyAsync(createSpecialtyDto);
                return CreatedAtAction(nameof(GetSpecialty), new { id = specialty.Id }, specialty);
            }
            catch (ArgumentException ex)
            {
                _logger.LogWarning("Specialty creation failed: {Message}", ex.Message);
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating specialty");
                return StatusCode(500, new { message = "An error occurred while creating the specialty" });
            }
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<SpecialtyDto>> UpdateSpecialty(string id, [FromBody] UpdateSpecialtyDto updateSpecialtyDto)
        {
            try
            {
                var updatedSpecialty = await _specialtyService.UpdateSpecialtyAsync(id, updateSpecialtyDto);
                if (updatedSpecialty == null)
                    return NotFound(new { message = "Specialty not found" });

                return Ok(updatedSpecialty);
            }
            catch (ArgumentException ex)
            {
                _logger.LogWarning("Specialty update failed: {Message}", ex.Message);
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating specialty with ID: {SpecialtyId}", id);
                return StatusCode(500, new { message = "An error occurred while updating the specialty" });
            }
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteSpecialty(string id)
        {
            try
            {
                var result = await _specialtyService.DeleteSpecialtyAsync(id);
                if (!result)
                    return NotFound(new { message = "Specialty not found" });

                return Ok(new { message = "Specialty deleted successfully" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting specialty with ID: {SpecialtyId}", id);
                return StatusCode(500, new { message = "An error occurred while deleting the specialty" });
            }
        }

        [HttpGet("active")]
        public async Task<ActionResult<IEnumerable<SpecialtyDto>>> GetActiveSpecialties()
        {
            try
            {
                var specialties = await _specialtyService.GetAllSpecialtiesAsync();
                var activeSpecialties = specialties.Where(s => s.IsActive);
                return Ok(activeSpecialties);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving active specialties");
                return StatusCode(500, new { message = "An error occurred while retrieving active specialties" });
            }
        }
    }
}

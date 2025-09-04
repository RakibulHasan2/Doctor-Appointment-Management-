using DoctorAppointmentAPI.DTOs;
using DoctorAppointmentAPI.Models;
using DoctorAppointmentAPI.Services;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;

namespace DoctorAppointmentAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UsersController : ControllerBase
    {
        private readonly IMongoDbService _mongoDbService;
        private readonly ILogger<UsersController> _logger;

        public UsersController(IMongoDbService mongoDbService, ILogger<UsersController> logger)
        {
            _mongoDbService = mongoDbService;
            _logger = logger;
        }

        [HttpPost("register")]
        public async Task<ActionResult<UserDto>> Register([FromBody] RegisterUserDto registerRequest)
        {
            try
            {
                if (registerRequest.Password != registerRequest.ConfirmPassword)
                {
                    return BadRequest(new { message = "Passwords do not match" });
                }

                var existingUser = await _mongoDbService.Users
                    .Find(u => u.Email == registerRequest.Email)
                    .FirstOrDefaultAsync();

                if (existingUser != null)
                {
                    return BadRequest(new { message = "User with this email already exists" });
                }

                var user = new User
                {
                    Name = registerRequest.Name,
                    Email = registerRequest.Email,
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword(registerRequest.Password),
                    Role = registerRequest.Role,
                    Phone = registerRequest.Phone,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow,
                    IsActive = true
                };

                await _mongoDbService.Users.InsertOneAsync(user);

                var userDto = new UserDto
                {
                    Id = user.Id,
                    Name = user.Name,
                    Email = user.Email,
                    Role = user.Role,
                    Phone = user.Phone,
                    IsActive = user.IsActive,
                    CreatedAt = user.CreatedAt
                };

                return CreatedAtAction(nameof(GetUser), new { id = user.Id }, userDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during registration for email: {Email}", registerRequest.Email);
                return StatusCode(500, new { message = "An error occurred during registration" });
            }
        }

        [HttpPost("login")]
        public async Task<ActionResult<LoginResponseDto>> Login([FromBody] LoginUserDto loginRequest)
        {
            try
            {
                var user = await _mongoDbService.Users
                    .Find(u => u.Email == loginRequest.Email && u.IsActive)
                    .FirstOrDefaultAsync();

                if (user == null || !BCrypt.Net.BCrypt.Verify(loginRequest.Password, user.PasswordHash))
                {
                    return Unauthorized(new { message = "Invalid email or password" });
                }

                var userDto = new UserDto
                {
                    Id = user.Id,
                    Name = user.Name,
                    Email = user.Email,
                    Role = user.Role,
                    Phone = user.Phone,
                    IsActive = user.IsActive,
                    CreatedAt = user.CreatedAt
                };

                var response = new LoginResponseDto
                {
                    User = userDto,
                    Message = "Login successful"
                };

                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during login for email: {Email}", loginRequest.Email);
                return StatusCode(500, new { message = "An error occurred during login" });
            }
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<UserDto>>> GetAllUsers()
        {
            try
            {
                var users = await _mongoDbService.Users
                    .Find(u => u.IsActive)
                    .ToListAsync();

                var userDtos = users.Select(u => new UserDto
                {
                    Id = u.Id,
                    Name = u.Name,
                    Email = u.Email,
                    Role = u.Role,
                    Phone = u.Phone,
                    IsActive = u.IsActive,
                    CreatedAt = u.CreatedAt
                }).ToList();

                return Ok(userDtos);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving users");
                return StatusCode(500, new { message = "An error occurred while retrieving users" });
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<UserDto>> GetUser(string id)
        {
            try
            {
                var user = await _mongoDbService.Users
                    .Find(u => u.Id == id && u.IsActive)
                    .FirstOrDefaultAsync();

                if (user == null)
                    return NotFound(new { message = "User not found" });

                var userDto = new UserDto
                {
                    Id = user.Id,
                    Name = user.Name,
                    Email = user.Email,
                    Role = user.Role,
                    Phone = user.Phone,
                    IsActive = user.IsActive,
                    CreatedAt = user.CreatedAt
                };

                return Ok(userDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving user with ID: {UserId}", id);
                return StatusCode(500, new { message = "An error occurred while retrieving the user" });
            }
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<UserDto>> UpdateUser(string id, [FromBody] UpdateUserDto updateUserDto)
        {
            try
            {
                var user = await _mongoDbService.Users
                    .Find(u => u.Id == id)
                    .FirstOrDefaultAsync();

                if (user == null)
                    return NotFound(new { message = "User not found" });

                if (updateUserDto.Name != null)
                    user.Name = updateUserDto.Name;

                if (updateUserDto.Phone != null)
                    user.Phone = updateUserDto.Phone;

                if (updateUserDto.IsActive.HasValue)
                    user.IsActive = updateUserDto.IsActive.Value;

                user.UpdatedAt = DateTime.UtcNow;

                await _mongoDbService.Users.ReplaceOneAsync(u => u.Id == id, user);

                var userDto = new UserDto
                {
                    Id = user.Id,
                    Name = user.Name,
                    Email = user.Email,
                    Role = user.Role,
                    Phone = user.Phone,
                    IsActive = user.IsActive,
                    CreatedAt = user.CreatedAt
                };

                return Ok(userDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating user with ID: {UserId}", id);
                return StatusCode(500, new { message = "An error occurred while updating the user" });
            }
        }

        [HttpPost("{id}/change-password")]
        public async Task<ActionResult> ChangePassword(string id, [FromBody] ChangePasswordDto changePasswordDto)
        {
            try
            {
                if (id != changePasswordDto.UserId)
                    return BadRequest(new { message = "User ID mismatch" });

                if (changePasswordDto.NewPassword != changePasswordDto.ConfirmNewPassword)
                    return BadRequest(new { message = "New passwords do not match" });

                var user = await _mongoDbService.Users
                    .Find(u => u.Id == id && u.IsActive)
                    .FirstOrDefaultAsync();

                if (user == null)
                    return NotFound(new { message = "User not found" });

                if (!BCrypt.Net.BCrypt.Verify(changePasswordDto.CurrentPassword, user.PasswordHash))
                    return BadRequest(new { message = "Current password is incorrect" });

                user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(changePasswordDto.NewPassword);
                user.UpdatedAt = DateTime.UtcNow;

                await _mongoDbService.Users.ReplaceOneAsync(u => u.Id == id, user);

                return Ok(new { message = "Password changed successfully" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error changing password for user with ID: {UserId}", id);
                return StatusCode(500, new { message = "An error occurred while changing the password" });
            }
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteUser(string id)
        {
            try
            {
                var user = await _mongoDbService.Users
                    .Find(u => u.Id == id)
                    .FirstOrDefaultAsync();

                if (user == null)
                    return NotFound(new { message = "User not found" });

                // Soft delete - mark as inactive
                user.IsActive = false;
                user.UpdatedAt = DateTime.UtcNow;

                await _mongoDbService.Users.ReplaceOneAsync(u => u.Id == id, user);

                return Ok(new { message = "User deleted successfully" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting user with ID: {UserId}", id);
                return StatusCode(500, new { message = "An error occurred while deleting the user" });
            }
        }

        [HttpGet("by-role/{role}")]
        public async Task<ActionResult<IEnumerable<UserDto>>> GetUsersByRole(UserRole role)
        {
            try
            {
                var users = await _mongoDbService.Users
                    .Find(u => u.Role == role && u.IsActive)
                    .ToListAsync();

                var userDtos = users.Select(u => new UserDto
                {
                    Id = u.Id,
                    Name = u.Name,
                    Email = u.Email,
                    Role = u.Role,
                    Phone = u.Phone,
                    IsActive = u.IsActive,
                    CreatedAt = u.CreatedAt
                }).ToList();

                return Ok(userDtos);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving users with role: {Role}", role);
                return StatusCode(500, new { message = "An error occurred while retrieving users" });
            }
        }
    }
}

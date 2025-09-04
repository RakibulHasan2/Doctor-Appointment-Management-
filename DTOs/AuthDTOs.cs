using DoctorAppointmentAPI.Models;

namespace DoctorAppointmentAPI.DTOs
{
    // User Registration DTO
    public class RegisterUserDto
    {
        public required string Name { get; set; }
        public required string Email { get; set; }
        public required string Password { get; set; }
        public required string ConfirmPassword { get; set; }
        public required UserRole Role { get; set; }
        public string? Phone { get; set; }
    }

    // User Login DTO  
    public class LoginUserDto
    {
        public required string Email { get; set; }
        public required string Password { get; set; }
    }

    // User DTOs
    public class UserDto
    {
        public string? Id { get; set; }
        public required string Name { get; set; }
        public required string Email { get; set; }
        public required UserRole Role { get; set; }
        public string? Phone { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    public class CreateUserDto
    {
        public required string Name { get; set; }
        public required string Email { get; set; }
        public required string Password { get; set; }
        public required UserRole Role { get; set; }
        public string? Phone { get; set; }
    }

    public class UpdateUserDto
    {
        public string? Name { get; set; }
        public string? Phone { get; set; }
        public bool? IsActive { get; set; }
    }

    public class ChangePasswordDto
    {
        public required string UserId { get; set; }
        public required string CurrentPassword { get; set; }
        public required string NewPassword { get; set; }
        public required string ConfirmNewPassword { get; set; }
    }

    public class LoginResponseDto
    {
        public required UserDto User { get; set; }
        public string? Message { get; set; }
    }
}

using EmployeeTimeTrackingBackend.Models;
using EmployeeTimeTrackingBackend.Services;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace EmployeeTimeTrackingBackend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        private readonly ILogger<AuthService> _logger;

        public AuthController(IAuthService authService, ILogger<AuthService> logger)
        {
            _authService = authService;
            _logger = logger;
        }

        // POST: api/employee/login
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto loginDto)
        {
            try
            {
                if (loginDto == null || loginDto.employeeNo <= 0 || string.IsNullOrEmpty(loginDto.Password))
                {
                    return BadRequest(new { Message = "Invalid login credentials provided." });
                }

                var employee = await _authService.LoginAsync(loginDto.employeeNo, loginDto.Password);
                if (employee == null)
                {
                    return Unauthorized(new { Message = "Invalid credentials." });
                }

                return Ok(new
                {
                    Message = employee.IsFirstLogin ? "Please change your password." : "Login successful",
                    Employee = employee
                });
            }
            catch (Exception ex)
            {
                _logger.LogError($"Login error for EmployeeNo {loginDto?.employeeNo}: {ex.Message}");
                return StatusCode(500, new { Message = "An error occurred while processing your login request." });
            }
        }

        // POST: api/employee/{employeeNo}/change-password
        [HttpPost("{employeeNo}/change-password")]
        public async Task<IActionResult> ChangePassword(int employeeNo, [FromBody] ChangePasswordDto request)
        {
            try
            {
                _logger.LogInformation($"Received ChangePassword request for EmployeeNo: {employeeNo}");

                if (employeeNo <= 0 || request == null || string.IsNullOrEmpty(request.NewPassword))
                {
                    return BadRequest(new { Message = "Invalid request. Employee number and new password are required." });
                }

                var updatedEmployee = await _authService.ChangePasswordAsync(employeeNo, request);
                if (updatedEmployee == null)
                {
                    _logger.LogError($"Employee with EmployeeNo {employeeNo} not found.");
                    return NotFound(new { Message = "Employee not found." });
                }

                _logger.LogInformation($"Password change successful for EmployeeNo: {employeeNo}");
                return Ok(new { Message = "Password changed successfully" });
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error changing password for EmployeeNo {employeeNo}: {ex.Message}");
                return StatusCode(500, new { Message = "An error occurred while processing your request." });
            }
        }
    }
}

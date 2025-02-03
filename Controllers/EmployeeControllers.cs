using EmployeeTimeTrackingBackend.Models;
using EmployeeTimeTrackingBackend.Services;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace EmployeeTimeTrackingBackend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EmployeeController : ControllerBase
    {
        private readonly IEmployeeService _employeeService;
        private readonly ILogger<EmployeeService> _logger;

        public EmployeeController(IEmployeeService employeeService, ILogger<EmployeeService> logger)
        {
            _employeeService = employeeService;
            _logger = logger;
        }

        // GET: api/employee
        [HttpGet]
        public async Task<IActionResult> GetAllEmployees()
        {
            var employees = await _employeeService.GetAllEmployeesAsync();
            return Ok(employees);
        }

        // GET: api/employee/{id}
        [HttpGet("{employeeNo}")]
        public async Task<IActionResult> GetEmployeeById(int employeeNo)
        {
            var employee = await _employeeService.GetEmployeeByIdAsync(employeeNo);
            if (employee == null)
            {
                return NotFound();
            }

            return Ok(employee);
        }

        // POST: api/employee
        [HttpPost]
        public async Task<IActionResult> AddEmployee([FromBody] EmployeeDto employeeDto)
        {
            try
            {
                var newEmployee = await _employeeService.AddEmployeeAsync(employeeDto);
                return CreatedAtAction(nameof(GetEmployeeById), new { employeeNo = newEmployee.EmployeeNo },
                    newEmployee);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // PUT: api/employee/{id}
        [HttpPut("{employeeNo}")]
        public async Task<IActionResult> UpdateEmployee(int employeeNo, [FromBody] EmployeeDto employeeDto)
        {
            var updatedEmployee = await _employeeService.UpdateEmployeeAsync(employeeNo, employeeDto);
            if (updatedEmployee == null)
            {
                return NotFound();
            }

            return Ok(updatedEmployee);
        }

        // DELETE: api/employee/{id}
        [HttpDelete("{employeeNo}")]
        public async Task<IActionResult> DeleteEmployee(int employeeNo)
        {
            var success = await _employeeService.DeleteEmployeeAsync(employeeNo);
            if (!success)
            {
                return NotFound();
            }

            return NoContent();
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto loginDto)
        {
            var employee = await _employeeService.LoginAsync(loginDto.employeeNo, loginDto.Password);
            if (employee == null)
            {
                return Unauthorized("Invalid credentials.");
            }

            // If this is the first login, the user gets to change their password
            if (employee.IsFirstLogin)
            {
                return Ok(new { Message = "Please change your password." });
            }

            // If it's not the first login, return a normal login process
            return Ok(new { Message = "Login successful", Employee = employee });
        }


        // POST: api/employee/{id}/clock-in
        [HttpPost("{employeeNo}/clock-in")]
        public async Task<IActionResult> ClockIn(int employeeNo)
        {
            var (success, message) = await _employeeService.ClockInAsync(employeeNo);

            if (!success)
            {
                if (message == "Employee not found.")
                {
                    return NotFound(new { Message = message });
                }

                return BadRequest(new { Message = message });
            }

            return Ok(new { Message = message });
        }

        // POST: api/employee/{id}/clock-out
        [HttpPost("{employeeNo}/clock-out")]
        public async Task<IActionResult> ClockOut(int employeeNo)
        {
            var success = await _employeeService.ClockOutAsync(employeeNo);
            if (!success)
            {
                return BadRequest("You are not clocked in.");
            }

            return Ok(new { Message = "Clocked out successfully" });
        }

        // GET: api/employee/{employeeNo}/clock-in-status
        [HttpGet("{employeeNo}/clock-in-status")]
        public async Task<IActionResult> GetClockInStatus(int employeeNo)
        {
            var isClockedIn = await _employeeService.IsClockedInAsync(employeeNo);
            return Ok(new { isClockedIn });
        }

        // POST: api/employee/{employeeNo}/change-password
        [HttpPost("{employeeNo}/change-password")]
        public async Task<IActionResult> ChangePassword(int employeeNo, [FromBody] ChangePasswordDto request)
        {
            _logger.LogInformation($"Received ChangePassword request for EmployeeNo: {employeeNo}");

            if (!ModelState.IsValid)
            {
                _logger.LogError("Validation failed for ChangePasswordDto: " + string.Join(", ",
                    ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage)));
                return BadRequest(ModelState);
            }

            if (string.IsNullOrEmpty(request.NewPassword))
            {
                _logger.LogError("New password is empty.");
                return BadRequest("New password is required.");
            }

            var employeeDto = await _employeeService.ChangePasswordAsync(employeeNo, request);
            if (employeeDto == null)
            {
                _logger.LogError($"Employee with EmployeeNo {employeeNo} not found.");
                return NotFound("Employee not found.");
            }

            
            employeeDto.IsFirstLogin = false;

            
            var updatedEmployee = await _employeeService.UpdateEmployeeAsync(employeeNo, employeeDto);

            if (updatedEmployee == null)
            {
                _logger.LogError($"Failed to update employee {employeeNo} after password change.");
                return StatusCode(500, "Error updating employee details.");
            }

            _logger.LogInformation($"Password change successful for EmployeeNo: {employeeNo}");
            return Ok(new { Message = "Password changed successfully" });
        }
    }
}

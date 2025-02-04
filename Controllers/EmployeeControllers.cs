using EmployeeTimeTrackingBackend.Models;
using EmployeeTimeTrackingBackend.Services;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
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
            try
            {
                var employees = await _employeeService.GetAllEmployeesAsync();

                if (employees == null || !employees.Any())
                {
                    return NotFound(new { Message = "No employees found." });
                }

                return Ok(employees);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error retrieving employees: {ex.Message}");
                return StatusCode(500, new { Message = "An error occurred while retrieving employees." });
            }
        }

        // GET: api/employee/{id}
        [HttpGet("{employeeNo}")]
        public async Task<IActionResult> GetEmployeeById(int employeeNo)
        {
            try
            {
                if (employeeNo <= 0)
                {
                    return BadRequest(new { Message = "Invalid employee number provided." });
                }

                var employee = await _employeeService.GetEmployeeByIdAsync(employeeNo);

                if (employee == null)
                {
                    return NotFound(new { Message = $"Employee with EmployeeNo {employeeNo} not found." });
                }

                return Ok(employee);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error retrieving employee {employeeNo}: {ex.Message}");
                return StatusCode(500, new { Message = "An error occurred while retrieving the employee." });
            }
        }

        // POST: api/employee
        [HttpPost]
        public async Task<IActionResult> AddEmployee([FromBody] EmployeeDto employeeDto)
        {
            try
            {
                if (employeeDto == null)
                {
                    return BadRequest(new { Message = "Employee data is required." });
                }

                var newEmployee = await _employeeService.AddEmployeeAsync(employeeDto);

                return CreatedAtAction(nameof(GetEmployeeById), new { employeeNo = newEmployee.EmployeeNo }, newEmployee);
            }
            catch (ArgumentException ex)
            {
                _logger.LogWarning($"Validation error: {ex.Message}");
                return BadRequest(new { Message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError($"An unexpected error occurred while adding an employee: {ex.Message}");
                return StatusCode(500, new { Message = "An error occurred while processing your request." });
            }
        }

        // PUT: api/employee/{id}
        [HttpPut("{employeeNo}")]
        public async Task<IActionResult> UpdateEmployee(int employeeNo, [FromBody] EmployeeDto employeeDto)
        {
            try
            {
                if (employeeNo <= 0)
                {
                    return BadRequest(new { Message = "Invalid employee number provided." });
                }

                if (employeeDto == null)
                {
                    return BadRequest(new { Message = "Employee data is required." });
                }

                var updatedEmployee = await _employeeService.UpdateEmployeeAsync(employeeNo, employeeDto);

                if (updatedEmployee == null)
                {
                    return NotFound(new { Message = $"Employee with EmployeeNo {employeeNo} not found." });
                }

                return Ok(updatedEmployee);
            }
            catch (ArgumentException ex)
            {
                _logger.LogWarning($"Validation error while updating EmployeeNo {employeeNo}: {ex.Message}");
                return BadRequest(new { Message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError($"An unexpected error occurred while updating EmployeeNo {employeeNo}: {ex.Message}");
                return StatusCode(500, new { Message = "An error occurred while processing your request." });
            }
        }

        // DELETE: api/employee/{id}
        [HttpDelete("{employeeNo}")]
        public async Task<IActionResult> DeleteEmployee(int employeeNo)
        {
            try
            {
                if (employeeNo <= 0)
                {
                    return BadRequest(new { Message = "Invalid employee number provided." });
                }

                var success = await _employeeService.DeleteEmployeeAsync(employeeNo);
                if (!success)
                {
                    return NotFound(new { Message = $"Employee with EmployeeNo {employeeNo} not found." });
                }

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error deleting EmployeeNo {employeeNo}: {ex.Message}");
                return StatusCode(500, new { Message = "An error occurred while processing your request." });
            }
        }
    }
}

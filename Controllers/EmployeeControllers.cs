using EmployeeTimeTrackingBackend.Services;
using Microsoft.AspNetCore.Mvc;

namespace EmployeeTimeTrackingBackend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EmployeeController : ControllerBase
    {
        private readonly IEmployeeService _employeeService;

        public EmployeeController(IEmployeeService employeeService)
        {
            _employeeService = employeeService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllEmployees()
        {
            try
            {
                var employees = await _employeeService.GetAllEmployeesAsync();
                return Ok(employees);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = "An error occurred while fetching employees.", Error = ex.Message });
            }
        }

        [HttpGet("{employeeNo}")]
        public async Task<IActionResult> GetEmployee(int employeeNo)
        {
            try
            {
                var employee = await _employeeService.GetEmployeeByIdAsync(employeeNo);
                if (employee == null)
                {
                    return NotFound(new { Message = "Employee not found" });
                }
                return Ok(employee);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = "An error occurred while fetching the employee.", Error = ex.Message });
            }
        }

        [HttpPost]
        public async Task<IActionResult> AddEmployee([FromBody] EmployeeDto employeeDto)
        {
            try
            {
                var employee = await _employeeService.AddEmployeeAsync(employeeDto);
                return Ok(new { Message = "Employee added successfully", EmployeeNo = employee.EmployeeNo });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = "An error occurred while adding the employee.", Error = ex.Message });
            }
        }

        [HttpPut("{employeeNo}")]
        public async Task<IActionResult> UpdateEmployee(int employeeNo, [FromBody] EmployeeDto employeeDto)
        {
            try
            {
                
                if (string.IsNullOrEmpty(employeeDto.PlainPassword))
                {
                    employeeDto.PlainPassword = null;  
                }

                var employee = await _employeeService.UpdateEmployeeAsync(employeeNo, employeeDto);
                if (employee == null)
                {
                    return NotFound(new { Message = "Employee not found" });
                }
                return Ok(new { Message = "Employee updated successfully" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = "An error occurred while updating the employee.", Error = ex.Message });
            }
        }


        [HttpDelete("{employeeNo}")]
        public async Task<IActionResult> DeleteEmployee(int employeeNo)
        {
            try
            {
                var result = await _employeeService.DeleteEmployeeAsync(employeeNo);
                if (!result)
                {
                    return NotFound(new { Message = "Employee not found" });
                }
                return Ok(new { Message = "Employee deleted successfully" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = "An error occurred while deleting the employee.", Error = ex.Message });
            }
        }
    }
}

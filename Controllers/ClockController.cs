using EmployeeTimeTrackingBackend.Models;
using EmployeeTimeTrackingBackend.Services;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace EmployeeTimeTrackingBackend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ClockController : ControllerBase
    {
        private readonly IClockingService _clockingService;
        private readonly ILogger<ClockingService> _logger;

        public ClockController(IClockingService clockingService, ILogger<ClockingService> logger)
        {
            _clockingService = clockingService;
            _logger = logger;
        }

        // POST: api/employee/{employeeNo}/clock-in
        [HttpPost("{employeeNo}/clock-in")]
        public async Task<IActionResult> ClockIn(int employeeNo)
        {
            try
            {
                if (employeeNo <= 0)
                {
                    return BadRequest(new { Message = "Invalid employee number provided." });
                }

                var (success, message) = await _clockingService.ClockInAsync(employeeNo);

                if (!success)
                {
                    return message == "Employee not found."
                        ? NotFound(new { Message = message })
                        : BadRequest(new { Message = message });
                }

                return Ok(new { Message = message });
            }
            catch (Exception ex)
            {
                _logger.LogError($"Clock-in error for EmployeeNo {employeeNo}: {ex.Message}");
                return StatusCode(500, new { Message = "An error occurred while processing your request." });
            }
        }

        // POST: api/employee/{employeeNo}/clock-out
        [HttpPost("{employeeNo}/clock-out")]
        public async Task<IActionResult> ClockOut(int employeeNo)
        {
            try
            {
                if (employeeNo <= 0)
                {
                    return BadRequest(new { Message = "Invalid employee number provided." });
                }

                var success = await _clockingService.ClockOutAsync(employeeNo);
                if (!success)
                {
                    return NotFound(new { Message = "Employee not found or not clocked in." });
                }

                return Ok(new { Message = "Clocked out successfully." });
            }
            catch (Exception ex)
            {
                _logger.LogError($"Clock-out error for EmployeeNo {employeeNo}: {ex.Message}");
                return StatusCode(500, new { Message = "An error occurred while processing your request." });
            }
        }

        // GET: api/employee/{employeeNo}/clock-in-status
        // [HttpGet("{employeeNo}/clock-in-status")]
        // public async Task<IActionResult> GetClockInStatus(int employeeNo)
        // {
        //     try
        //     {
        //         if (employeeNo <= 0)
        //         {
        //             return BadRequest(new { Message = "Invalid employee number provided." });
        //         }
        //
        //         var status = await _clockingService.GetClockInStatusAsync(employeeNo);
        //         if (status == null)
        //         {
        //             return NotFound(new { Message = "Employee not found." });
        //         }
        //
        //         return Ok(new { ClockInStatus = status });
        //     }
        //     catch (Exception ex)
        //     {
        //         _logger.LogError($"Error retrieving clock-in status for EmployeeNo {employeeNo}: {ex.Message}");
        //         return StatusCode(500, new { Message = "An error occurred while processing your request." });
        //     }
        // }
    }
}

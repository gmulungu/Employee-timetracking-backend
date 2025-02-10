using System.Text;
using AutoMapper;
using EmployeeTimeTrackingBackend.Models;
using Microsoft.EntityFrameworkCore;



namespace EmployeeTimeTrackingBackend.Services
{
    public interface IClockingService
    {
        Task<(bool success, string message)> ClockInAsync(int employeeNo);
        Task<bool> ClockOutAsync(int employeeNo);
        Task<bool?> GetClockInStatusAsync(int employeeNo);
    }

    public class ClockingService : IClockingService
    {
        private readonly AppDbContext _context;
        private readonly ILogger<ClockingService> _logger;

        public ClockingService(AppDbContext context, ILogger<ClockingService> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<(bool success, string message)> ClockInAsync(int employeeNo)
        {
            var employee = await _context.Employees.FirstOrDefaultAsync(e => e.EmployeeNo == employeeNo);
            if (employee == null) return (false, "Employee not found.");

            if (employee.IsClockedIn ?? false)
            {
                return (false, "Already clocked in.");
            }

            employee.IsClockedIn = true;
            await _context.SaveChangesAsync();

            return (true, "Clocked in successfully.");
        }

        public async Task<bool> ClockOutAsync(int employeeNo)
        {
            var employee = await _context.Employees.FirstOrDefaultAsync(e => e.EmployeeNo == employeeNo);
            if (employee == null) return false;

            if (!(employee.IsClockedIn ?? false))
            {
                return false;
            }

            employee.IsClockedIn = false;
            await _context.SaveChangesAsync();
            return true;
        }
        
        public async Task<bool?> GetClockInStatusAsync(int employeeNo)
        {
            try
            {
                if (employeeNo <= 0)
                {
                    throw new ArgumentException("Invalid employee number provided.", nameof(employeeNo));
                }

                
                var employee = await _context.Employees
                    .Where(e => e.EmployeeNo == employeeNo)
                    .Select(e => e.IsClockedIn)
                    .FirstOrDefaultAsync();

               
                return employee;
            }
            catch (ArgumentException ex)
            {
                _logger.LogWarning($"Validation error: {ex.Message}");
                return null; 
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error retrieving clock-in status for EmployeeNo {employeeNo}: {ex.Message}");
                return null; 
            }
        }


    }
}
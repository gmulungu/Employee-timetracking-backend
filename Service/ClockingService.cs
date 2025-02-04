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
    }
}
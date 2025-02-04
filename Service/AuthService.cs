using System.Text;
using AutoMapper;
using EmployeeTimeTrackingBackend.Models;
using Microsoft.EntityFrameworkCore;



namespace EmployeeTimeTrackingBackend.Services
{
    public interface IAuthService
    {
        Task<Employee> LoginAsync(int employeeNo, string password);
        Task<EmployeeDto> ChangePasswordAsync(int employeeNo, ChangePasswordDto changePasswordDto);
    }

    public class AuthService : IAuthService
    {
        private readonly AppDbContext _context;
        private readonly ILogger<AuthService> _logger;

        public AuthService(AppDbContext context, ILogger<AuthService> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<Employee> LoginAsync(int employeeNo, string password)
        {
            try
            {
                var employee = await _context.Employees.FirstOrDefaultAsync(e => e.EmployeeNo == employeeNo);
                if (employee == null) return null;

                if (!BCrypt.Net.BCrypt.Verify(password, employee.PasswordHash))
                {
                    _logger.LogWarning($"Login failed: Incorrect password for EmployeeNo {employeeNo}.");
                    return null;
                }

                return employee;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error occurred while logging in EmployeeNo {employeeNo}: {ex.Message}");
                return null;
            }
        }

        public async Task<EmployeeDto> ChangePasswordAsync(int employeeNo, ChangePasswordDto changePasswordDto)
        {
            try
            {
                var employee = await _context.Employees.FirstOrDefaultAsync(e => e.EmployeeNo == employeeNo);
                if (employee == null) return null;

                var newPasswordHash = HashPassword(changePasswordDto.NewPassword);
                employee.PasswordHash = newPasswordHash;
                await _context.SaveChangesAsync();

                return new EmployeeDto { EmployeeNo = employee.EmployeeNo };
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error occurred while changing password for EmployeeNo {employeeNo}: {ex.Message}");
                return null;
            }
        }


        private string HashPassword(string password)
        {
            return BCrypt.Net.BCrypt.HashPassword(password);
        }
    }
}
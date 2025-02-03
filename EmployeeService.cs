using System.Text;
using AutoMapper;
using EmployeeTimeTrackingBackend.Models;
using Microsoft.EntityFrameworkCore;

namespace EmployeeTimeTrackingBackend.Services
{
    public interface IEmployeeService
    {
        Task<IEnumerable<EmployeeDto>> GetAllEmployeesAsync();
        Task<EmployeeDto> GetEmployeeByIdAsync(int employeeNo);
        Task<EmployeeDto> AddEmployeeAsync(EmployeeDto employeeDto);
        Task<EmployeeDto> UpdateEmployeeAsync(int employeeNo, EmployeeDto employeeDto);
        Task<bool> DeleteEmployeeAsync(int employeeNo);
        Task<Employee> LoginAsync(int employeeNo, string password);
        Task<(bool success, string message)> ClockInAsync(int employeeNo);
        Task<bool> ClockOutAsync(int employeeNo);
        Task<EmployeeDto> ChangePasswordAsync(int employeeNo, ChangePasswordDto changePasswordDto); 
        Task<bool> IsClockedInAsync(int employeeNo);
        
    }

    public class EmployeeService : IEmployeeService
    {
        private readonly AppDbContext _context;
        private readonly IMapper _mapper;
        private readonly ILogger<EmployeeService> _logger;

        public EmployeeService(AppDbContext context, IMapper mapper, ILogger<EmployeeService> logger)
        {
            _context = context;
            _mapper = mapper;
            _logger = logger;
        }

        // Get all employees
        public async Task<IEnumerable<EmployeeDto>> GetAllEmployeesAsync()
        {
            var employees = await _context.Employees.ToListAsync();
            return _mapper.Map<IEnumerable<EmployeeDto>>(employees);
        }

        // Get employee by ID
        public async Task<EmployeeDto> GetEmployeeByIdAsync(int employeeNo)
        {
            var employee = await _context.Employees.FirstOrDefaultAsync(e => e.EmployeeNo == employeeNo);
            if (employee == null) return null;
            return _mapper.Map<EmployeeDto>(employee);
        }

        // Add a new employee
        public async Task<EmployeeDto> AddEmployeeAsync(EmployeeDto employeeDto)
        {
            try
            {
                if (string.IsNullOrEmpty(employeeDto.PlainPassword))
                {
                    throw new ArgumentException("PlainPassword is required for new employee.");
                }

                // Create a new employee
                var newEmployee = _mapper.Map<Employee>(employeeDto);
                string plainPassword = employeeDto.PlainPassword ?? GenerateRandomPassword();
                string hashedPassword = HashPassword(plainPassword);
                newEmployee.PasswordHash = hashedPassword;

               

                _context.Employees.Add(newEmployee);
                await _context.SaveChangesAsync();

                Console.WriteLine($"Plain password for {employeeDto.EmployeeNo}: {plainPassword}");

                return _mapper.Map<EmployeeDto>(newEmployee);
            }
            catch (ArgumentException ex)
            {
                // Handle specific argument exceptions
                Console.WriteLine($"Argument error: {ex.Message}");
                throw;
            }
            catch (KeyNotFoundException ex)
            {
                // Handle case where manager doesn't exist
                Console.WriteLine($"Manager not found: {ex.Message}");
                throw;
            }
            catch (Exception ex)
            {
                // Handle general exceptions
                Console.WriteLine($"An error occurred while adding the employee: {ex.Message}");
                throw new Exception("An error occurred while adding the employee.", ex);
            }
        }


        // Update an existing employee
        public async Task<EmployeeDto> UpdateEmployeeAsync(int employeeNo, EmployeeDto employeeDto)
        {
            try
            {
                var employee = await _context.Employees.FirstOrDefaultAsync(e => e.EmployeeNo == employeeNo);
                if (employee == null)
                {
                    throw new KeyNotFoundException($"Employee with EmployeeNo {employeeNo} not found.");
                }

               

                // Handle password change
                if (!string.IsNullOrEmpty(employeeDto.PlainPassword))
                {
                    employee.PasswordHash = HashPassword(employeeDto.PlainPassword);
                }

                _mapper.Map(employeeDto, employee);
                await _context.SaveChangesAsync();

                return _mapper.Map<EmployeeDto>(employee);
            }
            catch (KeyNotFoundException ex)
            {
                // Handle specific case where employee or manager is not found
                Console.WriteLine($"Not found error: {ex.Message}");
                throw;
            }
            catch (ArgumentException ex)
            {
                // Handle any argument-related exceptions
                Console.WriteLine($"Argument error: {ex.Message}");
                throw;
            }
            catch (Exception ex)
            {
                // Handle general exceptions
                Console.WriteLine($"An error occurred while updating the employee: {ex.Message}");
                throw new Exception("An error occurred while updating the employee.", ex);
            }
        }


        // Delete an employee
        public async Task<bool> DeleteEmployeeAsync(int employeeNo)
        {
            var employee = await _context.Employees.FirstOrDefaultAsync(e => e.EmployeeNo == employeeNo);
            if (employee == null) return false;

            _context.Employees.Remove(employee);
            await _context.SaveChangesAsync();

            return true;
        }

        // Login logic
        public async Task<Employee> LoginAsync(int employeeNo, string password)
        {
            // Fetch the employee by username
            var employee = await _context.Employees.FirstOrDefaultAsync(e => e.EmployeeNo == employeeNo);
            if (employee == null) return null; 

            try
            {
                // Check if the password is valid
                if (!BCrypt.Net.BCrypt.Verify(password, employee.PasswordHash))
                {
                    return null; 
                }

                
                if (employee.IsFirstLogin)
                {
                    return employee; 
                }

                return employee; 
            }
            catch (BCrypt.Net.SaltParseException)
            {
                // Rehash and save the new password hash 
                string newHash = BCrypt.Net.BCrypt.HashPassword(password);
                employee.PasswordHash = newHash;
                // Save changes to the database
                await _context.SaveChangesAsync(); 

                return employee;
            }
        }
        

        // Clock-in logic
        public async Task<(bool success, string message)> ClockInAsync(int employeeNo)
        {
            var employee = await _context.Employees.FirstOrDefaultAsync(e => e.EmployeeNo == employeeNo);
            if (employee == null) return (false, "Employee not found.");

            if (employee.IsClockedIn ?? false) return (false, "You are already clocked in.");

            employee.IsClockedIn = true;
            _context.Employees.Update(employee);
            await _context.SaveChangesAsync();

            return (true, "Clocked in successfully.");
        }

        // Clock-out logic
        public async Task<bool> ClockOutAsync(int employeeNo)
        {
            var employee = await _context.Employees.FindAsync(employeeNo);
            if (employee == null || employee.IsClockedIn == false) return false;

            employee.IsClockedIn = false;
            await _context.SaveChangesAsync();
            return true;
        }

        // Check if clocked in
        public async Task<bool> IsClockedInAsync(int employeeNo)
        {
            var employee = await _context.Employees
                .FirstOrDefaultAsync(e => e.EmployeeNo == employeeNo);
            return employee?.IsClockedIn ?? false;
        }

        // Change password logic 
        public async Task<EmployeeDto> ChangePasswordAsync(int employeeNo, ChangePasswordDto changePasswordDto)
        {
            _logger.LogInformation($"Changing password for EmployeeNo: {employeeNo}");

            var employee = await _context.Employees
                .Where(e => e.EmployeeNo == employeeNo)
                .FirstOrDefaultAsync();

            if (employee == null)
            {
                _logger.LogError($"Employee with EmployeeNo {employeeNo} not found.");
                return null; // Employee not found
            }

            _logger.LogInformation($"Employee found. Current PasswordHash: {employee.PasswordHash}");

            
            var newPasswordHash = HashPassword(changePasswordDto.NewPassword);
            _logger.LogInformation($"New Password Hash: {newPasswordHash}");

            employee.PasswordHash = newPasswordHash;
          

            // Log before saving changes
            _logger.LogInformation("Saving changes to the database...");
    
            var changes = await _context.SaveChangesAsync();
            _logger.LogInformation($"Changes saved: {changes} rows affected.");

            // Log final state of the employee
            _logger.LogInformation($"Updated Employee - PasswordHash: {employee.PasswordHash}");

            return _mapper.Map<EmployeeDto>(employee);
        }




        // Hashing method
        private string HashPassword(string password)
        {
            return BCrypt.Net.BCrypt.HashPassword(password);
        }

        // Helper method to generate a random password (for demonstration purposes)
        private string GenerateRandomPassword()
        {
            const int passwordLength = 12;
            const string validChars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz1234567890!@#$%^&*()";
            var random = new Random();
            var password = new StringBuilder();

            for (int i = 0; i < passwordLength; i++)
            {
                password.Append(validChars[random.Next(validChars.Length)]);
            }

            return password.ToString();
        }
    }
}

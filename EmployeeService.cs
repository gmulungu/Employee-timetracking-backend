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
    }

    public class EmployeeService : IEmployeeService
    {
        private readonly AppDbContext _context;
        private readonly IMapper _mapper;

        public EmployeeService(AppDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<IEnumerable<EmployeeDto>> GetAllEmployeesAsync()
        {
            var employees = await _context.Employees.ToListAsync();
            return _mapper.Map<IEnumerable<EmployeeDto>>(employees);
        }

        public async Task<EmployeeDto> GetEmployeeByIdAsync(int employeeNo)
        {
            var employee = await _context.Employees.FirstOrDefaultAsync(e => e.EmployeeNo == employeeNo);
            if (employee == null) return null;
            return _mapper.Map<EmployeeDto>(employee);
        }

        public async Task<EmployeeDto> AddEmployeeAsync(EmployeeDto employeeDto)
        {
            // Ensure PlainPassword is provided for new employee
            if (string.IsNullOrEmpty(employeeDto.PlainPassword))
            {
                throw new ArgumentException("PlainPassword is required for new employee.");
            }

            var newEmployee = _mapper.Map<Employee>(employeeDto);

            // Generate random password if it's not provided
            string plainPassword = employeeDto.PlainPassword ?? GenerateRandomPassword();
            string hashedPassword = HashPassword(plainPassword);

            newEmployee.PasswordHash = hashedPassword;

            _context.Employees.Add(newEmployee);
            await _context.SaveChangesAsync();

            Console.WriteLine($"Plain password for {employeeDto.EmployeeNo}: {plainPassword}");

            return _mapper.Map<EmployeeDto>(newEmployee);
        }

        public async Task<EmployeeDto> UpdateEmployeeAsync(int employeeNo, EmployeeDto employeeDto)
        {
            var employee = await _context.Employees.FirstOrDefaultAsync(e => e.EmployeeNo == employeeNo);
            if (employee == null) return null;

            // // Only update password if PlainPassword is provided
            if (!string.IsNullOrEmpty(employeeDto.PlainPassword))
            {
                employee.PasswordHash = HashPassword(employeeDto.PlainPassword);
            }

            // Map the other fields
            _mapper.Map(employeeDto, employee);
            await _context.SaveChangesAsync();

            return _mapper.Map<EmployeeDto>(employee);
        }

        public async Task<bool> DeleteEmployeeAsync(int employeeNo)
        {
            var employee = await _context.Employees.FirstOrDefaultAsync(e => e.EmployeeNo == employeeNo);
            if (employee == null) return false;

            _context.Employees.Remove(employee);
            await _context.SaveChangesAsync();

            return true;
        }
//refering to the comment from PR; resolving the issue of a password autogenerating
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

        private string HashPassword(string plainPassword)
        {
            return BCrypt.Net.BCrypt.HashPassword(plainPassword);
        }
    }
}

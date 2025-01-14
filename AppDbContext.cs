using Microsoft.EntityFrameworkCore;
using EmployeeTimeTrackingBackend.Models;

namespace EmployeeTimeTrackingBackend
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<Employee> Employees { get; set; }
    }
}
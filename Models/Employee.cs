using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EmployeeTimeTrackingBackend.Models
{
    
    public class Employee
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)] // Auto-generate EmployeeNo
        public int EmployeeNo { get; set; }
        //required

        [Required]
        public string FirstName { get; set; }
        //required

        [Required]
        public string LastName { get; set; }

        [Required]
        public string Username { get; set; }

        [Required]
        public string CellPhoneNumber { get; set; }

        [Required]
        public string Position { get; set; }

        public bool? IsManager { get; set; }  
        public bool IsDisabled { get; set; }

        [Required]
        public string PasswordHash { get; set; }  

        public int? ManagerId { get; set; }  

      
    }
}
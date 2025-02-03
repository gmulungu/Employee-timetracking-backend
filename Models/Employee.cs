using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EmployeeTimeTrackingBackend.Models
{
    public class Employee
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)] 
        public int EmployeeNo { get; set; }

        [Required]
        public string FirstName { get; set; }

        [Required]
        public string LastName { get; set; }

        // [Required]
         // public string Username { get; set; }

        [Required]
        public string CellPhoneNumber { get; set; }

        [Required]
        public string Position { get; set; }

        public bool? IsManager { get; set; }  
        public bool IsDisabled { get; set; }

        public string PasswordHash { get; set; }  

        public int? ManagerId { get; set; }  

        
        public bool ?IsClockedIn { get; set; }  

        
        public bool IsFirstLogin { get; set; }  
        
        public DateTime? LastPasswordChangeDate { get; set; } 
        public bool PasswordNeedsChange { get; set; }
        


    }
}
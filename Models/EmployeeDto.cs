public class EmployeeDto
{
    public int EmployeeNo { get; set; }

    public string FirstName { get; set; }

    public string LastName { get; set; }

    public string Username { get; set; }

    public string CellPhoneNumber { get; set; }

    public string Position { get; set; }

    public bool? IsManager { get; set; }

    public bool IsDisabled { get; set; }  

    public string PasswordHash { get; set; }

    public int? ManagerId { get; set; }

    public bool IsFirstLogin { get; set; }  

    public string? PlainPassword { get; set; }

    public bool IsClockedIn { get; set; }  
    public string? NewPassword { get; set; }
    


}


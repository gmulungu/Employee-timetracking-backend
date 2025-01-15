using AutoMapper;
using EmployeeTimeTrackingBackend.Models;

namespace EmployeeTimeTrackingBackend
{
    public class EmployeeProfile : Profile
    {
        public EmployeeProfile()
        {
            CreateMap<Employee, EmployeeDto>().ReverseMap();
        }
    }
}
using Application.Dtos;
using AutoMapper;
using Domain.Concrete;

namespace Application.Mappings
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<CreateDepartmentDto, Department>().ReverseMap();
            CreateMap<UpdateDepartmentDto, Department>()
                .ForMember(d => d.Id, opt => opt.Ignore())
                .ReverseMap();
            CreateMap<Department, ResultDepartmentDto>().ReverseMap();

            CreateMap<CreateEmployeeDto, Employee>().ReverseMap();
            CreateMap<UpdateEmployeeDto, Employee>().ReverseMap();
            CreateMap<Employee, ResultEmployeeDto>()
                            .ForMember(d => d.DepartmentName, opt => opt.MapFrom(s => s.Department != null ? s.Department.Name : null));


        }
    }
}

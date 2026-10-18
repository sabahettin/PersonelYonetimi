using Application.Mappings;
using Application.Validators;
using Application.Validators.Department;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;

namespace Application.Registration
{
    public static class ServiceRegistration
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services, string? autoMapperLicenseKey = null)
        {
            services.AddAutoMapper(cfg =>
            {
                cfg.LicenseKey = autoMapperLicenseKey;
            }, typeof(MappingProfile));

            services.AddValidatorsFromAssemblyContaining<CreateDepartmentValidator>();
            services.AddValidatorsFromAssemblyContaining<UpdateDepartmentValidator>();
            services.AddValidatorsFromAssemblyContaining<CreateEmployeeValidator>();
            services.AddValidatorsFromAssemblyContaining<UpdateEmployeeValidator>();

            return services;
        }
    }
}

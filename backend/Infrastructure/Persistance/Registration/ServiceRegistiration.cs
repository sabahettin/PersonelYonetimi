using Application.Repositories.Abstract;
using Application.Repositories.Concrete;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Persistance.Context;
using Persistance.Repositories.Abstract;
using Persistance.Repositories.Concrete;

namespace Persistance.Registration
{
    public static class ServiceRegistration
    {
        public static IServiceCollection AddPersistanceServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseNpgsql(configuration.GetConnectionString("DefaultConnection")));

            services.AddScoped(typeof(IBaseRepository<>), typeof(BaseRepository<>));

            services.AddScoped<IDepartmentRepository, DepartmentRepository>();
            services.AddScoped<IEmployeeRepository, EmployeeRepository>();

            return services;
        }
    }
}

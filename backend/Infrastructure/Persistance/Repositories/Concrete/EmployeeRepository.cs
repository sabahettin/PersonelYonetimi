using Application.Repositories.Concrete;
using Domain.Concrete;
using Persistance.Context;
using Persistance.Repositories.Abstract;

namespace Persistance.Repositories.Concrete
{
    public class EmployeeRepository : BaseRepository<Employee>, IEmployeeRepository
    {
        public EmployeeRepository(ApplicationDbContext context) : base(context)
        {
        }
    }
}

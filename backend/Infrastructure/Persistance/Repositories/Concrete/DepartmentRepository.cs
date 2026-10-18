using Application.Repositories.Concrete;
using Domain.Concrete;
using Persistance.Context;
using Persistance.Repositories.Abstract;

namespace Persistance.Repositories.Concrete
{
    public class DepartmentRepository : BaseRepository<Department>, IDepartmentRepository
    {
        public DepartmentRepository(ApplicationDbContext context) : base(context)
        {
        }
    }
}

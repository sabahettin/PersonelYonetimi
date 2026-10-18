using Application.Repositories.Abstract;
using Domain.Concrete;

namespace Application.Repositories.Concrete
{
    public interface IEmployeeRepository : IBaseRepository<Employee>
    {
    }
}

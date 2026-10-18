using Domain.Abstract;

namespace Domain.Concrete
{
    public class Department : BaseEntity
    {
        public required string Name { get; set; }
        public virtual List<Employee>? Employees { get; set; }
    }
}

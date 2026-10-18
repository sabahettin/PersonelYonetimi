using Domain.Abstract;

namespace Domain.Concrete
{
    public class Employee : BaseEntity
    {
        public required string Name { get; set; }
        public required string Surname { get; set; }
        public required string Email { get; set; }
        public Guid? DepartmentId { get; set; }
        public Department? Department { get; set; }
    }
}

namespace Application.Dtos
{
    public class UpdateEmployeeDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Surname { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public  Guid DepartmentId { get; set; }
    }
}

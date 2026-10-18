namespace Application.Dtos
{
    public class ResultEmployeeDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Surname { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public Guid DepartmentId { get; set; }
        public string? DepartmentName { get; set; }
    }
}

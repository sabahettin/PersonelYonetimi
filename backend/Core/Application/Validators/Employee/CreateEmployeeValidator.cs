using Application.Dtos;
using Application.Repositories.Concrete;
using FluentValidation;

namespace Application.Validators
{
    public class CreateEmployeeValidator : AbstractValidator<CreateEmployeeDto>
    {
        private readonly IEmployeeRepository _employeeRepository;
        private readonly IDepartmentRepository _departmentRepository;

        public CreateEmployeeValidator(IEmployeeRepository employeeRepository, IDepartmentRepository departmentRepository)
        {
            _employeeRepository = employeeRepository;
            _departmentRepository = departmentRepository;

            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("First name is required.")
                .MaximumLength(30).WithMessage("First name cannot exceed 30 characters.");

            RuleFor(x => x.Surname)
                .NotEmpty().WithMessage("Last name is required.")
                .MaximumLength(30).WithMessage("Last name cannot exceed 30 characters.");

            RuleFor(x => x.Email)
                .Cascade(CascadeMode.Stop)
                .NotEmpty().WithMessage("Email address is required.")
                .MaximumLength(75).WithMessage("E-mail cannot exceed 75 characters.")
                .EmailAddress().WithMessage("A valid email address is required.")
                .MustAsync(EmailDoesNotExist).WithMessage("This email address is already in use.");

            RuleFor(x => x.DepartmentId)
                .Cascade(CascadeMode.Stop)
                .NotEmpty().WithMessage("Department is required.")
                .MustAsync(DepartmentExist).WithMessage("The selected department does not exist.");
        }

        private async Task<bool> EmailDoesNotExist(string email, CancellationToken ct)
        {
            var existing = await _employeeRepository.GetByDefaultAsync(x => x.Email.ToLower() == email.ToLower(), tracking: false, ct);
            return existing is null;
        }

        private async Task<bool> DepartmentExist(Guid id, CancellationToken ct)
        {
            var department = await _departmentRepository.GetByIdAsync(id, tracking: false, ct);
            return department is not null;
        }
    }
}
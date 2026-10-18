using Application.Dtos;
using Application.Repositories.Concrete;
using FluentValidation;

namespace Application.Validators.Department
{
    public class CreateDepartmentValidator : AbstractValidator<CreateDepartmentDto>
    {
        private readonly IDepartmentRepository _departmentRepository;

        public CreateDepartmentValidator(IDepartmentRepository departmentRepository)
        {
            _departmentRepository = departmentRepository;

            RuleFor(x => x.Name)
                .Cascade(CascadeMode.Stop)
                .NotEmpty().WithMessage("Department name is required.")
                .Length(2, 100).WithMessage("Department name must be between 2 and 100 characters.")
                .MustAsync(BeUniqueName).WithMessage("A department with this name already exists.");
        }

        private async Task<bool> BeUniqueName(string name, CancellationToken ct)
        {
            var existing = await _departmentRepository.GetByDefaultAsync(
                x => x.Name == name.Trim(),
                tracking: false,
                ct);

            return existing is null;
        }
    }
}
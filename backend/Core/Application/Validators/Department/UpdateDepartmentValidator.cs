using Application.Dtos;
using Application.Repositories.Abstract;
using Application.Repositories.Concrete;
using FluentValidation;

namespace Application.Validators.Department
{
    public class UpdateDepartmentValidator : AbstractValidator<UpdateDepartmentDto>
    {
        private readonly IDepartmentRepository _departmentRepository;

        public UpdateDepartmentValidator(IDepartmentRepository departmentRepository)
        {
            _departmentRepository = departmentRepository;

            RuleFor(x => x.Id)
                .NotEmpty().WithMessage("Department id is required.");

            RuleFor(x => x.Name)
                .Cascade(CascadeMode.Stop)
                .NotEmpty().WithMessage("Department name is required.")
                .Length(2, 100).WithMessage("Department name must be between 2 and 100 characters.")
                .MustAsync(BeUniqueName).WithMessage("A department with this name already exists.");
        }

        private async Task<bool> BeUniqueName(UpdateDepartmentDto dto, string name, CancellationToken ct)
        {
            var existing = await _departmentRepository.GetByDefaultAsync(
                x => x.Name == name.Trim() && x.Id != dto.Id,
                tracking: false,
                ct);

            return existing is null;
        }
    }
}
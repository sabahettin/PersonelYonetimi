using Application.Dtos;
using Application.Repositories.Concrete;
using AutoMapper;
using Domain.Abstract;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.Mvc;

namespace WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DepartmentsController : ControllerBase
    {
        #region INJECTION
        private readonly IDepartmentRepository _departmentRepository;
        private readonly IMapper _mapper;
        private readonly IValidator<CreateDepartmentDto> _createValidator;
        private readonly IValidator<UpdateDepartmentDto> _updateValidator;

        public DepartmentsController(
            IDepartmentRepository departmentRepository,
            IMapper mapper,
            IValidator<CreateDepartmentDto> createValidator,
            IValidator<UpdateDepartmentDto> updateValidator)
        {
            _departmentRepository = departmentRepository;
            _mapper = mapper;
            _createValidator = createValidator;
            _updateValidator = updateValidator;
        }
        #endregion
        #region GET
        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] Status? status, CancellationToken ct)
        {
            return Ok(_mapper.Map<List<ResultDepartmentDto>>(await _departmentRepository.GetAllAsync()));
        }

        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetById(Guid id, CancellationToken ct)
        {
            var department = await _departmentRepository.GetByIdAsync(id, ct: ct);
            if (department is null)
                return NotFound();

            return Ok(_mapper.Map<ResultDepartmentDto>(department));
        }
        #endregion
        #region POST
        [HttpPost]
        public async Task<IActionResult> Create(CreateDepartmentDto dto, CancellationToken ct)
        {
            var validation = await _createValidator.ValidateAsync(dto, ct);
            if (!validation.IsValid)
                return ToValidationProblem(validation);

            var department = _mapper.Map<Domain.Concrete.Department>(dto);
            var created = await _departmentRepository.AddAsync(department, ct: ct);
            if (!created)
                return Problem("Department could not be created.");

            var result = _mapper.Map<ResultDepartmentDto>(department);
            return CreatedAtAction(nameof(GetById), new { id = department.Id }, result);
        }
        #endregion
        #region PUT
        [HttpPut("{id:guid}")]
        public async Task<IActionResult> Update(Guid id, UpdateDepartmentDto dto, CancellationToken ct)
        {
            if (id != dto.Id)
            {
                ModelState.AddModelError(nameof(dto.Id), "Route id and body id do not match.");
                return ToValidationProblem();
            }

            var validation = await _updateValidator.ValidateAsync(dto, ct);
            if (!validation.IsValid)
                return ToValidationProblem(validation);

            var department = await _departmentRepository.GetByIdAsync(id, ct: ct);
            if (department is null)
                return NotFound();

            _mapper.Map(dto, department);
            var updated = await _departmentRepository.UpdateAsync(department, ct);

            return updated ? NoContent() : Problem("Department could not be updated.");
        }
        #endregion
        #region DELETE
        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> Delete(Guid id, CancellationToken ct)
        {
            var department = await _departmentRepository.GetByIdAsync(id, ct: ct);
            if (department is null)
                return NotFound();

            var deleted = await _departmentRepository.DeleteAsync(department, ct);

            return deleted ? NoContent() : Problem("Department could not be deleted.");
        }
        #endregion
        #region VALIDATIONCHECK
        private IActionResult ToValidationProblem(ValidationResult validation)
        {
            foreach (var error in validation.Errors)
                ModelState.AddModelError(error.PropertyName, error.ErrorMessage);

            return ToValidationProblem();
        }

        private IActionResult ToValidationProblem()
        {
            var problem = ProblemDetailsFactory.CreateValidationProblemDetails(HttpContext, ModelState);
            return BadRequest(problem);
        }
        #endregion
    }
}
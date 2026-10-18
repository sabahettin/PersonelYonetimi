using Application.Dtos;
using Application.Repositories.Concrete;
using AutoMapper;
using Domain.Concrete;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EmployeesController : ControllerBase
    {
        #region INJECTION
        private readonly IDepartmentRepository _departmentRepository;
        private readonly IEmployeeRepository _employeeRepository;
        private readonly IMapper _mapper;
        private readonly IValidator<CreateEmployeeDto> _createValidator;
        private readonly IValidator<UpdateEmployeeDto> _updateValidator;

        public EmployeesController(
            IDepartmentRepository departmentRepository,
            IEmployeeRepository employeeRepository,
            IMapper mapper,
            IValidator<CreateEmployeeDto> createValidator,
            IValidator<UpdateEmployeeDto> updateValidator)
        {
            _departmentRepository = departmentRepository;
            _employeeRepository = employeeRepository;
            _mapper = mapper;
            _createValidator = createValidator;
            _updateValidator = updateValidator;
        }
        #endregion
        #region GET
        [HttpGet]
        public async Task<IActionResult> GetAll(CancellationToken ct)
        {
            var employees = await _employeeRepository.GetAllAsync(includeProperties: "Department", ct:ct);
            return Ok(_mapper.Map<List<ResultEmployeeDto>>(employees));
        }
        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetById(Guid id, CancellationToken ct)
        {
            var employee = await _employeeRepository.GetFirstOrDefaultAsync(
                            x => x.Id == id, includeProperties: "Department", tracking: false, ct: ct);
            if (employee is null)
                return NotFound();

            return Ok(_mapper.Map<ResultEmployeeDto>(employee));
        }
        #endregion
        #region POST
        [HttpPost]
        public async Task<IActionResult> Create(CreateEmployeeDto dto, CancellationToken ct)
        {
            var validation = await _createValidator.ValidateAsync(dto, ct);
            if (!validation.IsValid)
                return ToValidationProblem(validation);

            var employee = _mapper.Map<Employee>(dto);
            var created = await _employeeRepository.AddAsync(employee, ct: ct);
            if (!created)
                return Problem("Employee could not be created.");

            var result = _mapper.Map<ResultEmployeeDto>(employee);
            return CreatedAtAction(nameof(GetById), new { id = employee.Id }, result);
        }
        #endregion
        #region PUT
        [HttpPut("{id:guid}")]
        public async Task<IActionResult> Update(Guid id, UpdateEmployeeDto dto, CancellationToken ct)
        {
            if (id != dto.Id)
                return BadRequest("Route id and body id do not match.");

            var validation = await _updateValidator.ValidateAsync(dto, ct);
            if (!validation.IsValid)
                return ToValidationProblem(validation);

            var employee = await _employeeRepository.GetByIdAsync(id, ct: ct);
            if (employee is null)
                return NotFound();

            _mapper.Map(dto, employee);
            var updated = await _employeeRepository.UpdateAsync(employee, ct);

            return updated ? NoContent() : Problem("Employee could not be updated.");
        }
        #endregion
        #region DELETE
        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> Delete(Guid id, CancellationToken ct)
        {
            var employee = await _employeeRepository.GetByIdAsync(id, ct: ct);
            if (employee is null)
                return NotFound();

            var deleted = await _employeeRepository.DeleteAsync(employee, ct);

            return deleted ? NoContent() : Problem("Employee could not be deleted.");
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

using Microsoft.AspNetCore.Mvc;
using RRHH.Application.Interfaces.Services;
using RRHH.Domain.DTOs;

namespace RRHH.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class EmployeesController : ControllerBase
    {
        private readonly IEmployeeService _employeeService;

        public EmployeesController(IEmployeeService employeeService)
            => _employeeService = employeeService;

        // GET -=> api/employees
        [HttpGet]
        public async Task<ActionResult<IEnumerable<EmployeeDto>>> GetAll()
        {
            var employees = await _employeeService.GetAllAsync();
            return Ok(employees);
        }

        // GET => api/employees/paged?page=1&pageSize=10
        [HttpGet("paged")]
        public async Task<ActionResult<PagedResultDto<EmployeeDto>>> GetPaged(
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10)
        {
            if (page <= 0 || pageSize <= 0)
                return BadRequest("Los valores de paginación deben ser mayores a cero");

            var result = await _employeeService.GetPagedAsync(page, pageSize);
            return Ok(result);
        }

        // GET -=> api/employees/5
        [HttpGet("{id:int}")]
        public async Task<ActionResult<EmployeeDto>> GetById(int id)
        {
            var employee = await _employeeService.GetByIdAsync(id);

            if (employee is null)
                return NotFound();

            return Ok(employee);
        }

        // GET => api/employees/search?term=SALAS&page=1&pageSize=10
        [HttpGet("search")]
        public async Task<ActionResult<PagedResultDto<EmployeeDto>>> Search(
            [FromQuery] string? term,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10)
        {
            var result = await _employeeService.SearchAsync(term, page, pageSize);
            return Ok(result);
        }

        // POST -=> api/employees
        [HttpPost]
        public async Task<IActionResult> Create(EmployeeDto employeeDto)
        {
            try
            {
                await _employeeService.CreateAsync(employeeDto);
                return CreatedAtAction(nameof(GetById), new { id = employeeDto.Id }, employeeDto);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        // PUT -=> api/employees/5
        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(int id, EmployeeDto employeeDto)
        {
            if (id != employeeDto.Id)
                return BadRequest(new { message = "El ID no coincide" });

            try
            {
                await _employeeService.UpdateAsync(employeeDto);
                return NoContent();
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        // DELETE -=> api/employees/5
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            await _employeeService.DeleteAsync(id);
            return NoContent();
        }
    }
}

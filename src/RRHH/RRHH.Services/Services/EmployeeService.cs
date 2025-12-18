using RRHH.Application.Interfaces.Repositories;
using RRHH.Application.Interfaces.Services;
using RRHH.Domain.DTOs;
using RRHH.Domain.Entities;

namespace RRHH.Services.Services
{
    public class EmployeeService : IEmployeeService
    {
        private readonly IGenericRepository<Employee> _repository;

        public EmployeeService(IGenericRepository<Employee> repository)
            => _repository = repository;

        public async Task<IEnumerable<EmployeeDto>> GetAllAsync()
        {
            var employees = await _repository.GetAllAsync();
            return employees.Select(MapToDto);
        }

        public async Task<EmployeeDto?> GetByIdAsync(int id)
        {
            var employee = await _repository.GetByIdAsync(id);
            return employee is null ? null : MapToDto(employee);
        }

    public async Task CreateAsync(EmployeeDto employeeDto)
        {
            ValidateEmployee(employeeDto);

            var employee = MapToEntity(employeeDto);
            await _repository.AddAsync(employee);
            employeeDto.Id = employee.Id;
        }

        public async Task UpdateAsync(EmployeeDto employeeDto)
        {
            ValidateEmployee(employeeDto);

            var employee = MapToEntity(employeeDto);
            await _repository.UpdateAsync(employee);
        }

        public async Task DeleteAsync(int id)
        {
            await _repository.DeleteAsync(id);
        }

        // ======================
        // Validaciones
        // ======================
        private void ValidateEmployee(EmployeeDto dto)
        {
          
            if (dto.Salary <= 0)
            {
                throw new ArgumentException("El salario debe ser mayor a cero.");
            }

            if (dto.HireDate == default(DateTime) || dto.HireDate == DateTime.MinValue)
            {
                throw new ArgumentException("La fecha de contratación es obligatoria.");
            }

            
            if (string.IsNullOrWhiteSpace(dto.FullName))
            {
                throw new ArgumentException("El nombre completo es obligatorio.");
            }

            
            if (string.IsNullOrWhiteSpace(dto.Position))
            {
                throw new ArgumentException("El cargo es obligatorio.");
            }

            
            if (string.IsNullOrWhiteSpace(dto.Department))
            {
                throw new ArgumentException("El departamento es obligatorio.");
            }
        }

        // ======================
        // Mappers privados
        // ======================

        private static EmployeeDto MapToDto(Employee employee)
        {
            return new EmployeeDto
            {
                Id = employee.Id,
                FullName = employee.FullName,
                HireDate = employee.HireDate,
                Position = employee.Position,
                Salary = employee.Salary,
                Department = employee.Department
            };
        }

        private static Employee MapToEntity(EmployeeDto dto)
        {
            return new Employee
            {
                Id = dto.Id,
                FullName = dto.FullName,
                HireDate = dto.HireDate,
                Position = dto.Position,
                Salary = dto.Salary,
                Department = dto.Department
            };
        }

        public async Task<PagedResultDto<EmployeeDto>> GetPagedAsync(
            int page, 
            int pageSize)
        {
            var (employees, totalRecords) =
                await _repository.GetPagedAsync(page, pageSize);

            return new PagedResultDto<EmployeeDto>
            {
                Page = page,
                PageSize = pageSize,
                TotalRecords = totalRecords,
                Data = employees.Select(MapToDto)
            };
        }

        public async Task<PagedResultDto<EmployeeDto>> SearchAsync(
            string? searchTerm,
            int page,
            int pageSize)
        {
            if (string.IsNullOrWhiteSpace(searchTerm))
                return await GetPagedAsync(page, pageSize);

            searchTerm = searchTerm.Trim();

            bool isNumeric = int.TryParse(searchTerm, out int id);

            var (employees, totalRecords) =
                await _repository.SearchAsync(
                    e =>
                        e.FullName.Contains(searchTerm) ||
                        (isNumeric && e.Id == id),
                    page,
                    pageSize);

            return new PagedResultDto<EmployeeDto>
            {
                Page = page,
                PageSize = pageSize,
                TotalRecords = totalRecords,
                Data = employees.Select(MapToDto)
            };
        }
    }
}

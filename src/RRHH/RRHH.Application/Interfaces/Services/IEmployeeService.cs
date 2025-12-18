using RRHH.Domain.DTOs;

namespace RRHH.Application.Interfaces.Services
{
    public interface IEmployeeService
    {
        public Task<IEnumerable<EmployeeDto>> GetAllAsync();
        public Task<EmployeeDto?> GetByIdAsync(int id);
        public Task CreateAsync(EmployeeDto employeeDto);
        public Task UpdateAsync(EmployeeDto employeeDto);
        public Task DeleteAsync(int id);
        public Task<PagedResultDto<EmployeeDto>> GetPagedAsync(
            int page, 
            int pageSize);
        public Task<PagedResultDto<EmployeeDto>> SearchAsync(
            string? searchTerm,
            int page,
            int pageSize);
    }
}

using RRHH.Domain.Entities;

namespace RRHH.Application.Interfaces.Repositories
{
    public interface IEmployeeRepository : IGenericRepository<Employee>
    {
        public Task<IEnumerable<Employee>> SearchAsync(string? name, int? id);
    }
}

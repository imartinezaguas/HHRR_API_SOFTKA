using System.Linq.Expressions;

namespace RRHH.Application.Interfaces.Repositories
{
    public interface IGenericRepository<T> where T : class
    {
        public Task<IEnumerable<T>> GetAllAsync();
        public Task<T?> GetByIdAsync(int id);
        public Task AddAsync(T entity);
        public Task UpdateAsync(T entity);
        public Task DeleteAsync(int id);
        public Task<(IEnumerable<T> Data, int TotalRecords)> GetPagedAsync(
            int page, 
            int pageSize);
    public Task<(IEnumerable<T> Data, int TotalRecords)> SearchAsync(
        Expression<Func<T, bool>> predicate,
        int page,
        int pageSize);
    }
}

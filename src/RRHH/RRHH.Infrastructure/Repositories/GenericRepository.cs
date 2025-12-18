using Microsoft.EntityFrameworkCore;
using RRHH.Application.Interfaces.Repositories;
using RRHH.Infrastructure.Data;
using System.Linq.Expressions;

namespace RRHH.Infrastructure.Repositories
{
    public class GenericRepository<T> : IGenericRepository<T> where T : class
    {
        protected readonly RRHHDbContext _context;
        protected readonly DbSet<T> _dbSet;

        public GenericRepository(RRHHDbContext context)
            => (_context, _dbSet) = (context, context.Set<T>());

        public async Task<IEnumerable<T>> GetAllAsync()
        {
            return await _dbSet.ToListAsync();
        }

        public async Task<T?> GetByIdAsync(int id)
        {
            return await _dbSet.FindAsync(id);
        }

        public async Task AddAsync(T entity)
        {
            await _dbSet.AddAsync(entity);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(T entity)
        {
            _dbSet.Update(entity);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var entity = await GetByIdAsync(id);
            if (entity == null) return;

            _dbSet.Remove(entity);
            await _context.SaveChangesAsync();
        }

        public async Task<(IEnumerable<T> Data, int TotalRecords)>
            GetPagedAsync(int page, int pageSize)
        {
            var totalRecords = await _dbSet.CountAsync();

            var data = await _dbSet
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return (data, totalRecords);
        }

        public async Task<(IEnumerable<T> Data, int TotalRecords)> SearchAsync(
            Expression<Func<T, bool>> predicate,
            int page,
            int pageSize)
        {
            var query = _dbSet.Where(predicate);

            var totalRecords = await query.CountAsync();

            var data = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return (data, totalRecords);
        }
    }
}

using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RestaurantReservation.Db.Respositories
{
    public class GenericRepository<T> : IGenericRepository<T> where T : class
    {
        private readonly RestaurantReservationDbContext _context;
        private readonly DbSet<T> _dbSet;

        public GenericRepository(RestaurantReservationDbContext context)
        {
            _context = context;
            _dbSet = _context.Set<T>();
        }

        public async Task AddSync(T entity)
        {
            await _dbSet.AddAsync(entity);
        }

        public async Task AddRangeAsync(IEnumerable<T> entities)
        {
            await _dbSet.AddRangeAsync(entities);
        }

        public async Task<IEnumerable<T>> GetAllAsync()
        {
            return await _dbSet.AsNoTracking().ToListAsync();
        }

        public async Task<IEnumerable<T>> FindAsync(Func<T, bool> predicate)
        {
            return await Task.FromResult(_dbSet.AsNoTracking().Where(predicate).AsEnumerable());
        }

        public async Task<T?> GetByIdAsync(int id)
        {
            return await _dbSet.FindAsync(id);
        }

        public async Task<T> UpdateAsync(T entity)
        {
            _dbSet.Update(entity);
            return await Task.FromResult(entity);
        }

        public async Task DeleteAsync(T entity)
        {
            _dbSet.Remove(entity);
            await Task.CompletedTask;
        }

        public async Task DeleteRangeAsync(IEnumerable<T> entities)
        {
            _dbSet.RemoveRange(entities);
            await Task.CompletedTask;
        }

        public async Task<int> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync();
        }

    }
}

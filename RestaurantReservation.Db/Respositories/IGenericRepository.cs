using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RestaurantReservation.Db.Respositories
{
    public interface IGenericRepository<T> where T : class
    {
        Task AddSync(T entity);
        Task AddRangeAsync(IEnumerable<T> entities);

        Task<T?> GetByIdAsync(int id);
        Task<IEnumerable<T>> GetAllAsync();
        Task<IEnumerable<T>> FindAsync(Func<T, bool> predicate);


        Task<T> UpdateAsync(T entity);

        Task DeleteAsync(T entity);
        Task DeleteRangeAsync(IEnumerable<T> entities);

        Task<int> SaveChangesAsync();
    }
}

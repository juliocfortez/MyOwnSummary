using System.Linq.Expressions;

namespace MyOwnSummary_API.Repositories.IRepository
{
    public interface IRepository<T> where T : class
    {
        Task Create(T entity);
        Task<IEnumerable<T>> GetAll(Expression<Func<T, bool>>? filter = null);

        Task<T> Get(Expression<Func<T, bool>> filter = null, bool tracked = true);

        Task Remove(T entity);

        bool Contains(T entity);

        Task Save(); 
    }
}

using MyOwnSummary_API.Models;
using System.Linq.Expressions;

namespace MyOwnSummary_API.Repositories.IRepository
{
    public interface ICategoryRepository : IRepository<Category>
    {
        Task Update(Category category);

        IQueryable<Category> GetAllIncluding(params Expression<Func<Category, object>>[] includes);

        Task<IEnumerable<Category>> GetCategoriesByUser(int userId);
    }
}

using MyOwnSummary_API.Models;
using System.Linq.Expressions;

namespace MyOwnSummary_API.Repositories.IRepository
{
    public interface IUserRepository : IRepository<User>
    {
        Task Update(User user);

        IQueryable<User> GetAllIncluding(params Expression<Func<User, object>>[] includes);

        
    }
}

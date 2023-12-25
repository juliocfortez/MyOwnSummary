using MyOwnSummary_API.Models;
using System.Linq.Expressions;

namespace MyOwnSummary_API.Repositories.IRepository
{
    public interface IRoleRepository : IRepository<Role>
    {
        Task Update(Role role);

        IQueryable<Role> GetAllIncluding(params Expression<Func<Role, object>>[] includes);
    }
}

using Microsoft.EntityFrameworkCore;
using MyOwnSummary_API.Data;
using MyOwnSummary_API.Models;
using MyOwnSummary_API.Repositories.IRepository;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;

namespace MyOwnSummary_API.Repositories
{
    public class RoleRepository : Repository<Role>, IRoleRepository
    {
        private ApplicationDbContext _context;
        public RoleRepository(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task Update(Role role)
        {
            _context.Update<Role>(role);
            await Save();
        }


        public IQueryable<Role> GetAllIncluding(params Expression<Func<Role, object>>[] includes) 
        {
            IQueryable<Role> query = _context.Roles;
            foreach (var include in includes)
            {
                query = _context.Roles.Include(include);
            }
            return query;
        }
    }
}

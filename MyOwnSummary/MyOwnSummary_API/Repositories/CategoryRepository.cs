using Microsoft.EntityFrameworkCore;
using MyOwnSummary_API.Data;
using MyOwnSummary_API.Models;
using MyOwnSummary_API.Repositories.IRepository;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;

namespace MyOwnSummary_API.Repositories
{
    public class CategoryRepositoy : Repository<Category>, ICategoryRepository
    {
        private ApplicationDbContext _context;
        public CategoryRepositoy(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task Update(Category category)
        {
            _context.Update<Category>(category);
            await Save();
        }


        public IQueryable<Category> GetAllIncluding(params Expression<Func<Category, object>>[] includes) 
        {
            IQueryable<Category> query = _context.Categories;
            foreach (var include in includes)
            {
                query = _context.Categories.Include(include);
            }
            return query;
        }

        public async Task<IEnumerable<Category>> GetCategoriesByUser(int userId)
        {
            var categories = await _context.Notes.Where(x => x.UserId == userId).Include(x => x.Category).Select(x => x.Category).ToListAsync();
            return categories;
        }
    }
}

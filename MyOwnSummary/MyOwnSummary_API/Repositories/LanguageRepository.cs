using Microsoft.EntityFrameworkCore;
using MyOwnSummary_API.Data;
using MyOwnSummary_API.Models;
using MyOwnSummary_API.Repositories.IRepository;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;

namespace MyOwnSummary_API.Repositories
{
    public class LanguageRepository : Repository<Language>, ILanguageRepository
    {
        private ApplicationDbContext _context;
        public LanguageRepository(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task Update(Language language)
        {
            _context.Update<Language>(language);
            await Save();
        }


        public IQueryable<Language> GetAllIncluding(params Expression<Func<Language, object>>[] includes) 
        {
            IQueryable<Language> query = _context.Languages;
            foreach (var include in includes)
            {
                query = _context.Languages.Include(include);
            }
            return query;
        }
    }
}

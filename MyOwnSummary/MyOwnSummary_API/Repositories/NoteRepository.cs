using Microsoft.EntityFrameworkCore;
using MyOwnSummary_API.Data;
using MyOwnSummary_API.Models;
using MyOwnSummary_API.Repositories.IRepository;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;

namespace MyOwnSummary_API.Repositories
{
    public class NoteRepository : Repository<Note>, INoteRepository
    {
        private ApplicationDbContext _context;
        public NoteRepository(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task Update(Note note)
        {
            _context.Update<Note>(note);
            await Save();
        }


        public IQueryable<Note> GetAllIncluding(params Expression<Func<Note, object>>[] includes) 
        {
            IQueryable<Note> query = _context.Notes;
            foreach (var include in includes)
            {
                query = _context.Notes.Include(include);
            }
            return query;
        }
    }
}

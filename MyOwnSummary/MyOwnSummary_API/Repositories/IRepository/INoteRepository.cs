using MyOwnSummary_API.Models;
using System.Linq.Expressions;

namespace MyOwnSummary_API.Repositories.IRepository
{
    public interface INoteRepository : IRepository<Note>
    {
        Task Update(Note note);

        IQueryable<Note> GetAllIncluding(params Expression<Func<Note, object>>[] includes);
    }
}

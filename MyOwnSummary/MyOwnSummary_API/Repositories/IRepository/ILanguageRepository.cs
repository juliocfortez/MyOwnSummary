using MyOwnSummary_API.Models;
using System.Linq.Expressions;

namespace MyOwnSummary_API.Repositories.IRepository
{
    public interface ILanguageRepository : IRepository<Language>
    {
        Task Update(Language language);

        IQueryable<Language> GetAllIncluding(params Expression<Func<Language, object>>[] includes);
    }
}

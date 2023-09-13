using MyOwnSummary_API.Models;

namespace MyOwnSummary_API.Repositories.IRepository
{
    public interface IUserRepository : IRepository<User>
    {
        Task Update(User user);   
    }
}

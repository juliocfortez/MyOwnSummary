using Microsoft.AspNetCore.Mvc;

namespace MyOwnSummary_API.Models.Manager
{
    public interface ISessionManager
    {
        APIResponse IsAuthenticate(HttpContext context);

        Task<bool> IsAdmin(string userSessionId);
    }
}

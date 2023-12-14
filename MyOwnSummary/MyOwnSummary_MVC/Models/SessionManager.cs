using System.Security.Claims;

namespace MyOwnSummary_MVC.Models
{
    public class SessionManager:ISessionManager
    {
        public string Token;
        public SessionManager()
        {
            Token = string.Empty;
        }

        public string GetToken()
        {
            return this.Token;
        }

        public void SetToken(string token)
        {
            this.Token = token;
        }

    }
}

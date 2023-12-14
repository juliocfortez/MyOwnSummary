namespace MyOwnSummary_MVC.Models
{
    public interface ISessionManager
    {
        string GetToken();
        void SetToken(string token);
    }
}

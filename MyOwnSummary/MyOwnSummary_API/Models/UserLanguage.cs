namespace MyOwnSummary_API.Models
{
    public class UserLanguage
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int LanguageId { get; set; }
        public virtual User User { get; set; }
        public virtual Language Language { get; set; }
    }
}

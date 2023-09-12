using System.ComponentModel.DataAnnotations;

namespace MyOwnSummary_API.Models
{
    public class User
    {
        public int Id { get; set; }
        public int UserName { get; set; }
        public int Password { get; set; }

        public virtual ICollection<Note> Notes { get; set; }

        public virtual ICollection<UserLanguage> Languages { get; set;}
    }
}

using System.ComponentModel.DataAnnotations;

namespace MyOwnSummary_API.Models
{
    public class User
    {
        public int Id { get; set; }
        [Required]
        public string UserName { get; set; }
        [Required]
        public string Password { get; set; }

        public int RoleId { get; set; }

        public virtual Role Role { get; set; }

        public virtual ICollection<Note> Notes { get; set; }

        public virtual ICollection<UserLanguage> Languages { get; set;}
    }
}

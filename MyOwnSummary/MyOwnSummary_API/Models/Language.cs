using System.ComponentModel.DataAnnotations;

namespace MyOwnSummary_API.Models
{
    public class Language
    {
        public int Id { get; set; }
        [MaxLength(50)]
        public  string Name { get; set; }

        public virtual ICollection<UserLanguage> Users { get; set; }

        public virtual ICollection<Note> Notes { get; set; }
    }

    
   
}

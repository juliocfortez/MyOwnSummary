using System.ComponentModel.DataAnnotations;

namespace MyOwnSummary_API.Models
{
    public class Note
    {
        public int Id { get; set; } 
        public int UserId { get; set; }
        public int LanguageId { get; set; }
        public int CategoryId { get; set; }
        [MaxLength(500)]
        public string Description { get; set; }
        [MaxLength(50)]
        public string? Title { get; set; }
        public virtual Category Category { get; set; }
        public virtual User User { get; set; }
        public virtual Language Language { get; set; }
    }
}

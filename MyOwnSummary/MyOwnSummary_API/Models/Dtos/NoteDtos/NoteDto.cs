using System.ComponentModel.DataAnnotations;

namespace MyOwnSummary_API.Models.Dtos.NoteDtos
{
    public class NoteDto
    {
        [Required]
        public int Id { get; set; }
        [Required]
        public int UserId { get; set; }
        [Required]
        public int LanguageId { get; set; }
        [Required]
        public int CategoryId { get; set; }
        [Required]
        public string Description { get; set; }
        [MaxLength(50)]
        [Required]
        public string? Title { get; set; }
    }
}

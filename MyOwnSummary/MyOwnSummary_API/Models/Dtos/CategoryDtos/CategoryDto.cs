using System.ComponentModel.DataAnnotations;

namespace MyOwnSummary_API.Models.Dtos.CategoryDtos
{
    public class CategoryDto
    {
        [Required]
        public int Id { get; set; }
        [MaxLength(50)]
        [Required]
        public string Name { get; set; }
    }
}

using System.ComponentModel.DataAnnotations;

namespace MyOwnSummary_API.Models.Dtos.CategoryDtos
{
    public class CreateCategoryDto
    {
        [MaxLength(50)]
        [Required]
        public string Name { get; set; }
    }
}

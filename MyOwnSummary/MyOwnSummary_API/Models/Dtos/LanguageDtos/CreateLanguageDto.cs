using System.ComponentModel.DataAnnotations;

namespace MyOwnSummary_API.Models.Dtos.LanguageDtos
{
    public class CreateLanguageDto
    {
        [Required]
        public string Name { get; set; }
    }
}

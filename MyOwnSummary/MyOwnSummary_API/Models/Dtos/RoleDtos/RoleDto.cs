using System.ComponentModel.DataAnnotations;

namespace MyOwnSummary_API.Models.Dtos.RoleDtos
{
    public class RoleDto
    {
        [Required]
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }
    }
}

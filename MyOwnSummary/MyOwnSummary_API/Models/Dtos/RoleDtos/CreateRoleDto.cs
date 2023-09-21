using System.ComponentModel.DataAnnotations;

namespace MyOwnSummary_API.Models.Dtos.RoleDtos
{
    public class CreateRoleDto
    {
        [Required]
        public string Name { get; set; }
    }
}

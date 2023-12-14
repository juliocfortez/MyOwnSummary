using System.ComponentModel.DataAnnotations;

namespace MyOwnSummary_MVC.Models
{
    public class UserViewModel
    {
        [Required]
        public int Id { get; set; }
        [Required]
        public string UserName { get; set; }
        [Required]
        public string Password { get; set; }
    }
}

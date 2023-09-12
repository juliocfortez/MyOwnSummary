using System.ComponentModel.DataAnnotations;

namespace MyOwnSummary_API.Models.Dtos.UsersDto
{
    public class UserDto
    {
        public int Id { get; set; }
        public int UserName { get; set; }
        public int Password { get; set; }
    }
}

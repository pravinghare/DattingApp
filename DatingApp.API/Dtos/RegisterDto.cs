using System.ComponentModel.DataAnnotations;

namespace DatingApp.API.Dtos
{
    public class RegisterDto
    {
        [Required]
        public string username { get; set; }
        [Required]
        [MinLength(8)]
        public string password { get; set; }

    }
}
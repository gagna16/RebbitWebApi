using System.ComponentModel.DataAnnotations;

namespace RebbitWebApi.Dto
{
    public class RegistrationDto
    {
        [Required]
        public string Username { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        public string FullName { get; set; }

        [Required]
        [MinLength(6)]
        public string Password { get; set; }

    }
}

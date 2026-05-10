using System.ComponentModel.DataAnnotations;

namespace API.Entities
{
    public class RegisterEntity
    {
        [Required]
        public string DisplayName { get; set; } = string.Empty;
        [Required]
        public string Email { get; set; } = string.Empty;
        [Required]
        public string Password { get; set; } = string.Empty;
    }
}

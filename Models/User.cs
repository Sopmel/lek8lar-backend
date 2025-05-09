using System.ComponentModel.DataAnnotations;

namespace Lek8LarBackend.Models
{
    public class User
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public required string Username { get; set; }

        [Required]
        public required string PasswordHash { get; set; }

        [Required]

        public string Role { get; set; } = "User";
    }
}

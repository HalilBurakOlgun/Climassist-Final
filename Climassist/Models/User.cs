using System.ComponentModel.DataAnnotations;

namespace Climassist.Models
{
    public class User
    {
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        public string SurName { get; set; }

        [Required]
        public string Password { get; set; }

        [Required]
        public string Email { get; set; }
        public string Phone { get; set; }

        public string UserType { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;
    }
}
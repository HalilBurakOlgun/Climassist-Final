using System.ComponentModel.DataAnnotations;
namespace Climassist.Models
{
    public class Requests
    {
        public int Id { get; set; }
        [Required]
        [StringLength(100)]
        public string UserName { get; set; }
        [Required]
        [StringLength(100)]
        public string UserSurname { get; set; }
        [Required]
        [EmailAddress]
        public string Email { get; set; }
        [Required]
        public string ClientType { get; set; }
        [Required]
        [Phone]
        public string Phone { get; set; }
        [Required]
        public string RequestType { get; set; }
        public string SparePartType { get; set; }
        public string RecoveryTime { get; set; }
        public string UnitType { get; set; }
        public string Message { get; set; }
        public string Status { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;
    }
}
using System;

namespace Climassist.Models
{
    public class User
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string SurName { get; set; }
        public string Password { get; set; } // Şifreleri şifrelemek önemlidir
        public string Email { get; set; }
        public string UserType { get; set; } // Müşteri, personel, admin gibi
        public DateTime CreatedAt { get; set; } = DateTime.Now;
    }
}
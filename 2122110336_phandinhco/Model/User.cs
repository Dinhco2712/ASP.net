using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace _2122110336_phandinhco.Model
{
    public class User
    {
        [Key]
        public int id { get; set; }

        [Required]
        [MaxLength(100)]
        public string name { get; set; }

        [Required]
        [EmailAddress]
        public string email { get; set; }

        [Required]
        public string password { get; set; }

        public string role { get; set; } = "user";

        public DateTime createAt { get; set; } = DateTime.Now;

        public DateTime updateAt { get; set; } = DateTime.Now;
        // Xóa thuộc tính Role
        // public string Role { get; set; } = "user";
        public ICollection<UserRoles> UserRoles { get; set; }

    }
}
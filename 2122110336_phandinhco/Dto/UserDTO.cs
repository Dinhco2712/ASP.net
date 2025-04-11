namespace _2122110336_phandinhco.Dto
{
    public class UserDTO
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string Role { get; set; } = null!;
        public DateTime CreateAt { get; set; }
        public DateTime UpdateAt { get; set; }
    }
}

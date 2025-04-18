namespace _2122110336_phandinhco.Request
{
    public class RegisterRequest
    {
        public string Email { get; set; }
        public string Password { get; set; }
        public string Name { get; set; } // Ensure this property exists
        public List<int> Roles { get; set; }
    }

}

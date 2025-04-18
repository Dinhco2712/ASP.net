namespace _2122110336_phandinhco.Model
{
    public class Role
    {
        public int RoleId { get; set; }
        public string RoleName { get; set; }
        public ICollection<UserRoles> UserRoles { get; set; }
    }
}

namespace Stocky.Models
{
    public enum Role
    {
        User,
        Admin
    }

    public class User
    {
        public int Id { get; set; } 
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public Role Role { get; set; } = Role.User;

        public override string ToString() => $"[{Id}] {Email} ({Role})";

        public bool IsAdmin() => Role == Role.Admin;
    }
}

// ============================================================
// Models/User.cs
// Authentication model for Admin and Officer users
// ============================================================
namespace CriminalRecordMS.Models
{
    public class User
    {
        public string UserId { get; set; }
        public string Username { get; set; }
        public string PasswordHash { get; set; }
        public UserRole Role { get; set; }
        public string LinkedOfficerId { get; set; }  // Empty for Admin
        public bool IsActive { get; set; }
        public DateTime LastLogin { get; set; }

        public User(string userId, string username, string passwordHash,
                    UserRole role, string linkedOfficerId = "")
        {
            UserId = userId;
            Username = username;
            PasswordHash = passwordHash;
            Role = role;
            LinkedOfficerId = linkedOfficerId;
            IsActive = true;
        }

        public override string ToString()
            => $"[{UserId}] {Username} | Role: {Role}";
    }
}
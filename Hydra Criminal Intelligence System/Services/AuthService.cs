// ============================================================
// Services/AuthService.cs
// Handles authentication and session management
// ============================================================
using CriminalRecordMS.Models;
using CriminalRecordMS.Utilities;

namespace CriminalRecordMS.Services
{
    public class AuthService
    {
        private readonly DataStore _store;

        public AuthService(DataStore store)
        {
            _store = store;
        }

        public bool Login(string username, string password)
        {
            try
            {
                var user = _store.Users.Values
                    .FirstOrDefault(u => u.Username.Equals(username, StringComparison.OrdinalIgnoreCase));

                if (user == null)
                    throw new Exception("Username not found.");

                if (!user.IsActive)
                    throw new Exception("Account is deactivated.");

                if (!PasswordHelper.Verify(password, user.PasswordHash))
                    throw new Exception("Incorrect password.");

                _store.CurrentUser = user;
                user.LastLogin = DateTime.Now;
                ConsoleUI.PrintSuccess($"Welcome, {username}! Role: {user.Role}");
                return true;
            }
            catch (Exception ex)
            {
                ConsoleUI.PrintError(ex.Message);
                return false;
            }
        }

        public void Logout()
        {
            _store.CurrentUser = null;
            ConsoleUI.PrintSuccess("Logged out successfully.");
        }

        public bool IsAdmin() => _store.CurrentUser?.Role == UserRole.Admin;

        public bool HasAccess(UserRole required)
            => _store.IsLoggedIn && (_store.CurrentUser!.Role == UserRole.Admin
               || _store.CurrentUser.Role == required);

        public void ChangePassword(string oldPassword, string newPassword)
        {
            if (_store.CurrentUser == null) throw new Exception("Not logged in.");
            if (!PasswordHelper.Verify(oldPassword, _store.CurrentUser.PasswordHash))
                throw new Exception("Old password is incorrect.");
            if (newPassword.Length < 6)
                throw new Exception("New password must be at least 6 characters.");

            _store.CurrentUser.PasswordHash = PasswordHelper.Hash(newPassword);
            _store.SaveAll();
            ConsoleUI.PrintSuccess("Password changed successfully.");
        }
    }
}
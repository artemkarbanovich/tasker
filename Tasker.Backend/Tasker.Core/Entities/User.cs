using System.Security.Cryptography;
using System.Text;

namespace Tasker.Core.Entities;

public class User
{
    public User(string email, string username, string password)
    {
        Email = email.ToLower();
        Username = username.ToLower();
        Role = "User";

        using var hmac = new HMACSHA512();

        PasswordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));
        PasswordSalt = hmac.Key;
    }

    public User(string id, string email, string username, DateTime registrationDate, byte[] passwordHash, byte[] passwordSalt, string role)
    {
        Id = id;
        Email = email;
        Username = username;
        RegistrationDate = registrationDate;
        PasswordHash = passwordHash;
        PasswordSalt = passwordSalt;
        Role = role;
    }

    public string Id { get; private set; } = Guid.NewGuid().ToString();
    public string Email { get; private set; }
    public string NormalizedEmail => Email.ToUpper();
    public string Username { get; private set; }
    public string NormalizedUsername => Username.ToUpper();
    public DateTime RegistrationDate { get; private set; } = DateTime.UtcNow;
    public byte[] PasswordHash { get; private set; }
    public byte[] PasswordSalt { get; private set; }
    public string Role { get; private set; }

    public bool CheckPassword(string password)
    {
        using var hmac = new HMACSHA512(PasswordSalt);
        var computedHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));

        for (int i = 0; i < computedHash.Length; i++)
            if (computedHash[i] != PasswordHash[i])
                return false;

        return true;
    }
}

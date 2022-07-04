using System.Security.Cryptography;

namespace Tasker.Core.Entities;

public class RefreshToken
{
    public RefreshToken(string userId)
    {
        UserId = userId;
    }

    public RefreshToken(string id, string userId, string token, DateTime creationTime, DateTime expiryTime)
    {
        Id = id;
        UserId = userId;
        Token = token;
        CreationTime = creationTime;
        ExpiryTime = expiryTime;
    }

    public string Id { get; private set; } = Guid.NewGuid().ToString();
    public string UserId { get; private set; }
    public string Token { get; private set; } = GenerateToken();
    public DateTime CreationTime { get; private set; } = DateTime.UtcNow;
    public DateTime ExpiryTime { get; private set; } = DateTime.UtcNow.AddMonths(6);

    public void UpdateToken()
    {
        Token = GenerateToken();
        CreationTime = DateTime.UtcNow;
        ExpiryTime = DateTime.UtcNow.AddMonths(6);
    }

    private static string GenerateToken()
    {
        using var randomNumberGenerator = RandomNumberGenerator.Create();
        var randomNumber = new byte[32];

        randomNumberGenerator.GetBytes(randomNumber);
        return Convert.ToBase64String(randomNumber);
    }
}

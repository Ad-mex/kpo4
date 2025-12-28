using System.Security.Cryptography;
using System.Text;

namespace Common;

public class IdGenerator
{
    public static string Get()
    {
        var random = Guid.NewGuid().ToString() + DateTime.UtcNow.Ticks;
        var bytes = Encoding.UTF8.GetBytes(random);
        var hashBytes = SHA256.HashData(bytes);

        return Convert.ToHexString(hashBytes).ToLower();
    }
}
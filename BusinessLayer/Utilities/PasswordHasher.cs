using System.Security.Cryptography;

namespace BusinessLayer.Utilities;

public class PasswordHasher
{
    /// <summary>
    /// Verilen şifreyi hash'ler ve Base64 string olarak döner.
    /// </summary>
    /// <param name="password">Hash'lenecek şifre.</param>
    /// <returns>Hash'lenmiş şifrenin Base64 string hali.</returns>
    public static string HashPassword(string password)
    {
        byte[] salt = new byte[16];
        using (var rng = RandomNumberGenerator.Create())
        {
            rng.GetBytes(salt);
        }

        var pbkdf2 = new Rfc2898DeriveBytes(password, salt, 10000);
        byte[] hash = pbkdf2.GetBytes(20);

        byte[] hashBytes = new byte[36];
        Array.Copy(salt, 0, hashBytes, 0, 16);
        Array.Copy(hash, 0, hashBytes, 16, 20);

        return Convert.ToBase64String(hashBytes);
    }

    /// <summary>
    /// Verilen şifrenin hash'lenmiş şifre ile eşleşip eşleşmediğini doğrular.
    /// </summary>
    /// <param name="password">Doğrulanacak şifre.</param>
    /// <param name="hashedPassword">Hash'lenmiş şifre.</param>
    /// <returns>Şifreler eşleşirse true, aksi takdirde false döner.</returns>
    public static bool VerifyPassword(string password, string hashedPassword)
    {
        byte[] hashBytes = Convert.FromBase64String(hashedPassword);

        byte[] salt = new byte[16];
        Array.Copy(hashBytes, 0, salt, 0, 16);

        var pbkdf2 = new Rfc2898DeriveBytes(password, salt, 10000);
        byte[] hash = pbkdf2.GetBytes(20);

        for (int i = 0; i < 20; i++)
            if (hashBytes[i + 16] != hash[i])
                return false;

        return true;
    }
}

using System;
using System.Security.Cryptography;
using System.Text;

public class PasswordService
{
    private static readonly char[] Lowercase = "abcdefghijklmnopqrstuvwxyz".ToCharArray();
    private static readonly char[] Uppercase = "ABCDEFGHIJKLMNOPQRSTUVWXYZ".ToCharArray();
    private static readonly char[] Numbers   = "0123456789".ToCharArray();
    private static readonly char[] Specials  = "!@#$%^&*()-_=+[]{}".ToCharArray();
    private static readonly char[] AllChars  = Lowercase.Concat(Uppercase).Concat(Numbers).Concat(Specials).ToArray();

    public static string GenerateStrongPassword(int length = 16)
    {
        if (length < 4)
            throw new ArgumentException("Password length must be at least 4");

        var password = new char[length];
        using (var rng = RandomNumberGenerator.Create())
        {
            // Ensure at least one of each type
            password[0] = Lowercase[GetRandomIndex(rng, Lowercase.Length)];
            password[1] = Uppercase[GetRandomIndex(rng, Uppercase.Length)];
            password[2] = Numbers[GetRandomIndex(rng, Numbers.Length)];
            password[3] = Specials[GetRandomIndex(rng, Specials.Length)];

            // Fill the rest randomly
            for (int i = 4; i < length; i++)
            {
                password[i] = AllChars[GetRandomIndex(rng, AllChars.Length)];
            }

            // Shuffle the array to avoid predictable placement
            ShuffleArray(password, rng);
        }

        return new string(password);
    }

    private static int GetRandomIndex(RandomNumberGenerator rng, int max)
    {
        byte[] data = new byte[4];
        rng.GetBytes(data);
        return (int)(BitConverter.ToUInt32(data, 0) % max);
    }

    private static void ShuffleArray(char[] array, RandomNumberGenerator rng)
    {
        for (int i = array.Length - 1; i > 0; i--)
        {
            int j = GetRandomIndex(rng, i + 1);
            var temp = array[i];
            array[i] = array[j];
            array[j] = temp;
        }
    }
}

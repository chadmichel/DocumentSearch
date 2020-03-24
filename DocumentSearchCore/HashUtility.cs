using System.Security.Cryptography;
using System.Text;

namespace DocumentSearchCore
{
    public static class HashUtility
    {
        public static string Hash(string input)
        {
            using (SHA256 sha = SHA256.Create())
            {
                Encoding enc = Encoding.UTF8;
                return System.Convert.ToBase64String(sha.ComputeHash(enc.GetBytes(input)));
            }
        }
    }
}
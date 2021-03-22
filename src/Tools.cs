using System;
using System.Security.Cryptography;
using System.Text;

namespace Vault
{
    public class Tools
    {
        /// <summary>
        /// Generate a random string.
        /// </summary>
        /// <param name="length">Total length of string.</param>
        /// <returns>Random string.</returns>
        public static string GenerateRandomString(int length)
        {
            const string valid = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890";

            using var rng = new RNGCryptoServiceProvider();

            var buffer = new byte[sizeof(uint)];
            var res = new StringBuilder();

            while (length-- > 0)
            {
                rng.GetBytes(buffer);

                var num = BitConverter.ToUInt32(buffer, 0);

                res.Append(valid[(int) (num % (uint) valid.Length)]);
            }

            return res.ToString();
        }
    }
}
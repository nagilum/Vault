using System;
using System.Security.Cryptography;
using System.Text;

namespace Vault.Core
{
    public class Tools
    {
        /// <summary>
        /// Create a hash of the input text.
        /// </summary>
        /// <param name="input">Input text to create hash from.</param>
        /// <param name="algorithm">Which hash algorithm to use. Defaults to SHA1.</param>
        /// <returns>Hash.</returns>
        public static string CreateHash(string input, HashAlgorithm algorithm = null)
        {
            if (input == null)
            {
                throw new ArgumentNullException(
                    nameof(input),
                    "'input' param cannot be null.");
            }

            // Default to SHA1.
            algorithm ??= SHA1.Create();

            // Convert the input string to a byte array and compute the hash.
            var bytes = algorithm.ComputeHash(Encoding.UTF8.GetBytes(input));

            // Create a new Stringbuilder to collect the bytes
            // and create a string.
            var output = new StringBuilder();

            // Loop through each byte of the hashed data
            // and format each one as a hexadecimal string.
            for (var i = 0; i < bytes.Length; i++)
            {
                output.Append(i.ToString("x2"));
            }

            // Return the hexadecimal string.
            return output.ToString();
        }

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
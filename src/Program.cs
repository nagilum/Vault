using System;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Vault
{
    public class Program
    {
        /// <summary>
        /// Init all the things..
        /// </summary>
        /// <param name="args">Command-line arguments.</param>
        public static void Main(string[] args)
        {
            // Load config from disk.
            Config.Load();

            // Verify that we have a secret, create one if now.
            VerifySecret();

            // Init the host.
            Host.CreateDefaultBuilder(args)
                .ConfigureLogging(l =>
                {
                    l.ClearProviders();
                    l.AddConsole();
                })
                .ConfigureWebHostDefaults(b =>
                {
                    b.UseStartup<Startup>();
                })
                .Build()
                .Run();
        }

        /// <summary>
        /// Verify that we have a secret, create one if now.
        /// </summary>
        private static void VerifySecret()
        {
            if (Config.Get("secret") == null)
            {
                throw new Exception("Secret not found in config!");
            }
        }
    }
}
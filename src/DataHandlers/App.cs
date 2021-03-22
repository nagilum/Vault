using System;
using System.Threading.Tasks;
using Vault.Core;
using Vault.Database;
using Vault.Database.Tables;
using Vault.Payloads;

namespace Vault.DataHandlers
{
    public class App
    {
        /// <summary>
        /// Create a new app in the database.
        /// </summary>
        /// <param name="name">Name of the app.</param>
        /// <param name="user">Requesting user.</param>
        /// <returns>App key.</returns>
        public static async Task<HelperRegisterNewApp> CreateAsync(string name, string user)
        {
            try
            {
                var secret = Config.Get("secret");
                var key = Tools.GenerateRandomString(64);

                await using var db = new DatabaseContext();

                var entry = new AppDbEntry
                {
                    Created = DateTimeOffset.Now,
                    Updated = DateTimeOffset.Now,
                    Name = Cryptography.Symmetric.Encrypt(name, $"{secret}{key}")
                };

                await db.Apps.AddAsync(entry);
                await db.SaveChangesAsync();

                // Log the creation of the app.
                await AccessLog.CreateAsync(
                    db,
                    secret,
                    key,
                    user,
                    entry);

                return new HelperRegisterNewApp
                {
                    App = entry,
                    Key = key
                };
            }
            catch (Exception ex)
            {
                // Log.
                Log.LogException(ex);

                // We're giving a null-object back to indicate something went wrong!
                return null;
            }
        }
    }
}
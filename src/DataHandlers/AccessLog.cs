using System;
using System.Threading.Tasks;
using Vault.Core;
using Vault.Database;
using Vault.Database.Tables;

namespace Vault.DataHandlers
{
    public class AccessLog
    {
        public static async Task CreateForAppAsync(string key, string requestUser, AppDbEntry app)
        {
            try
            {
                var secret = Config.Get("secret");

                await using var db = new DatabaseContext();

                var entry = new AccessLogDbEntry
                {
                    Created = DateTimeOffset.Now,
                    RequestUser = Cryptography.Symmetric.Encrypt(requestUser, $"{secret}{key}"),
                    RequestSource = Cryptography.Symmetric.Encrypt($"Created app {app.GetName(key)}", $"{secret}{key}")
                };

                await db.AccessLogs.AddAsync(entry);
                await db.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                // TODO: Log to ILS
            }
        }
    }
}
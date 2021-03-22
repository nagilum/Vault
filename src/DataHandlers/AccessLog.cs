using System;
using System.Threading.Tasks;
using Vault.Core;
using Vault.Database;
using Vault.Database.Tables;

namespace Vault.DataHandlers
{
    public class AccessLog
    {
        /// <summary>
        /// Create a log entry for creating the app.
        /// </summary>
        /// <param name="db">Database context.</param>
        /// <param name="secret">Local secret.</param>
        /// <param name="key">Remote encryption key.</param>
        /// <param name="user">The requesting user.</param>
        /// <param name="app">The created app.</param>
        public static async Task CreateAsync(
            DatabaseContext db,
            string secret,
            string key,
            string user,
            AppDbEntry app)
        {
            try
            {
                db ??= new DatabaseContext();
                secret ??= Config.Get("secret");

                var fk = $"{secret}{key}";

                var entry = new AccessLogDbEntry
                {
                    Created = DateTimeOffset.Now,
                    RequestUser = Cryptography.Symmetric.Encrypt(user, fk),
                    RequestSource = Cryptography.Symmetric.Encrypt($"Created app {app.GetName(key)}", fk),
                    AccessType = "WRITE",
                    DataRowId = app.Id
                };

                await db.AccessLogs.AddAsync(entry);
                await db.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                Log.LogException(ex);
            }
        }

        /// <summary>
        /// Create a log entry for property access.
        /// </summary>
        /// <param name="db">Database context.</param>
        /// <param name="secret">Local secret.</param>
        /// <param name="key">Remote encryption key.</param>
        /// <param name="user">The requesting user.</param>
        /// <param name="source">The requesting source.</param>
        /// <param name="accessType">Type of access to data.</param>
        /// <param name="rowId">Id of db row.</param>
        public static async Task CreateAsync(
            DatabaseContext db,
            string secret,
            string key,
            string user,
            string source,
            string accessType,
            long rowId)
        {
            try
            {
                db ??= new DatabaseContext();
                secret ??= Config.Get("secret");

                var fk = $"{secret}{key}";

                var entry = new AccessLogDbEntry
                {
                    Created = DateTimeOffset.Now,
                    RequestUser = Cryptography.Symmetric.Encrypt(user, fk),
                    RequestSource = Cryptography.Symmetric.Encrypt(source, fk),
                    AccessType = accessType,
                    DataRowId = rowId
                };

                await db.AccessLogs.AddAsync(entry);
                await db.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                Log.LogException(ex);
            }
        }
    }
}
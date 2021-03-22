using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Vault.Core;
using Vault.Database;
using Vault.Database.Tables;

namespace Vault.DataHandlers
{
    public class DataStorage
    {
        /// <summary>
        /// Delete the data and mark a row as deleted.
        /// </summary>
        /// <param name="encryptionKey">The encryption key to use for encryption.</param>
        /// <param name="type">The type the property is connected to.</param>
        /// <param name="id">Id of the connected type.</param>
        /// <param name="key">Key of the property.</param>
        /// <param name="user">The requesting user.</param>
        /// <param name="source">The requesting source.</param>
        public static async Task DeleteAsync(string encryptionKey, string type, string id, string key, string user, string source)
        {
            try
            {
                var secret = Config.Get("secret");

                var typeHashed = Tools.CreateHash(type);
                var idHashed = Tools.CreateHash(id);
                var keyHashed = Tools.CreateHash(key);

                await using var db = new DatabaseContext();

                var dbApp = await GetApp(db, secret, encryptionKey);

                if (dbApp == null)
                {
                    throw new Exception("The app is missing!");
                }

                var entry = await db.Properties
                    .FirstOrDefaultAsync(n => !n.Deleted.HasValue &&
                                              n.AppId == dbApp.Id &&
                                              n.Type == typeHashed &&
                                              n.Ident == idHashed &&
                                              n.Key == keyHashed);

                if (entry == null)
                {
                    throw new Exception("Data with that type, id, and key could not be found.");
                }

                entry.Updated = DateTimeOffset.Now;
                entry.Deleted = DateTimeOffset.Now;
                entry.Value = " --- DATA DELETED --- ";

                await db.SaveChangesAsync();

                // Create log of entry.
                await AccessLog.CreateAsync(
                    db,
                    secret,
                    encryptionKey,
                    user,
                    source,
                    "DELETE",
                    entry.Id);
            }
            catch (Exception ex)
            {
                // TODO: Log to ILS
            }
        }

        /// <summary>
        /// Get a property value from db.
        /// </summary>
        /// <param name="encryptionKey">The encryption key to use for encryption.</param>
        /// <param name="type">The type the property is connected to.</param>
        /// <param name="id">Id of the connected type.</param>
        /// <param name="key">Key of the property.</param>
        /// <param name="user">The requesting user.</param>
        /// <param name="source">The requesting source.</param>
        /// <returns>Value from property.</returns>
        public static async Task<string> GetAsync(string encryptionKey, string type, string id, string key, string user, string source)
        {
            try
            {
                var secret = Config.Get("secret");

                var typeHashed = Tools.CreateHash(type);
                var idHashed = Tools.CreateHash(id);
                var keyHashed = Tools.CreateHash(key);

                await using var db = new DatabaseContext();

                var dbApp = await GetApp(db, secret, encryptionKey);

                if (dbApp == null)
                {
                    throw new Exception("The app is missing!");
                }

                var entry = await db.Properties
                    .FirstOrDefaultAsync(n => !n.Deleted.HasValue &&
                                              n.AppId == dbApp.Id &&
                                              n.Type == typeHashed &&
                                              n.Ident == idHashed &&
                                              n.Key == keyHashed);

                if (entry == null)
                {
                    throw new Exception("Data with that type, id, and key could not be found.");
                }

                // Create log of entry.
                await AccessLog.CreateAsync(
                    db,
                    secret,
                    encryptionKey,
                    user,
                    source,
                    "READ",
                    entry.Id);

                return Cryptography.Symmetric.Decrypt(
                    entry.Value,
                    $"{secret}{encryptionKey}");
            }
            catch (Exception ex)
            {
                // TODO: Log to ILS

                // Since it's an error, we just return null.
                return null;
            }
        }

        /// <summary>
        /// Save a property to db.
        /// </summary>
        /// <param name="encryptionKey">The encryption key to use for encryption.</param>
        /// <param name="type">The type the property is connected to.</param>
        /// <param name="id">Id of the connected type.</param>
        /// <param name="key">Key of the property.</param>
        /// <param name="value">Value of the property.</param>
        /// <param name="user">The requesting user.</param>
        /// <param name="source">The requesting source.</param>
        public static async Task SaveAsync(string encryptionKey, string type, string id, string key, object value, string user, string source)
        {
            try
            {
                var secret = Config.Get("secret");

                var typeHashed = Tools.CreateHash(type);
                var idHashed = Tools.CreateHash(id);
                var keyHashed = Tools.CreateHash(key);

                await using var db = new DatabaseContext();

                var dbApp = await GetApp(db, secret, encryptionKey);

                if (dbApp == null)
                {
                    throw new Exception("The app is missing!");
                }

                var entry = await db.Properties
                                .FirstOrDefaultAsync(n => !n.Deleted.HasValue &&
                                                          n.AppId == dbApp.Id &&
                                                          n.Type == typeHashed &&
                                                          n.Ident == idHashed &&
                                                          n.Key == keyHashed)
                            ?? new PropertyDbEntry
                            {
                                Created = DateTimeOffset.Now,
                                AppId = dbApp.Id,
                                Type = typeHashed,
                                Ident = idHashed,
                                Key = keyHashed
                            };

                entry.Updated = DateTimeOffset.Now;
                entry.Value = Cryptography.Symmetric.Encrypt(value.ToString(), $"{secret}{encryptionKey}");

                if (entry.Id == 0)
                {
                    await db.Properties.AddAsync(entry);
                }

                await db.SaveChangesAsync();

                // Create log of entry.
                await AccessLog.CreateAsync(
                    db,
                    secret,
                    encryptionKey,
                    user,
                    source,
                    "WRITE",
                    entry.Id);
            }
            catch (Exception ex)
            {
                // TODO: Log to ILS
            }
        }

        #region Helper functions

        /// <summary>
        /// Get the app from db matching given encryption key.
        /// </summary>
        /// <param name="db">Database context.</param>
        /// <param name="secret">Local secret.</param>
        /// <param name="key">Remote encryption key.</param>
        /// <returns>Database row.</returns>
        private static async Task<AppDbEntry> GetApp(DatabaseContext db, string secret, string key)
        {
            db ??= new DatabaseContext();
            secret ??= Config.Get("secret");

            var apps = await db.Apps
                .Where(n => !n.Deleted.HasValue)
                .ToListAsync();

            foreach (var app in apps)
            {
                try
                {
                    var name = app.GetName(key, secret);
                    var enc = Cryptography.Symmetric.Encrypt(name, $"{secret}{key}");

                    if (enc == app.Name)
                    {
                        return app;
                    }
                }
                catch
                {
                    //
                }
            }

            return null;
        }

        #endregion
    }
}
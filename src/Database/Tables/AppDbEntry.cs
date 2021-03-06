using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Vault.Core;

namespace Vault.Database.Tables
{
    [Table("Apps")]
    public class AppDbEntry
    {
        #region ORM

        [Key]
        [Column]
        public long Id { get; set; }

        [Column]
        public DateTimeOffset Created { get; set; }

        [Column]
        public DateTimeOffset Updated { get; set; }

        [Column]
        public DateTimeOffset? Deleted { get; set; }

        [Column]
        [MaxLength(128)]
        public string Name { get; set; }

        #endregion

        #region Instance functions

        /// <summary>
        /// Get the decrypted name.
        /// </summary>
        /// <param name="key">Partial key.</param>
        /// <param name="secret">Partial key.</param>
        /// <returns>Name.</returns>
        public string GetName(string key, string secret = null)
        {
            return Cryptography.Symmetric.Decrypt(this.Name, $"{secret ?? Config.Get("secret")}{key}");
        }

        #endregion
    }
}
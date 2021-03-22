using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Vault.Database.Tables
{
    [Table("Properties")]
    public class PropertyDbEntry
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
        public long AppId { get; set; }

        [Column]
        [MaxLength(128)]
        public string Type { get; set; }

        [Column]
        [MaxLength(128)]
        public string Ident { get; set; }

        [Column]
        [MaxLength(512)]
        public string Key { get; set; }

        [Column]
        public string Value { get; set; }

        #endregion
    }
}
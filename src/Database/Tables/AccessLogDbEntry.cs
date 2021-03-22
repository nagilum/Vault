using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Vault.Database.Tables
{
    [Table("AccessLogs")]
    public class AccessLogDbEntry
    {
        #region ORM

        [Key]
        [Column]
        public long Id { get; set; }

        [Column]
        public DateTimeOffset Created { get; set; }

        [Column]
        public string RequestUser { get; set; }

        [Column]
        public string RequestSource { get; set; }

        [Column]
        public long? DataRowId { get; set; }

        #endregion
    }
}
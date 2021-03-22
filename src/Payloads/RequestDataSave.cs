using System.Collections.Generic;

namespace Vault.Payloads
{
    public class RequestDataSave
    {
        /// <summary>
        /// List of properties to save.
        /// </summary>
        public Dictionary<string, object> Properties { get; set; }
    }
}
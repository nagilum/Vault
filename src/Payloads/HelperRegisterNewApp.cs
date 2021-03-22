using Vault.Database.Tables;

namespace Vault.Payloads
{
    public class HelperRegisterNewApp
    {
        /// <summary>
        /// The created app.
        /// </summary>
        public AppDbEntry App { get; set; }

        /// <summary>
        /// App-specific key.
        /// </summary>
        public string Key { get; set; }
    }
}
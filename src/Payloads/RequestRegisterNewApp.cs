namespace Vault.Payloads
{
    public class RequestRegisterNewApp
    {
        /// <summary>
        /// Name of the app.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// User who requested the registration.
        /// </summary>
        public string User { get; set; }
    }
}
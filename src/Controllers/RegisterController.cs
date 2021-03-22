using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Vault.Payloads;

namespace Vault.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RegisterController : ControllerBase
    {
        /// <summary>
        /// Register a new app.
        /// </summary>
        /// <param name="payload">App name and needed credentials.</param>
        /// <returns>App secret.</returns>
        [HttpPost]
        public async Task<ActionResult> RegisterNewApp(RequestRegisterNewApp payload)
        {
            // Verify payload.
            if (string.IsNullOrWhiteSpace(payload.Name))
            {
                return this.BadRequest(new
                {
                    message = "Parameter 'name' is required!"
                });
            }

            if (string.IsNullOrWhiteSpace(payload.User))
            {
                return this.BadRequest(new
                {
                    message = "Parameter 'user' is required!"
                });
            }

            // Create new app.
            var wrapper = await DataHandlers.App.CreateAsync(payload.Name, payload.User);

            if (wrapper == null)
            {
                return this.BadRequest(new
                {
                    message = "Something went wrong while creating new app!"
                });
            }

            // Return to client.
            return this.Ok(new
            {
                wrapper.Key
            });
        }
    }
}
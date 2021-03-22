using System.Net;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Vault.Attributes
{
    public class VerifyDataHeadersAttribute : ActionFilterAttribute
    {
        /// <summary>
        /// Validate the presence of two headers for user and source.
        /// </summary>
        /// <param name="context">Current executing context.</param>
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            if (!context.HttpContext.Request.Headers.ContainsKey("x-data-user"))
            {
                this.BadRequest(context, "Missing required header 'x-data-user'");
            }
            else if (!context.HttpContext.Request.Headers.ContainsKey("x-data-source"))
            {
                this.BadRequest(context, "Missing required header 'x-data-source'");
            }
            else if (!context.HttpContext.Request.Headers.ContainsKey("x-data-key"))
            {
                this.BadRequest(context, "Missing required header 'x-data-key'");
            }
        }

        /// <summary>
        /// Return a BadRequest response.
        /// </summary>
        /// <param name="context">Current executing context.</param>
        /// <param name="message">Message to include in response.</param>
        private void BadRequest(ActionExecutingContext context, string message = null)
        {
            context.Result = new ContentResult
            {
                Content = JsonSerializer.Serialize("{\"message\"=\"" + message + "\"}"),
                ContentType = "application/json"
            };

            context.HttpContext.Response.StatusCode = (int) HttpStatusCode.Unauthorized;
        }
    }
}
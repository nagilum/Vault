using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Vault.Attributes;
using Vault.DataHandlers;
using Vault.Payloads;

namespace Vault.Controllers
{
    [Route("api/[controller]/{type}/{id}")]
    [ApiController]
    [VerifyDataHeaders]
    public class DataController : ControllerBase
    {
        /// <summary>
        /// Delete the data and mark items as deleted.
        /// </summary>
        /// <param name="type">Type of object data is connected to.</param>
        /// <param name="id">Id of object.</param>
        /// <param name="keys">Set of property keys, comma separated.</param>
        /// <returns>Success.</returns>
        [HttpDelete]
        public async Task<ActionResult> Delete(string type, string id, [FromQuery] string keys)
        {
            var user = this.Request.Headers["x-data-user"].ToString();
            var source = this.Request.Headers["x-data-source"].ToString();
            var encKey = this.Request.Headers["x-data-key"].ToString();

            if (string.IsNullOrWhiteSpace(keys))
            {
                return this.BadRequest(new
                {
                    message = "The 'keys' query string is required."
                });
            }

            foreach (var key in keys.Split(','))
            {
                await DataStorage.DeleteAsync(encKey, type, id, key, user, source);
            }

            return this.Ok();
        }

        /// <summary>
        /// Get a list of values from the db.
        /// </summary>
        /// <param name="type">Type of object data is connected to.</param>
        /// <param name="id">Id of object.</param>
        /// <param name="keys">Set of property keys, comma separated.</param>
        /// <returns>List of keys and values.</returns>
        [HttpGet]
        public async Task<ActionResult> Get(string type, string id, [FromQuery] string keys)
        {
            var user = this.Request.Headers["x-data-user"].ToString();
            var source = this.Request.Headers["x-data-source"].ToString();
            var encKey = this.Request.Headers["x-data-key"].ToString();

            if (string.IsNullOrWhiteSpace(keys))
            {
                return this.BadRequest(new
                {
                    message = "The 'keys' query string is required."
                });
            }

            var res = new RequestDataSave
            {
                Properties = new Dictionary<string, object>()
            };

            foreach (var key in keys.Split(','))
            {
                res.Properties.Add(key, await DataStorage.GetAsync(encKey, type, id, key, user, source) ?? " -- DATA NOT FOUND --");
            }

            return this.Ok(res);
        }

        /// <summary>
        /// Save a list of properties to db.
        /// </summary>
        /// <param name="type">Type of object data is connected to.</param>
        /// <param name="id">Id of object.</param>
        /// <param name="payload">List of properties.</param>
        /// <returns>Success.</returns>
        [HttpPost]
        public async Task<ActionResult> Save(string type, string id, RequestDataSave payload)
        {
            var user = this.Request.Headers["x-data-user"].ToString();
            var source = this.Request.Headers["x-data-source"].ToString();
            var encKey = this.Request.Headers["x-data-key"].ToString();

            if (payload.Properties == null || 
                payload.Properties.Count == 0)
            {
                return this.BadRequest(new
                {
                    message = "'properties' is required!"
                });
            }

            if (payload.Properties.Any(n => n.Key.IndexOf(',') > -1))
            {
                return this.BadRequest(new
                {
                    message = "The property key cannot contain a comma (,)"
                });
            }

            foreach (var (key, value) in payload.Properties)
            {
                await DataStorage.SaveAsync(encKey, type, id, key, value, user, source);
            }

            return this.Ok();
        }
    }
}
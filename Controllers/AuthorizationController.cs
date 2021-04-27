using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Xero.NetStandard.OAuth2.Client;
using Xero.NetStandard.OAuth2.Config;
using Xero.NetStandard.OAuth2.Models;
using Xero.NetStandard.OAuth2.Token;

namespace xero_api_angular.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthorizationController : ControllerBase
    {
        private readonly ILogger<AuthorizationController> _logger;
        private readonly IOptions<XeroConfiguration> XeroConfig;
        private readonly IHttpClientFactory httpClientFactory;

        public AuthorizationController(IOptions<XeroConfiguration> XeroConfig, IHttpClientFactory httpClientFactory, ILogger<AuthorizationController> logger)
        {
            _logger = logger;
            this.XeroConfig = XeroConfig;
            this.httpClientFactory = httpClientFactory;
        }

        [Obsolete]
        [HttpGet]
        public async Task<IActionResult> Get()
        {
            try
            {
                return Ok();
            }
            catch (Exception ex)
            {
                return StatusCode((int)HttpStatusCode.InternalServerError, "error occured. ERROR: " + ex.Message);
            }
        }

        // GET /Authorization/Callback
        [HttpGet("Callback")]
        public async Task<IActionResult> Callback(string code, string state)
        {
            var client = new XeroClient(XeroConfig.Value, httpClientFactory);
            var xeroToken = (XeroOAuth2Token)await client.RequestXeroTokenAsync(code);

            List<Tenant> tenants = await client.GetConnectionsAsync(xeroToken);

            Tenant firstTenant = tenants[0];

            TokenUtilities.StoreToken(xeroToken);

            return Ok(xeroToken);
        }

        // GET /Authorization/Disconnect
        [HttpGet("Disconnect")]
        public async Task<IActionResult> Disconnect()
        {
            var client = new XeroClient(XeroConfig.Value, httpClientFactory);

            var xeroToken = TokenUtilities.GetStoredToken();
            var utcTimeNow = DateTime.UtcNow;

            if (utcTimeNow > xeroToken.ExpiresAtUtc)
            {
                xeroToken = (XeroOAuth2Token)await client.RefreshAccessTokenAsync(xeroToken);
                TokenUtilities.StoreToken(xeroToken);
            }

            string accessToken = xeroToken.AccessToken;
            Tenant xeroTenant = xeroToken.Tenants[0];

            await client.DeleteConnectionAsync(xeroToken, xeroTenant);

            TokenUtilities.DestroyToken();

            return Ok();
        }
    }
}

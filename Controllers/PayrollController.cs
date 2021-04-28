using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Xero.NetStandard.OAuth2.Api;
using Xero.NetStandard.OAuth2.Client;
using Xero.NetStandard.OAuth2.Config;
using Xero.NetStandard.OAuth2.Model.PayrollAu;
using Xero.NetStandard.OAuth2.Models;
using Xero.NetStandard.OAuth2.Token;

namespace xero_api_angular.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PayrollController : ControllerBase
    {
        private readonly ILogger<AuthorizationController> _logger;
        private readonly IOptions<XeroConfiguration> XeroConfig;
        private readonly IHttpClientFactory httpClientFactory;

        public PayrollController(IOptions<XeroConfiguration> XeroConfig, IHttpClientFactory httpClientFactory, ILogger<AuthorizationController> logger)
        {
            _logger = logger;
            this.XeroConfig = XeroConfig;
            this.httpClientFactory = httpClientFactory;
        }

        // GET: /Payroll/Timesheets
        [HttpGet("Timesheets")]
        public async Task<IActionResult> Timesheets(string TenantId)
        {
            var xeroToken = TokenUtilities.GetStoredToken(TenantId);
            var utcTimeNow = DateTime.UtcNow;

            if (utcTimeNow > xeroToken.ExpiresAtUtc)
            {
                var client = new XeroClient(XeroConfig.Value, httpClientFactory);
                xeroToken = (XeroOAuth2Token)await client.RefreshAccessTokenAsync(xeroToken);
                TokenUtilities.StoreToken(xeroToken);
            }

            string accessToken = xeroToken.AccessToken;
            TenantId = string.IsNullOrEmpty(TenantId) ? xeroToken.Tenants[0].TenantId.ToString() : TenantId;

            var PayrollAUApi = new PayrollAuApi();
            var response = await PayrollAUApi.GetTimesheetsAsync(accessToken, TenantId);

            var timesheet = response._Timesheets;

            return Ok(timesheet);
        }

        // GET: /Payroll/AddTimesheets
        [HttpPost("AddTimesheets")]
        public async Task<IActionResult> AddTimesheets(string TenantId, List<Timesheet> Timesheets)
        {
            var xeroToken = TokenUtilities.GetStoredToken(TenantId);
            var utcTimeNow = DateTime.UtcNow;

            if (utcTimeNow > xeroToken.ExpiresAtUtc)
            {
                var client = new XeroClient(XeroConfig.Value, httpClientFactory);
                xeroToken = (XeroOAuth2Token)await client.RefreshAccessTokenAsync(xeroToken);
                TokenUtilities.StoreToken(xeroToken);
            }

            string accessToken = xeroToken.AccessToken;
            TenantId = string.IsNullOrEmpty(TenantId) ? xeroToken.Tenants[0].TenantId.ToString() : TenantId;

            var TimesheetList = Timesheets?.Select(ts => new Timesheet
            {
                EmployeeID = ts.EmployeeID,
                StartDate = ts.StartDate,
                EndDate = ts.EndDate,
                Hours = ts.Hours,
                Status = ts.Status //TimesheetStatus.APPROVED
            }).ToList();

            //var TimesheetLines = new List<TimesheetLine>();
            //var _timesheet = new Timesheet()
            //{
            //    EmployeeID = new Guid(),
            //    //TimesheetID = new Guid(),
            //    StartDate = DateTime.Now,
            //    EndDate = DateTime.Now,
            //    Hours = 0,
            //    Status = TimesheetStatus.APPROVED,
            //    TimesheetLines = TimesheetLines
            //};

            var PayrollAUApi = new PayrollAuApi();
            var response = await PayrollAUApi.CreateTimesheetAsync(accessToken, TenantId, TimesheetList);

            var timesheet = response._Timesheets;

            return Ok(new { success = true, data = timesheet });
        }

    }
}

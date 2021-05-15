using Identity.API.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Identity.API.Controllers
{
    [Authorize]
    public class DiagnosticsController : Controller
    {
        public async Task<IActionResult> Index()
        {
            var localAddress = new string[] { "127.0.0.1", "::1", HttpContext.Connection.LocalIpAddress.ToString() };

            if (!localAddress.Contains(HttpContext.Connection.RemoteIpAddress.ToString()))
            {
                return NotFound();
            }
            var model = new DiagnosticsViewModel(await HttpContext.AuthenticateAsync());
            return View(model);
        }
    }
}

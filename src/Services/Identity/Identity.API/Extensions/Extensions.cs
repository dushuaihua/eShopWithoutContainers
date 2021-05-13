using Identity.API.Models;
using Microsoft.AspNetCore.Mvc;
using System;

namespace IdentityServer4.Models
{
    public static class AuthorizationRequestExtensions
    {
        public static bool IsNativeClient(this AuthorizationRequest request)
        {
            return !request.RedirectUri.StartsWith("https", StringComparison.Ordinal)
                && !request.RedirectUri.StartsWith("http", StringComparison.Ordinal);
        }

        public static IActionResult LoadingPage(this Controller controller, string viewName, string redirectUri)
        {
            controller.HttpContext.Response.StatusCode = 200;
            controller.HttpContext.Response.Headers["Location"] = "";

            return controller.View(viewName, new RedirectViewModel { RedirectUrl = redirectUri });
        }
    }
}

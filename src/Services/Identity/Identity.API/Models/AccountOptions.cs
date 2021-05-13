using System;

namespace Identity.API.Models
{
    public class AccountOptions
    {
        public static bool AllowLocalSignIn = true;
        public static bool AllRememberMe = true;
        public static TimeSpan RememberMeDuration = TimeSpan.FromDays(30);
        public static bool ShowSignOutPrompt = true;
        public static bool AutomaticRedirectAfterSignOut = false;
        public static string InvalidCredentialsErrorMessage = "Invalid username or password";
    }
}

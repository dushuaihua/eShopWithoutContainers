namespace Identity.API.Models
{
    public class SignedOutViewModel
    {
        public string PostSignOutRedirectUri { get; set; }
        public string ClientName { get; set; }
        public string SignOutIframeUrl { get; set; }
        public bool AutomaticRedirectAfterSignOut { get; set; }
        public string SignOutId { get; set; }
        public bool TriggerExternalSignOut => ExternalAuthenticationScheme is not null;
        public string ExternalAuthenticationScheme { get; set; }
    }
}
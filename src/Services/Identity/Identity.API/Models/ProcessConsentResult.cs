using IdentityServer4.Models;

namespace Identity.API.Models
{
    public class ProcessConsentResult
    {
        public bool IsRedirect => RedirectUri is not null;
        public string RedirectUri { get; set; }
        public Client Client { get; set; }
        public bool ShowView => ViewModel is not null;

        public ConsentViewModel ViewModel { get; set; }
        public bool HasValidationError => ValidationError is not null;
        public string ValidationError { get; set; }
    }
}

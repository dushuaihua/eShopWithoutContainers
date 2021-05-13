using System.Collections.Generic;
using System.Linq;

namespace Identity.API.Models
{
    public class SignInViewModel : SignInInputModel
    {
        public bool AllowRememberMe { get; set; } = true;
        public bool EnableLocalSignIn { get; set; } = true;
        public IEnumerable<ExternalProvider> ExternalProviders { get; set; } = Enumerable.Empty<ExternalProvider>();
        public IEnumerable<ExternalProvider> VisibleExternalProvider => ExternalProviders.Where(x => !string.IsNullOrWhiteSpace(x.DisplayName));

        public bool IsExternalSigninOnly => EnableLocalSignIn == false && ExternalProviders?.Count() == 1;
        public string ExternalSignInScheme => IsExternalSigninOnly ? ExternalProviders?.SingleOrDefault()?.AuthenticationScheme : null;
    }
}

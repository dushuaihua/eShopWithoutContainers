namespace Identity.API.Models
{
    public class SignOutViewModel : SignOutInputModel
    {
        public bool ShowSignOutPrompt { get; set; } = true;
    }
}

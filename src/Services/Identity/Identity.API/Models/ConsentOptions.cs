namespace Identity.API.Models
{
    public class ConsentOptions
    {
        public static bool EnableOfflineAccess = true;
        public static string OfflineAccessDisplayName = "Offine Access";
        public static string OfflineAccessDescrption = "Access to your application and resources,even when you are offline";
        public static readonly string MustChooseOneErrorMessage = "You must pick at least one permission";
        public static readonly string InvalidSelectionErrorMessage = "Invalid selection";
    }
}

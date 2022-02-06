namespace Identity.API.Models;
public class DiagnosticsViewModel
{
    public AuthenticateResult AuthenticateResult { get; }
    public IEnumerable<string> Clients { get; } = new List<string>();

    public DiagnosticsViewModel(AuthenticateResult result)
    {
        AuthenticateResult = result;

        if (result.Properties.Items.ContainsKey("client_list"))
        {
            var encoded = result.Properties.Items["client_list"];
            var bytes = Base64Url.Decode(encoded);
            var value = Encoding.UTF8.GetString(bytes);

            Clients = JsonConvert.DeserializeObject<string[]>(value);
        }
    }
}
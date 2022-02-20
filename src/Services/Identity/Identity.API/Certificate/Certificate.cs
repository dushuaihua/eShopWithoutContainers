namespace eShopWithoutContainers.Services.Identity.API.Certificates;

public class Certificate
{
    public static X509Certificate2 Get()
    {
        var assembly = typeof(Certificate).GetTypeInfo().Assembly;
        var name = assembly.GetManifestResourceNames().FirstOrDefault(x => x.EndsWith(".Certificate.idsrv3test.pfx"));
        using (var stream = assembly.GetManifestResourceStream(name))
        {
            return new X509Certificate2(ReadStream(stream), "idsrv3test");
        }
    }

    private static byte[] ReadStream(Stream input)
    {
        byte[] buffer = new byte[16 * 1024];
        using (MemoryStream ms = new MemoryStream())
        {
            int read;
            while ((read = input.Read(buffer, 0, buffer.Length)) > 0)
            {
                ms.Write(buffer, 0, read);
            }
            return ms.ToArray();
        }
    }
}

namespace eShopWithoutContainers.Services.Identity.API.Devspaces;

static class IdentityDevspacesBuilderExtensions
{
    public static IIdentityServerBuilder AddDevspacesIfNeed(this IIdentityServerBuilder builder, bool useDevspaces)
    {
        if (useDevspaces)
        {
            builder.AddRedirectUriValidator<DevspacesRedirectUriValidator>();
        }
        return builder;
    }
}

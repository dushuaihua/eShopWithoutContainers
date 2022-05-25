namespace eShopWithoutContainers.Services.Ordering.API.Infrastructure.Services;

public class IdentityService : IIdentityService
{
    private readonly IHttpContextAccessor _contextAccessor;

    public IdentityService(IHttpContextAccessor contextAccessor)
    {
        _contextAccessor = contextAccessor;
    }

    public string GetUserIdentity()
    {
        return _contextAccessor.HttpContext.User.FindFirst("sub").Value;
    }

    public string GetUserName()
    {
        return _contextAccessor.HttpContext.User.Identity.Name;
    }
}
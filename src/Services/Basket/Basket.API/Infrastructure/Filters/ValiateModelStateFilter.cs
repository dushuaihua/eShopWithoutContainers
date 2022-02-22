namespace eShopWithoutContainers.Services.Basket.API.Infrastructure.Filters;

public class ValiateModelStateFilter : ActionFilterAttribute
{
    public override void OnActionExecuting(ActionExecutingContext context)
    {
        if (context.ModelState.IsValid)
        {
            return;
        }

        var validationErrors = context.ModelState
            .Keys
            .SelectMany(k => context.ModelState[k].Errors)
            .Select(e => e.ErrorMessage)
            .ToArray();

        var json = new JsonErrorResponse
        {
            Messages = validationErrors
        };

        context.Result = new BadRequestObjectResult(json);
    }
}
﻿namespace eShopWithoutContainers.Services.Basket.API.Controllers;

public class HomeController : Controller
{
    public IActionResult Index()
    {
        return new RedirectResult("~/swagger");
    }
}
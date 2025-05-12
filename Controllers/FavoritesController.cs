using dotnet_store.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace dotnet_store.Controllers;

public class FavoritesController : Controller
{
    private readonly IFavoriService _favoriService;

    public FavoritesController(IFavoriService favoriService)
    {
        _favoriService = favoriService;
    }
    [Authorize]
    public async Task<IActionResult> Index()
    {
        var customerId = GetCustomerId();
        var favorites = await _favoriService.GetFavorites(customerId);
        return View(favorites);
    }
    [HttpPost]
    public async Task<IActionResult> AddToFavorites(int urunId)
    {
        var customerId = GetCustomerId();
        if (!string.IsNullOrEmpty(customerId))
        {
            await _favoriService.AddToFavorites(urunId, customerId);
        }
        return RedirectToAction("Index");
    }

    [HttpPost]
    public async Task<IActionResult> RemoveFromFavorites(int urunId)
    {
        var customerId = GetCustomerId();
        if (!string.IsNullOrEmpty(customerId))
        {
            await _favoriService.RemoveFromFavorites(urunId, customerId);
        }
        return RedirectToAction("Index");
    }

    private string GetCustomerId()
    {

        return HttpContext.User.Identity?.Name ?? HttpContext.Request.Cookies["customerId"]!;
    }
}

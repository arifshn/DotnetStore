using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using dotnet_store.Models;

[Authorize(Roles = "Admin")]
public class AdminController : Controller
{
    private readonly DataContext _context;

    public AdminController(DataContext context)
    {
        _context = context;
    }

    public IActionResult Index()
    {
        ViewBag.TotalSales = _context.Orders?.Any() == true ? _context.Orders.Sum(o => o.ToplamFiyat) : 0;
        ViewBag.OrderCount = _context.Orders?.Count() ?? 0;
        ViewBag.ProductCount = _context.Urunler?.Count() ?? 0;
        var orders = _context.Orders?.OrderByDescending(o => o.SiparisTarihi).Take(5).ToList() ?? new List<Order>();
        return View(orders);
    }
}
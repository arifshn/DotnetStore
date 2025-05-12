using dotnet_store.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using System.Threading.Tasks;

namespace dotnet_store.Controllers
{
    [Authorize]
    public class AddressController : Controller
    {
        private readonly DataContext _context;
        private readonly UserManager<AppUser> _userManager;

        public AddressController(DataContext context, UserManager<AppUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        [HttpGet]
        public IActionResult Create()
        {
            if (!User.Identity!.IsAuthenticated)
            {
                return RedirectToAction("Login", "Account");
            }

            return View(new Address());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Address address)
        {
            ModelState.Remove("UserId");

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null)
            {
                return Unauthorized();
            }

            if (ModelState.IsValid)
            {
                address.UserId = userId;

                _context.Add(address);
                await _context.SaveChangesAsync();
                return RedirectToAction("Index", "Home");
            }

            return View(address);
        }

        public async Task<IActionResult> Index()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return Unauthorized();
            }

            var addresses = await _context.Addresses
                .Where(a => a.UserId == user.Id.ToString())
                .ToListAsync();

            return View(addresses);
        }

        [HttpGet]
        public IActionResult Edit(int id)
        {
            if (!User.Identity!.IsAuthenticated)
            {
                return RedirectToAction("Login", "Account");
            }

            var address = _context.Addresses.Find(id);
            if (address == null)
            {
                return NotFound();
            }
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (address.UserId != userId)
            {
                return Unauthorized();
            }

            return View(address);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Address address)
        {
            if (!User.Identity!.IsAuthenticated)
            {
                return RedirectToAction("Login", "Account");
            }

            if (id != address.Id)
            {
                return BadRequest();
            }

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null || address.UserId != userId)
            {
                return Unauthorized();
            }

            ModelState.Remove("UserId");

            if (ModelState.IsValid)
            {
                try
                {
                    var existingAddress = _context.Addresses.Find(id);
                    if (existingAddress == null)
                    {
                        return NotFound();
                    }
                    existingAddress.AdresAd = address.AdresAd;
                    existingAddress.AdresSatiri = address.AdresSatiri;
                    existingAddress.Sehir = address.Sehir;
                    existingAddress.PostaKodu = address.PostaKodu;
                    existingAddress.Country = address.Country;
                    existingAddress.TelefonNumarasi = address.TelefonNumarasi;
                    existingAddress.UserId = userId;

                    _context.Update(existingAddress);
                    await _context.SaveChangesAsync();
                    return RedirectToAction("Index", "Home");
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", $"Hata: {ex.Message}");
                }
            }
            return View(address);
        }

        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            var address = await _context.Addresses.FindAsync(id);
            if (address == null)
            {
                return NotFound();
            }

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (address.UserId != userId)
            {
                return Forbid();
            }

            _context.Addresses.Remove(address);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
    }
}
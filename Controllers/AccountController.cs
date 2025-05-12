using System.Security.Claims;
using System.Threading.Tasks;
using dotnet_store.Models;
using dotnet_store.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
namespace dotnet_store.Controllers;

public class AccountController : Controller
{
    private UserManager<AppUser> _userManager;
    private SignInManager<AppUser> _signInManager;
    private readonly DataContext _context;
    private IEmailService _emailService;
    private readonly ICartService _cartService;
    public AccountController(UserManager<AppUser> userManager, SignInManager<AppUser> signInManager, IEmailService emailService, DataContext context, ICartService cartService)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _emailService = emailService;
        _context = context;
        _cartService = cartService;
    }
    public ActionResult Create()
    {
        return View();
    }

    [HttpPost]
    public async Task<ActionResult> Create(AccountCreateModel model)
    {
        if (ModelState.IsValid)
        {
            var user = new AppUser { UserName = model.Email, Email = model.Email, AdSoyad = model.AdSoyad };
            var result = await _userManager.CreateAsync(user, model.Password);
            if (result.Succeeded)
            {
                return RedirectToAction("Index", "Home");
            }
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error.Description);
            }
        }
        return View(model);
    }
    public ActionResult Login()
    {
        return View();
    }
    [HttpPost]
    public async Task<ActionResult> Login(AccountLoginModel model, string? returnUrl)
    {

        if (ModelState.IsValid)
        {
            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user != null)
            {
                await _signInManager.SignOutAsync();
                var result = await _signInManager.PasswordSignInAsync(user, model.Password, model.BeniHatirla, lockoutOnFailure: true);
                if (result.Succeeded)
                {
                    await _userManager.ResetAccessFailedCountAsync(user);
                    await _userManager.SetLockoutEndDateAsync(user, null);
                    await _cartService.TransferCartToUser(user.UserName!);

                    if (!string.IsNullOrEmpty(returnUrl))
                    {
                        return Redirect(returnUrl);
                    }
                    else
                    {
                        return RedirectToAction("Index", "Home");
                    }
                }
                else if (result.IsLockedOut)
                {
                    var lockoutDate = await _userManager.GetLockoutEndDateAsync(user);
                    var timeLeft = lockoutDate.Value - DateTime.UtcNow;
                    ModelState.AddModelError("", $"Hesabınız {timeLeft.Minutes + 1} dakika süreyle kilitlenmiştir.");
                }
                else
                {
                    ModelState.AddModelError("", "Geçersiz  parola.");
                }

            }
            else
            {
                ModelState.AddModelError("", "Geçersiz e-posta adresi veya parola.");
            }
        }
        return View(model);

    }


    [Authorize]
    public async Task<ActionResult> LogOut()
    {
        await _signInManager.SignOutAsync();
        return RedirectToAction("Login", "Account");
    }

    [Authorize]
    public ActionResult Settings()
    {
        return View();
    }


    [Authorize]
    public async Task<ActionResult> EditUser()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var user = await _userManager.FindByIdAsync(userId!);

        if (user == null)
        {
            return RedirectToAction("Login", "Account");
        }
        return View(new AccountEditUserModel { AdSoyad = user.AdSoyad, Email = user.Email! });
    }

    [HttpPost]
    [Authorize]
    public async Task<ActionResult> EditUser(AccountEditUserModel model)
    {
        if (ModelState.IsValid)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var user = await _userManager.FindByIdAsync(userId!);
            if (user != null)
            {
                user.Email = model.Email;
                user.AdSoyad = model.AdSoyad;
                var result = await _userManager.UpdateAsync(user);

                if (result.Succeeded)
                {
                    TempData["message"] = "Kullanıcı bilgileri güncellendi.";
                }
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }
            }
        }
        return View(model);
    }


    [Authorize]
    public ActionResult ChangePassword()
    {
        return View();
    }

    [HttpPost]
    [Authorize]
    public async Task<ActionResult> ChangePassword(AccountChangePasswordModel model)
    {
        if (ModelState.IsValid)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var user = await _userManager.FindByIdAsync(userId!);
            if (user != null)
            {
                var result = await _userManager.ChangePasswordAsync(user, model.OldPassword, model.Password);
                if (result.Succeeded)
                {
                    TempData["message"] = "Parola değiştirildi.";
                }
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }
            }
        }
        return View(model);
    }
    public ActionResult AccessDenied()
    {
        return View();
    }

    public ActionResult ForgotPassword()
    {
        return View();
    }

    [HttpPost]
    public async Task<ActionResult> ForgotPassword(string email)
    {
        if (string.IsNullOrEmpty(email))
        {
            TempData["message"] = "E-posta adresini giriniz.";
            return View();
        }
        var user = await _userManager.FindByEmailAsync(email);
        if (user == null)
        {
            TempData["message"] = "E-posta adresi bulunamadı.";
            return View();
        }

        var token = await _userManager.GeneratePasswordResetTokenAsync(user);

        var url = Url.Action("ResetPassword", "Account", new { userId = user.Id, token });
        var link = $"<a href='http://localhost:5162{url}'> Şifre Yenile</a>";
        await _emailService.SendEmailAsync(user.Email!, "Şifre yenilemek için tıklayınız.", link);
        TempData["message"] = "E-posta adresine link gönderildi.";
        return RedirectToAction("Login");
    }

    public async Task<ActionResult> ResetPassword(string userId, string token)
    {
        if (userId == null || token == null)
        {
            return RedirectToAction("Login");
        }
        var user = await _userManager.FindByIdAsync(userId);
        if (user == null)
        {
            return RedirectToAction("Login");
        }
        var model = new AccountResetPasswordModel
        {
            Email = user.Email!,
            Token = token
        };
        return View(model);
    }

    [HttpPost]
    public async Task<ActionResult> ResetPassword(AccountResetPasswordModel model)
    {
        if (ModelState.IsValid)
        {
            var user = await _userManager.FindByEmailAsync(model.Email);

            if (user == null)
            {
                return RedirectToAction("Login");
            }

            var result = await _userManager.ResetPasswordAsync(user, model.Token, model.Password);

            if (result.Succeeded)
            {
                TempData["Mesaj"] = "Şifreniz güncellendi";
                return RedirectToAction("Login");
            }

            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error.Description);
            }
        }
        return View(model);
    }

}
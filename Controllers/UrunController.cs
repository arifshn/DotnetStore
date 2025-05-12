using System.Threading.Tasks;
using dotnet_store.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace dotnet_store.Controllers;

[Authorize]
public class UrunController : Controller
{
    private readonly UserManager<AppUser> _userManager;
    private readonly DataContext _context;
    private readonly SignInManager<AppUser> _signInManager;
    public UrunController(DataContext context, UserManager<AppUser> userManager, SignInManager<AppUser> signInManager)
    {
        _context = context;
        _userManager = userManager;
        _signInManager = signInManager;
    }

    public ActionResult Index(int? kategori)
    {
        var query = _context.Urunler.AsQueryable();
        if (kategori != null)
        {
            query = query.Where(i => i.KategoriId == kategori);
        }
        else
        {
            ViewBag.Kategori = null;
        }

        var urunler = query.Select(i => new UrunGetModel
        {
            Id = i.Id,
            UrunAdi = i.UrunAdi,
            Fiyat = i.Fiyat,
            Aktif = i.Aktif,
            Anasayfa = i.Anasayfa,
            KategoriAdi = i.Kategori.KategoriAdi,
            Resim = i.Resim
        }).ToList();
        ViewBag.Kategoriler = new SelectList(_context.Kategoriler.ToList(), "Id", "KategoriAdi", kategori);

        return View(urunler);
    }

    [AllowAnonymous]
    public IActionResult List(string url)
    {
        Console.WriteLine($"List action called. URL: {url}");
        var urunler = _context.Urunler
            .Where(u => u.Aktif);

        if (!string.IsNullOrEmpty(url))
        {
            urunler = urunler.Where(u => u.Kategori.Url == url);
        }

        return View(urunler.ToList());
    }
    [AllowAnonymous]
    public IActionResult Details(int id)
    {
        Console.WriteLine($"Details action called. Product ID: {id}");
        var urun = _context.Urunler.Find(id);

        if (urun == null)
        {
            Console.WriteLine("Product not found, redirecting to Home/Index");
            return RedirectToAction("Index", "Home");
        }


        ViewData["BenzerUrunler"] = _context.Urunler
            .Where(i => i.Aktif && i.KategoriId == urun.KategoriId && i.Id != id)
            .Take(4)
            .ToList();


        var reviews = _context.Reviews
            .Where(r => r.UrunId == id)
            .Include(r => r.User)
            .Select(r => new Review
            {
                Id = r.Id,
                Puan = r.Puan,
                Yorum = r.Yorum,
                YorumTarihi = r.YorumTarihi,
                User = r.User,
                UserId = r.User.Id
            })
            .ToList();
        ViewBag.Reviews = reviews;


        bool canComment = false;
        if (_signInManager.IsSignedIn(User))
        {
            var userName = _userManager.GetUserName(User);
            canComment = _context.Orders
                .Where(o => o.Username == userName)
                .Any(o => o.OrderItems.Any(oi => oi.UrunId == id));
            Console.WriteLine($"CanComment check for user {userName}: {canComment}");
        }
        ViewBag.CanComment = canComment;

        return View(urun);
    }

    public ActionResult Create()
    {
        ViewBag.Kategoriler = new SelectList(_context.Kategoriler.ToList(), "Id", "KategoriAdi");
        return View();
    }

    [HttpPost]
    public async Task<ActionResult> Create(UrunCreateModel model)
    {
        if (model.Resim == null || model.Resim.Length == 0)
        {
            ModelState.AddModelError("Resim", "Resim boş olamaz!");
        }
        if (ModelState.IsValid)
        {
            var fileName = Path.GetRandomFileName() + ".jpg";
            var path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/img", fileName);

            if (model.Resim != null)
            {
                using (var stream = new FileStream(path, FileMode.Create))
                {
                    await model.Resim.CopyToAsync(stream);
                }
            }

            var entity = new Urun()
            {
                UrunAdi = model.UrunAdi,
                Aciklama = model.Aciklama,
                Fiyat = model.Fiyat ?? 0,
                Aktif = model.Aktif,
                Anasayfa = model.Anasayfa,
                KategoriId = (int)model.KategoriId!,
                Resim = fileName
            };

            _context.Urunler.Add(entity);
            _context.SaveChanges();

            return RedirectToAction("Index");
        }
        ViewBag.Kategoriler = new SelectList(_context.Kategoriler.ToList(), "Id", "KategoriAdi");
        return View(model);
    }

    public ActionResult Edit(int id)
    {
        var entity = _context.Urunler.Select(i => new UrunEditModel
        {
            Id = i.Id,
            UrunAdi = i.UrunAdi,
            Aciklama = i.Aciklama,
            Aktif = i.Aktif,
            Anasayfa = i.Anasayfa,
            Fiyat = i.Fiyat,
            KategoriId = i.KategoriId,
            ResimAdi = i.Resim
        }).FirstOrDefault(i => i.Id == id);

        ViewBag.Kategoriler = new SelectList(_context.Kategoriler.ToList(), "Id", "KategoriAdi");
        return View(entity);
    }

    [HttpPost]
    public async Task<ActionResult> Edit(int id, UrunEditModel model)
    {
        if (id != model.Id)
        {
            return RedirectToAction("Index");
        }
        if (ModelState.IsValid)
        {
            var entity = _context.Urunler.FirstOrDefault(i => i.Id == model.Id);
            if (entity != null)
            {
                if (model.Resim != null)
                {
                    var fileName = Path.GetRandomFileName() + ".jpg";
                    var path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/img", fileName);

                    using (var stream = new FileStream(path, FileMode.Create))
                    {
                        await model.Resim!.CopyToAsync(stream);
                    }

                    entity.Resim = fileName;
                }
                entity.UrunAdi = model.UrunAdi;
                entity.Aciklama = model.Aciklama;
                entity.Fiyat = model.Fiyat ?? 0;
                entity.Aktif = model.Aktif;
                entity.Anasayfa = model.Anasayfa;
                entity.KategoriId = (int)model.KategoriId!;
                _context.SaveChanges();
                TempData["Mesaj"] = $"{entity.UrunAdi} ürünü güncellendi.";
                return RedirectToAction("Index");
            }
        }
        ViewBag.Kategoriler = new SelectList(_context.Kategoriler.ToList(), "Id", "KategoriAdi");
        return View(model);
    }
    public ActionResult Delete(int? id)
    {
        if (id == null)
        {
            return RedirectToAction("Index");
        }
        var entity = _context.Urunler.FirstOrDefault(i => i.Id == id);
        if (entity != null)
        {
            return View(entity);
        }
        return RedirectToAction("Index");
    }
    [HttpPost]
    public ActionResult DeleteConfirm(int? id)
    {
        if (id == null)
        {
            return RedirectToAction("Index");
        }
        var entity = _context.Urunler.FirstOrDefault(i => i.Id == id);
        if (entity != null)
        {
            _context.Urunler.Remove(entity);
            _context.SaveChanges();
            TempData["Message"] = $"{entity.UrunAdi} ürünü silindi.";
        }
        return RedirectToAction("Index");
    }
    [HttpPost]
    [Authorize]
    public async Task<IActionResult> AddReview(int UrunId, int Puan, string Yorum)
    {
        Console.WriteLine($"AddReview called. User: {User.Identity!.Name}, IsAuthenticated: {User.Identity.IsAuthenticated}, UrunId: {UrunId}, Puan: {Puan}, Yorum: {Yorum}");
        var currentUser = await _userManager.GetUserAsync(User);
        if (currentUser == null)
        {
            Console.WriteLine("Current user is null");
            TempData["Error"] = "Kullanıcı oturumu doğrulanamadı. Lütfen tekrar giriş yapın.";
            return RedirectToAction("Details", new { id = UrunId });
        }

        Console.WriteLine($"Current user: ID={currentUser.Id}, Username={currentUser.UserName}, Roles={string.Join(", ", await _userManager.GetRolesAsync(currentUser))}");

        if (!ModelState.IsValid || Puan < 1 || Puan > 5 || string.IsNullOrWhiteSpace(Yorum))
        {
            TempData["Error"] = "Lütfen geçerli bir puan (1-5) ve yorum girin.";
            return RedirectToAction("Details", new { id = UrunId });
        }
        var canComment = _context.Orders
            .Where(o => o.Username == (currentUser.UserName ?? string.Empty))
            .Any(o => o.OrderItems.Any(oi => oi.UrunId == UrunId));
        if (!canComment)
        {
            Console.WriteLine($"User {currentUser.UserName} has not purchased product ID={UrunId}");
            TempData["Error"] = "Bu ürünü satın almadığınız için yorum yapamazsınız.";
            return RedirectToAction("Details", new { id = UrunId });
        }


        var existingReview = _context.Reviews
            .FirstOrDefault(r => r.UrunId == UrunId && r.UserId == currentUser.Id);

        if (existingReview != null)
        {
            existingReview.Puan = Puan;
            existingReview.Yorum = Yorum;
            existingReview.YorumTarihi = DateTime.Now;
            _context.Reviews.Update(existingReview);
        }
        else
        {
            var newReview = new Review
            {
                UrunId = UrunId,
                Puan = Puan,
                Yorum = Yorum,
                YorumTarihi = DateTime.Now,
                UserId = currentUser.Id
            };
            _context.Reviews.Add(newReview);
        }

        await _context.SaveChangesAsync();
        TempData["Success"] = "Yorumunuz başarıyla kaydedildi.";
        return RedirectToAction("Details", new { id = UrunId });
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> DeleteReview(int id, int urunId)
    {
        Console.WriteLine($"DeleteReview called. Review ID: {id}, UrunId: {urunId}");
        var review = await _context.Reviews.FindAsync(id);
        if (review == null)
        {
            TempData["Error"] = "Yorum bulunamadı.";
            return RedirectToAction("Details", new { id = urunId });
        }

        _context.Reviews.Remove(review);
        await _context.SaveChangesAsync();

        TempData["Success"] = "Yorum başarıyla silindi.";
        return RedirectToAction("Details", new { id = urunId });
    }
    [HttpGet]
    [Authorize]
    public async Task<IActionResult> EditReview(int id, int urunId)
    {
        Console.WriteLine($"EditReview GET called. Review ID: {id}, UrunId: {urunId}");
        var review = await _context.Reviews
            .Include(r => r.User)
            .FirstOrDefaultAsync(r => r.Id == id && r.UrunId == urunId);

        if (review == null)
        {
            Console.WriteLine($"Review not found: ID={id}, UrunId={urunId}");
            TempData["Error"] = "Yorum bulunamadı.";
            return RedirectToAction("Details", new { id = urunId });
        }

        var currentUser = await _userManager.GetUserAsync(User);
        if (currentUser == null || (review.UserId != currentUser.Id && !await _userManager.IsInRoleAsync(currentUser, "Admin")))
        {
            Console.WriteLine($"Unauthorized access to EditReview: User={currentUser?.UserName ?? "Anonymous"}, Review UserID={review.UserId}");
            TempData["Error"] = "Bu yorumu düzenleme yetkiniz yok.";
            return RedirectToAction("Details", new { id = urunId });
        }

        var model = new ReviewEditModel
        {
            Id = review.Id,
            UrunId = review.UrunId,
            Puan = review.Puan,
            Yorum = review.Yorum
        };

        return View(model);
    }

    [HttpPost]
    [Authorize]
    public async Task<IActionResult> EditReview(ReviewEditModel model)
    {
        Console.WriteLine($"EditReview POST called. Review ID: {model.Id}, UrunId: {model.UrunId}, Puan: {model.Puan}, Yorum: {model.Yorum}");

        if (!User.Identity!.IsAuthenticated)
        {
            Console.WriteLine("User is not authenticated, redirecting to Login");
            return RedirectToAction("Login", "Account", new { returnUrl = $"/Urun/Details?id={model.UrunId}" });
        }

        var currentUser = await _userManager.GetUserAsync(User);
        if (currentUser == null)
        {
            Console.WriteLine("Current user is null, redirecting to Login");
            TempData["Error"] = "Kullanıcı oturumu doğrulanamadı. Lütfen tekrar giriş yapın.";
            return RedirectToAction("Login", "Account", new { returnUrl = $"/Urun/Details?id={model.UrunId}" });
        }

        if (!ModelState.IsValid || model.Puan < 1 || model.Puan > 5 || string.IsNullOrWhiteSpace(model.Yorum))
        {
            Console.WriteLine("Validation failed: Invalid Puan or Yorum");
            TempData["Error"] = "Lütfen geçerli bir puan (1-5) ve yorum girin.";
            return View(model);
        }

        var review = await _context.Reviews
            .FirstOrDefaultAsync(r => r.Id == model.Id && r.UrunId == model.UrunId);
        if (review == null)
        {
            Console.WriteLine($"Review not found: ID={model.Id}, UrunId={model.UrunId}");
            TempData["Error"] = "Yorum bulunamadı.";
            return RedirectToAction("Details", new { id = model.UrunId });
        }

        if (review.UserId != currentUser.Id && !await _userManager.IsInRoleAsync(currentUser, "Admin"))
        {
            Console.WriteLine($"Unauthorized access to EditReview: User={currentUser.UserName}, Review UserID={review.UserId}");
            TempData["Error"] = "Bu yorumu düzenleme yetkiniz yok.";
            return RedirectToAction("Details", new { id = model.UrunId });
        }

        review.Puan = model.Puan;
        review.Yorum = model.Yorum;
        review.YorumTarihi = DateTime.Now;

        await _context.SaveChangesAsync();
        Console.WriteLine($"Review updated: ID={model.Id}, UserID={currentUser.Id}, UrunId={model.UrunId}, Puan={model.Puan}");

        TempData["Success"] = "Yorumunuz başarıyla güncellendi.";
        return RedirectToAction("Details", new { id = model.UrunId });
    }

}
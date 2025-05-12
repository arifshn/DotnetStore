using System.ComponentModel.DataAnnotations;
namespace dotnet_store.Models;

public class UrunModel
{
    [Display(Name = "Ürün Adı")]
    [Required(ErrorMessage = "{0} boş olamaz!")]
    [StringLength(50, ErrorMessage = "{0} için {2}-{1} karakter aralığında değer girmelisiniz. ", MinimumLength = 10)]
    public string UrunAdi { get; set; } = null!;

    [Display(Name = "Ürün Fiyat")]
    [Required(ErrorMessage = "{0} boş olamaz!")]
    [Range(0, 1000000, ErrorMessage = "{0} için {1} ile {2} arasında değer girmelisiniz.")]
    public double? Fiyat { get; set; }

    [Display(Name = "Ürün Resmi")]
    public IFormFile? Resim { get; set; }
    public string? Aciklama { get; set; }
    public bool Aktif { get; set; }
    public bool Anasayfa { get; set; }

    [Display(Name = "Ürün Kategorisi")]
    [Required(ErrorMessage = "{0} boş olamaz!")]
    public int? KategoriId { get; set; }
}
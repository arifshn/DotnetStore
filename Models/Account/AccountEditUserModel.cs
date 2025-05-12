using System.ComponentModel.DataAnnotations;

namespace dotnet_store.Models;

public class AccountEditUserModel
{
    [Required]
    [Display(Name = "Ad Soyad")]
    // [RegularExpression(@"^[a-zA-Z0-9]+$", ErrorMessage = "Kullanıcı adı sadece harf ve rakamlardan oluşmalıdır.")]
    public string AdSoyad { get; set; } = null!;

    [Required]
    [Display(Name = "E-posta")]
    [EmailAddress(ErrorMessage = "Geçersiz e-posta adresi.")]
    public string Email { get; set; } = null!;


}

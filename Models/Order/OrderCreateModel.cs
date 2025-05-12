using System.ComponentModel.DataAnnotations;

namespace dotnet_store.Models;

public class OrderCreateModel
{
    [Required]
    public string AdSoyad { get; set; } = null!;
    [Required]
    public string Sehir { get; set; } = null!;
    [Required]
    public string TelefonNumarasi { get; set; } = null!;
    [Required]
    public string PostaKodu { get; set; } = null!;
    [Required]
    public string AdresSatiri { get; set; } = null!;
    [Required]
    public string Country { get; set; } = null!;
    public string? SiparisNotu { get; set; }
    [Required]
    public string CartName { get; set; } = null!;
    [Required]
    public string CartNumber { get; set; } = null!;
    [Required]
    public string CartExpirationYear { get; set; } = null!;
    [Required]
    public string CartExpirationMonth { get; set; } = null!;
    [Required]
    public string CartCVV { get; set; } = null!;
    public bool SaveAddress { get; set; }
}
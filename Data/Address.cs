using System.ComponentModel.DataAnnotations;

namespace dotnet_store.Models
{
    public class Address
    {
        public int Id { get; set; }

        [Required]
        public string AdresAd { get; set; } = null!;

        [Required]
        public string UserId { get; set; } = null!;

        [Required]
        public string AdresSatiri { get; set; } = null!;

        [Required]
        public string Sehir { get; set; } = null!;

        [Required]
        public string PostaKodu { get; set; } = null!;

        [Required]
        public string Country { get; set; } = null!;
        public string? TelefonNumarasi { get; set; }
    }
}

using System.ComponentModel.DataAnnotations;

namespace dotnet_store.Models
{
    public class Review
    {
        public int Id { get; set; }
        public int UrunId { get; set; }
        public Urun Urun { get; set; } = null!;
        public int UserId { get; set; }
        public AppUser User { get; set; } = null!;
        [Range(1, 5)]
        public int Puan { get; set; }
        [StringLength(500)]
        public string Yorum { get; set; } = null!;
        public DateTime YorumTarihi { get; set; }
    }
}
namespace dotnet_store.Models
{
    public class ReviewEditModel
    {
        public int Id { get; set; }
        public int UrunId { get; set; }
        public int Puan { get; set; }
        public string Yorum { get; set; } = string.Empty;
    }
}
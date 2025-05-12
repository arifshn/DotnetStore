namespace dotnet_store.Models;

public class Favorite
{
    public int Id { get; set; }
    public string CustomerId { get; set; } = string.Empty;

    public int UrunId { get; set; }
    public Urun Urun { get; set; } = null!;
}

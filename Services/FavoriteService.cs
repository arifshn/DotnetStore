using dotnet_store.Models;
using Microsoft.EntityFrameworkCore;

public interface IFavoriService
{
    Task AddToFavorites(int urunId, string customerId);
    Task RemoveFromFavorites(int urunId, string customerId);
    Task<List<Favorite>> GetFavorites(string customerId);
}

public class FavoriService : IFavoriService
{
    private readonly DataContext _context;

    public FavoriService(DataContext context)
    {
        _context = context;
    }

    public async Task AddToFavorites(int urunId, string customerId)
    {
        var existingFavorite = await _context.Favorites
            .FirstOrDefaultAsync(f => f.UrunId == urunId && f.CustomerId == customerId);

        if (existingFavorite == null)
        {
            var favorite = new Favorite
            {
                UrunId = urunId,
                CustomerId = customerId
            };

            _context.Favorites.Add(favorite);
            await _context.SaveChangesAsync();
        }
    }

    public async Task RemoveFromFavorites(int urunId, string customerId)
    {
        var favorite = await _context.Favorites
            .FirstOrDefaultAsync(f => f.UrunId == urunId && f.CustomerId == customerId);

        if (favorite != null)
        {
            _context.Favorites.Remove(favorite);
            await _context.SaveChangesAsync();
        }
    }

    public async Task<List<Favorite>> GetFavorites(string customerId)
    {
        return await _context.Favorites
            .Where(f => f.CustomerId == customerId)
            .Include(f => f.Urun)
            .ToListAsync();
    }
}



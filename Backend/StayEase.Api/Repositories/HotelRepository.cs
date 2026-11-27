using Microsoft.EntityFrameworkCore;
using StayEase.Api.Data;
using StayEase.Api.Enums;
using StayEase.Api.Models;

namespace StayEase.Api.Repositories;

public class HotelRepository : GenericRepository<Hotel>, IHotelRepository
{
    public HotelRepository(ApplicationDbContext context) : base(context) { }

    public Task<List<Hotel>> SearchAsync(string? search, HotelStatus? status, CancellationToken cancellationToken = default)
    {
        var query = Set.AsNoTracking().Include(h => h.Manager).AsQueryable();
        if (!string.IsNullOrWhiteSpace(search))
            query = query.Where(h => h.HotelName.Contains(search) || h.City.Contains(search) || h.Manager.FullName.Contains(search));
        if (status.HasValue) query = query.Where(h => h.Status == status.Value);
        return query.OrderBy(h => h.HotelName).ToListAsync(cancellationToken);
    }
}

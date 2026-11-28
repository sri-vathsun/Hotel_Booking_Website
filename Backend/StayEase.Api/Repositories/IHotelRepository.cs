using StayEase.Api.Enums;
using StayEase.Api.Models;

namespace StayEase.Api.Repositories;

public interface IHotelRepository : IGenericRepository<Hotel>
{
    Task<List<Hotel>> SearchAsync(string? search, HotelStatus? status, CancellationToken cancellationToken = default);
}

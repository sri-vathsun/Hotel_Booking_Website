using StayEase.Api.Models;

namespace StayEase.Api.Repositories;

public interface IFacilityRepository : IGenericRepository<Facility>
{
    Task<bool> NameExistsAsync(string name, int? excludedId = null, CancellationToken cancellationToken = default);
}

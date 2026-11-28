using Microsoft.EntityFrameworkCore;
using StayEase.Api.Data;
using StayEase.Api.Models;

namespace StayEase.Api.Repositories;

public class FacilityRepository : GenericRepository<Facility>, IFacilityRepository
{
    public FacilityRepository(ApplicationDbContext context) : base(context) { }
    public Task<bool> NameExistsAsync(string name, int? excludedId = null, CancellationToken cancellationToken = default) =>
        Set.AnyAsync(f => f.FacilityName == name && (!excludedId.HasValue || f.FacilityId != excludedId), cancellationToken);
}

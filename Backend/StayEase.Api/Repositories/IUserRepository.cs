using StayEase.Api.Models;

namespace StayEase.Api.Repositories;

public interface IUserRepository : IGenericRepository<User>
{
    Task<List<User>> GetByRoleAsync(Enums.UserRole role, string? search, CancellationToken cancellationToken = default);
}

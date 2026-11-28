using Microsoft.EntityFrameworkCore;
using StayEase.Api.Data;
using StayEase.Api.Enums;
using StayEase.Api.Models;

namespace StayEase.Api.Repositories;

public class UserRepository : GenericRepository<User>, IUserRepository
{
    public UserRepository(ApplicationDbContext context) : base(context) { }

    public Task<List<User>> GetByRoleAsync(UserRole role, string? search, CancellationToken cancellationToken = default)
    {
        var query = Set.AsNoTracking().Where(u => u.Role == role);
        if (!string.IsNullOrWhiteSpace(search))
            query = query.Where(u => u.FullName.Contains(search) || u.Email.Contains(search));
        return query.OrderBy(u => u.FullName).ToListAsync(cancellationToken);
    }
}

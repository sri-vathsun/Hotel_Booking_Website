
using Microsoft.EntityFrameworkCore;
using StayEase.Api.Models;

namespace StayEase.Api.Data
{
	public class ApplicationDbContext : DbContext
	{
		public ApplicationDbContext(
			DbContextOptions<ApplicationDbContext> options)
			: base(options)
		{
		}

		// Database tables
		public DbSet<User> Users { get; set; }

		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
			base.OnModelCreating(modelBuilder);

			// Every user must have a unique email address
			modelBuilder.Entity<User>()
				.HasIndex(u => u.Email)
				.IsUnique();
		}
	}
}


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
		public DbSet<Hotel> Hotels { get; set; }
		public DbSet<Room> Rooms { get; set; }
		public DbSet<RoomType> RoomTypes { get; set; }
		public DbSet<Booking> Bookings { get; set; }
		public DbSet<Facility> Facilities { get; set; }
		public DbSet<HotelFacility> HotelFacilities { get; set; }

		protected override void OnModelCreating(
			ModelBuilder modelBuilder)
		{
			base.OnModelCreating(modelBuilder);

			// 1. User: unique email
			modelBuilder.Entity<User>()
				.HasIndex(u => u.Email)
				.IsUnique();

			// 2. User -> Hotels (Manager relationship)
			modelBuilder.Entity<Hotel>()
				.HasOne(h => h.Manager)
				.WithMany(u => u.ManagedHotels)
				.HasForeignKey(h => h.ManagerId)
				.OnDelete(DeleteBehavior.Restrict);

			// 3. User -> Bookings (Customer relationship)
			modelBuilder.Entity<Booking>()
				.HasOne(b => b.Customer)
				.WithMany(u => u.CustomerBookings)
				.HasForeignKey(b => b.CustomerId)
				.OnDelete(DeleteBehavior.Restrict);

			// 4. Hotel -> Rooms
			modelBuilder.Entity<Room>()
				.HasOne(r => r.Hotel)
				.WithMany(h => h.Rooms)
				.HasForeignKey(r => r.HotelId)
				.OnDelete(DeleteBehavior.Restrict);

			// Unique room number within each hotel
			modelBuilder.Entity<Room>()
				.HasIndex(r => new { r.HotelId, r.RoomNumber })
				.IsUnique();

			// 5. RoomType -> Rooms
			modelBuilder.Entity<Room>()
				.HasOne(r => r.RoomType)
				.WithMany(rt => rt.Rooms)
				.HasForeignKey(r => r.RoomTypeId)
				.OnDelete(DeleteBehavior.Restrict);

			// 6. Hotel -> Bookings
			modelBuilder.Entity<Booking>()
				.HasOne(b => b.Hotel)
				.WithMany()
				.HasForeignKey(b => b.HotelId)
				.OnDelete(DeleteBehavior.Restrict);

			// 7. Room -> Bookings
			modelBuilder.Entity<Booking>()
				.HasOne(b => b.Room)
				.WithMany(r => r.Bookings)
				.HasForeignKey(b => b.RoomId)
				.OnDelete(DeleteBehavior.Restrict);

			// 8. HotelFacility composite primary key
			modelBuilder.Entity<HotelFacility>()
				.HasKey(hf => new
				{
					hf.HotelId,
					hf.FacilityId
				});

			// 9. Hotel -> HotelFacilities
			modelBuilder.Entity<HotelFacility>()
				.HasOne(hf => hf.Hotel)
				.WithMany(h => h.HotelFacilities)
				.HasForeignKey(hf => hf.HotelId)
				.OnDelete(DeleteBehavior.Restrict);

			// 10. Facility -> HotelFacilities
			modelBuilder.Entity<HotelFacility>()
				.HasOne(hf => hf.Facility)
				.WithMany(f => f.HotelFacilities)
				.HasForeignKey(hf => hf.FacilityId)
				.OnDelete(DeleteBehavior.Restrict);

			// 11. Unique facility names
			modelBuilder.Entity<Facility>()
				.HasIndex(f => f.FacilityName)
				.IsUnique();

			// Decimal precision
			modelBuilder.Entity<Hotel>()
				.Property(h => h.Rating)
				.HasPrecision(3, 2);

			modelBuilder.Entity<Room>()
				.Property(r => r.PricePerNight)
				.HasPrecision(18, 2);

			modelBuilder.Entity<Booking>()
				.Property(b => b.TotalAmount)
				.HasPrecision(18, 2);
		}
	}
}


using StayEase.Api.DTOs.Customer;
using StayEase.Api.Enums;
using StayEase.Api.Models;
using StayEase.Api.Repositories;
using StayEase.Api.Services.Interfaces;

namespace StayEase.Api.Services.Implementations;

public class CustomerBookingService : ICustomerBookingService
{
    private readonly ICustomerBookingRepository _repository;

    public CustomerBookingService(ICustomerBookingRepository repository)
    {
        _repository = repository;
    }

    public async Task<BookingConfirmationDto?> CreateBookingAsync(
        int customerId,
        CreateBookingDto dto)
    {
        var checkIn = dto.CheckInDate.Date;
        var checkOut = dto.CheckOutDate.Date;

        if (checkIn < DateTime.Today ||
            checkOut <= checkIn ||
            dto.NumberOfGuests < 1 ||
            dto.NumberOfGuests > 20)
        {
            return null;
        }

        var room = await _repository.GetRoomAsync(dto.RoomId);

        if (room == null ||
            room.Hotel.Status != HotelStatus.Approved ||
            room.Status != RoomStatus.Available ||
            room.Capacity < dto.NumberOfGuests)
        {
            return null;
        }

        bool hasConflict = await _repository.HasBookingConflictAsync(
            dto.RoomId,
            checkIn,
            checkOut);

        if (hasConflict)
        {
            return null;
        }

        int nights = (checkOut - checkIn).Days;

        decimal totalAmount = room.PricePerNight * nights;

        var booking = new Booking
        {
            CustomerId = customerId,
            HotelId = room.HotelId,
            RoomId = room.RoomId,
            CheckInDate = checkIn,
            CheckOutDate = checkOut,
            NumberOfGuests = dto.NumberOfGuests,
            TotalAmount = totalAmount,
            Status = BookingStatus.Pending,
            BookedAt = DateTime.UtcNow
        };

        await _repository.CreateBookingAsync(booking);

        return MapToDto(booking, room);
    }

    public async Task<BookingConfirmationDto?> GetBookingConfirmationAsync(
        int bookingId,
        int customerId)
    {
        var booking = await _repository.GetBookingAsync(
            bookingId,
            customerId);

        if (booking == null)
        {
            return null;
        }

        return new BookingConfirmationDto
        {
            BookingId = booking.BookingId,
            HotelName = booking.Hotel.HotelName,
            RoomNumber = booking.Room.RoomNumber,
            RoomType = booking.Room.RoomType.TypeName,
            CheckInDate = booking.CheckInDate,
            CheckOutDate = booking.CheckOutDate,
            NumberOfGuests = booking.NumberOfGuests,
            NumberOfNights = (booking.CheckOutDate.Date -
                              booking.CheckInDate.Date).Days,
            PricePerNight = booking.Room.PricePerNight,
            TotalAmount = booking.TotalAmount,
            Status = booking.Status,
            BookedAt = booking.BookedAt
        };
    }

    private static BookingConfirmationDto MapToDto(
        Booking booking,
        Room room)
    {
        return new BookingConfirmationDto
        {
            BookingId = booking.BookingId,
            HotelName = room.Hotel.HotelName,
            RoomNumber = room.RoomNumber,
            RoomType = room.RoomType.TypeName,
            CheckInDate = booking.CheckInDate,
            CheckOutDate = booking.CheckOutDate,
            NumberOfGuests = booking.NumberOfGuests,
            NumberOfNights = (booking.CheckOutDate -
                              booking.CheckInDate).Days,
            PricePerNight = room.PricePerNight,
            TotalAmount = booking.TotalAmount,
            Status = booking.Status,
            BookedAt = booking.BookedAt
        };
    }
}

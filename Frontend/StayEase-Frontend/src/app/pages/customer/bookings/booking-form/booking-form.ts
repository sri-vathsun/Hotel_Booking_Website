
import { Component, OnInit, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { ActivatedRoute, Router, RouterLink } from '@angular/router';
import {
  AvailableRoom,
  CustomerBookingService
} from '../../../../core/services/customer/customer-booking.service';

@Component({
  selector: 'app-booking-form',
  standalone: true,
  imports: [CommonModule, FormsModule, RouterLink],
  templateUrl: './booking-form.html',
  styleUrl: './booking-form.css'
})
export class BookingForm implements OnInit {

  hotelId = 0;
  roomId = 0;

  checkIn = '';
  checkOut = '';
  guests = 1;

  room = signal<AvailableRoom | null>(null);
  loading = signal(true);
  submitting = signal(false);
  errorMessage = signal('');

  constructor(
    private route: ActivatedRoute,
    private router: Router,
    private bookingService: CustomerBookingService
  ) {}

  ngOnInit(): void {
    this.hotelId = Number(this.route.snapshot.paramMap.get('hotelId'));
    this.roomId = Number(this.route.snapshot.paramMap.get('roomId'));

    const query = this.route.snapshot.queryParamMap;

    this.checkIn = query.get('checkIn') ?? '';
    this.checkOut = query.get('checkOut') ?? '';
    this.guests = Number(query.get('guests') ?? 1);

    if (
      !this.hotelId ||
      !this.roomId ||
      !this.checkIn ||
      !this.checkOut ||
      this.guests < 1
    ) {
      this.errorMessage.set('Invalid booking details.');
      this.loading.set(false);
      return;
    }

    this.loadSelectedRoom();
  }

  loadSelectedRoom(): void {
    this.bookingService.getAvailableRooms(
      this.hotelId,
      this.checkIn,
      this.checkOut,
      this.guests
    ).subscribe({
      next: rooms => {
        const selectedRoom = rooms.find(
          room => room.roomId === this.roomId
        );

        if (!selectedRoom) {
          this.errorMessage.set(
            'This room is no longer available. Please search again.'
          );
        } else {
          this.room.set(selectedRoom);
        }

        this.loading.set(false);
      },
      error: () => {
        this.errorMessage.set('Unable to verify room availability.');
        this.loading.set(false);
      }
    });
  }

  get numberOfNights(): number {
    const start = new Date(`${this.checkIn}T00:00:00`);
    const end = new Date(`${this.checkOut}T00:00:00`);

    return Math.round(
      (end.getTime() - start.getTime()) / (1000 * 60 * 60 * 24)
    );
  }

  get estimatedTotal(): number {
    return (this.room()?.pricePerNight ?? 0) * this.numberOfNights;
  }

  confirmBooking(): void {
    if (!this.room()) {
      return;
    }

    this.submitting.set(true);
    this.errorMessage.set('');

    this.bookingService.createBooking({
      roomId: this.roomId,
      checkInDate: `${this.checkIn}T00:00:00`,
      checkOutDate: `${this.checkOut}T00:00:00`,
      numberOfGuests: this.guests
    }).subscribe({
      next: response => {
        this.submitting.set(false);

        this.router.navigate([
          '/customer/bookings',
          response.bookingId,
          'confirmation'
        ]);
      },
      error: error => {
        this.submitting.set(false);

        if (error.status === 401 || error.status === 403) {
          this.errorMessage.set(
            'Customer sign-in is required to create a booking. Shared authentication will be connected at the final stage.'
          );
        } else {
          this.errorMessage.set(
            error.error?.message ??
            'Unable to create booking. The room may no longer be available.'
          );
        }
      }
    });
  }
}

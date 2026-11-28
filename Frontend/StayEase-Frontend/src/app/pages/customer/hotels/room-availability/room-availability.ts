
import { Component, OnInit, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { ActivatedRoute, Router, RouterLink } from '@angular/router';
import {
  AvailableRoom,
  CustomerBookingService
} from '../../../../core/services/customer/customer-booking.service';

@Component({
  selector: 'app-room-availability',
  standalone: true,
  imports: [CommonModule, FormsModule, RouterLink],
  templateUrl: './room-availability.html',
  styleUrl: './room-availability.css'
})
export class RoomAvailability implements OnInit {

  hotelId = 0;

  checkIn = '';
  checkOut = '';
  guests = 1;

  rooms = signal<AvailableRoom[]>([]);
  loading = signal(false);
  searched = signal(false);
  errorMessage = signal('');

  today = this.getToday();

  constructor(
    private route: ActivatedRoute,
    private router: Router,
    private bookingService: CustomerBookingService
  ) {}

  ngOnInit(): void {
    this.hotelId = Number(this.route.snapshot.paramMap.get('id'));

    const tomorrow = new Date();
    tomorrow.setDate(tomorrow.getDate() + 1);

    this.checkIn = this.getToday();
    this.checkOut = this.formatDate(tomorrow);
  }

  searchRooms(): void {

    this.errorMessage.set('');
    this.rooms.set([]);

    if (!this.checkIn || !this.checkOut) {
      this.errorMessage.set('Please select both dates.');
      return;
    }

    if (this.checkIn < this.today) {
      this.errorMessage.set('Check-in date cannot be in the past.');
      return;
    }

    if (this.checkOut <= this.checkIn) {
      this.errorMessage.set('Check-out must be after check-in.');
      return;
    }

    if (this.guests < 1 || this.guests > 20) {
      this.errorMessage.set('Guests must be between 1 and 20.');
      return;
    }

    this.loading.set(true);
    this.searched.set(true);

    this.bookingService.getAvailableRooms(
      this.hotelId,
      this.checkIn,
      this.checkOut,
      this.guests
    ).subscribe({
      next: data => {
        this.rooms.set(data);
        this.loading.set(false);
      },
      error: error => {
        this.loading.set(false);
        this.errorMessage.set(
          error.error?.message ??
          'Unable to check room availability.'
        );
      }
    });
  }

  selectRoom(room: AvailableRoom): void {
    this.router.navigate(
      ['/customer/hotels', this.hotelId, 'rooms', room.roomId, 'book'],
      {
        queryParams: {
          checkIn: this.checkIn,
          checkOut: this.checkOut,
          guests: this.guests
        }
      }
    );
  }

  private getToday(): string {
    return this.formatDate(new Date());
  }

  private formatDate(date: Date): string {
    const year = date.getFullYear();
    const month = String(date.getMonth() + 1).padStart(2, '0');
    const day = String(date.getDate()).padStart(2, '0');

    return `${year}-${month}-${day}`;
  }
}


import { Component, OnInit, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterLink } from '@angular/router';

import {
  CustomerAccountService,
  MyBooking
} from '../../../../core/services/customer/customer-account.service';

@Component({
  selector: 'app-my-bookings',
  standalone: true,
  imports: [CommonModule, RouterLink],
  templateUrl: './my-bookings.html',
  styleUrl: './my-bookings.css'
})
export class MyBookings implements OnInit {

  bookings = signal<MyBooking[]>([]);
  loading = signal(true);
  errorMessage = signal('');
  successMessage = signal('');

  constructor(
    private accountService: CustomerAccountService
  ) {}

  ngOnInit(): void {
    this.loadBookings();
  }

  loadBookings(): void {
    this.loading.set(true);
    this.errorMessage.set('');

    this.accountService.getMyBookings().subscribe({
      next: (data) => {
        this.bookings.set(data);
        this.loading.set(false);
      },
      error: (error) => {
        this.loading.set(false);

        if (error.status === 401 || error.status === 403) {
          this.errorMessage.set(
            'Please sign in with your Customer account to view your bookings.'
          );
        } else {
          this.errorMessage.set(
            'Unable to load bookings. Please try again.'
          );
        }
      }
    });
  }

  getStatus(status: number): string {
    const statuses: { [key: number]: string } = {
      1: 'Pending',
      2: 'Confirmed',
      3: 'Rejected',
      4: 'Cancelled',
      5: 'Checked In',
      6: 'Checked Out'
    };

    return statuses[status] || 'Unknown';
  }

  canCancel(booking: MyBooking): boolean {
    const today = new Date();
    today.setHours(0, 0, 0, 0);

    const checkIn = new Date(booking.checkInDate);
    checkIn.setHours(0, 0, 0, 0);

    return (
      (booking.status === 1 || booking.status === 2) &&
      checkIn > today
    );
  }

  cancelBooking(id: number): void {
    const confirmed = window.confirm(
      'Are you sure you want to cancel this booking?'
    );

    if (!confirmed) {
      return;
    }

    this.successMessage.set('');
    this.errorMessage.set('');

    this.accountService.cancelBooking(id).subscribe({
      next: () => {
        this.successMessage.set(
          'Booking cancelled successfully.'
        );

        this.loadBookings();
      },
      error: (error) => {
        if (error.status === 401 || error.status === 403) {
          this.errorMessage.set(
            'Please sign in with your Customer account.'
          );
        } else {
          this.errorMessage.set(
            error.error?.message ||
            'Unable to cancel this booking.'
          );
        }
      }
    });
  }
}

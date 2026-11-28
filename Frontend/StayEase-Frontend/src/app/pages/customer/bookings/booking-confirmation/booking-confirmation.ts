
import { Component, OnInit, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ActivatedRoute, RouterLink } from '@angular/router';
import {
  BookingConfirmation,
  CustomerBookingService
} from '../../../../core/services/customer/customer-booking.service';

@Component({
  selector: 'app-booking-confirmation',
  standalone: true,
  imports: [CommonModule, RouterLink],
  templateUrl: './booking-confirmation.html',
  styleUrl: './booking-confirmation.css'
})
export class BookingConfirmationPage implements OnInit {

  booking = signal<BookingConfirmation | null>(null);
  loading = signal(true);
  errorMessage = signal('');

  constructor(
    private route: ActivatedRoute,
    private bookingService: CustomerBookingService
  ) {}

  ngOnInit(): void {
    const id = Number(this.route.snapshot.paramMap.get('id'));

    if (!id || id <= 0) {
      this.errorMessage.set('Invalid booking ID.');
      this.loading.set(false);
      return;
    }

    this.bookingService.getBookingConfirmation(id).subscribe({
      next: data => {
        this.booking.set(data);
        this.loading.set(false);
      },
      error: error => {
        this.loading.set(false);

        if (error.status === 401 || error.status === 403) {
          this.errorMessage.set(
            'Customer sign-in is required to view this confirmation. Shared authentication will be connected at the final stage.'
          );
        } else {
          this.errorMessage.set(
            'Unable to load booking confirmation.'
          );
        }
      }
    });
  }

  getStatus(status: number): string {
    switch (status) {
      case 1: return 'Pending';
      case 2: return 'Confirmed';
      case 3: return 'Rejected';
      case 4: return 'Cancelled';
      case 5: return 'Checked In';
      case 6: return 'Checked Out';
      default: return 'Unknown';
    }
  }
}

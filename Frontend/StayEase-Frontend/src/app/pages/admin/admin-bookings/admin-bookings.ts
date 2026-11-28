import { HttpErrorResponse } from '@angular/common/http';
import { CommonModule } from '@angular/common';
import { Component, OnInit, signal } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { AdminService, BookingDetail, BookingStatus, BookingSummary } from '../../../core/services/admin.service';

@Component({
  selector: 'app-admin-bookings',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './admin-bookings.html',
  styleUrl: './admin-bookings.css'
})
export class AdminBookings implements OnInit {
  bookings = signal<BookingSummary[]>([]);
  selectedBooking = signal<BookingDetail | null>(null);
  loading = signal(false);
  errorMessage = signal('');
  searchText = '';
  statusFilter: BookingStatus | null = null;
  page = 1;
  pageSize = 20;
  totalCount = 0;
  readonly statuses = [
    { value: BookingStatus.Pending, label: 'Pending' },
    { value: BookingStatus.Confirmed, label: 'Confirmed' },
    { value: BookingStatus.Rejected, label: 'Rejected' },
    { value: BookingStatus.Cancelled, label: 'Cancelled' },
    { value: BookingStatus.CheckedIn, label: 'Checked in' },
    { value: BookingStatus.CheckedOut, label: 'Checked out' }
  ];

  constructor(private adminService: AdminService) {}

  get totalPages(): number {
    return Math.max(1, Math.ceil(this.totalCount / this.pageSize));
  }

  ngOnInit(): void {
    this.loadBookings();
  }

  loadBookings(): void {
    this.loading.set(true);
    this.errorMessage.set('');
    this.adminService.getBookings(this.searchText.trim(), this.statusFilter, this.page).subscribe({
      next: (result) => {
        this.bookings.set(result.items);
        this.page = result.page;
        this.pageSize = result.pageSize;
        this.totalCount = result.totalCount;
        this.loading.set(false);
      },
      error: () => {
        this.loading.set(false);
        this.errorMessage.set('Unable to load bookings. Try again.');
      }
    });
  }

  filterByStatus(status: BookingStatus | null): void {
    this.statusFilter = status;
    this.page = 1;
    this.loadBookings();
  }

  updateStatus(booking: BookingSummary, status: BookingStatus): void {
    if (booking.status === status) {
      return;
    }

    this.adminService.setBookingStatus(booking.bookingId, status).subscribe({
      next: () => this.loadBookings(),
      error: (error: unknown) => {
        this.errorMessage.set(
          error instanceof HttpErrorResponse && error.status === 404
            ? 'This booking could not be found.'
            : 'Unable to update the booking status.'
        );
      }
    });
  }

  showDetails(id: number): void {
    this.selectedBooking.set(null);
    this.adminService.getBooking(id).subscribe({
      next: (booking) => this.selectedBooking.set(booking),
      error: () => this.errorMessage.set('Unable to load booking details.')
    });
  }

  changePage(page: number): void {
    if (page < 1 || page > this.totalPages || page === this.page) {
      return;
    }
    this.page = page;
    this.loadBookings();
  }

  statusName(status: BookingStatus): string {
    return BookingStatus[status] ?? 'Unknown';
  }
}

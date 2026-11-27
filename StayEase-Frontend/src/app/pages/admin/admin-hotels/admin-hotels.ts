import { HttpErrorResponse } from '@angular/common/http';
import { CommonModule } from '@angular/common';
import { Component, OnInit, signal } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { AdminService, HotelDetail, HotelStatus, HotelSummary } from '../../../core/services/admin.service';

@Component({
  selector: 'app-admin-hotels',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './admin-hotels.html',
  styleUrl: './admin-hotels.css'
})
export class AdminHotels implements OnInit {
  hotels = signal<HotelSummary[]>([]);
  selectedHotel = signal<HotelDetail | null>(null);
  loading = signal(false);
  errorMessage = signal('');
  searchText = '';
  statusFilter: HotelStatus | null = null;
  page = 1;
  pageSize = 20;
  totalCount = 0;
  readonly statuses = [
    { value: HotelStatus.Pending, label: 'Pending' },
    { value: HotelStatus.Approved, label: 'Approved' },
    { value: HotelStatus.Rejected, label: 'Rejected' },
    { value: HotelStatus.Inactive, label: 'Inactive' }
  ];

  constructor(private adminService: AdminService) {}

  get totalPages(): number {
    return Math.max(1, Math.ceil(this.totalCount / this.pageSize));
  }

  ngOnInit(): void {
    this.loadHotels();
  }

  loadHotels(): void {
    this.loading.set(true);
    this.errorMessage.set('');
    this.adminService.getHotels(this.searchText.trim(), this.statusFilter, this.page).subscribe({
      next: (result) => {
        this.hotels.set(result.items);
        this.page = result.page;
        this.pageSize = result.pageSize;
        this.totalCount = result.totalCount;
        this.loading.set(false);
      },
      error: () => {
        this.loading.set(false);
        this.errorMessage.set('Unable to load hotels. Try again.');
      }
    });
  }

  filterByStatus(status: HotelStatus | null): void {
    this.statusFilter = status;
    this.page = 1;
    this.loadHotels();
  }

  updateStatus(hotel: HotelSummary, status: HotelStatus): void {
    if (hotel.status === status) {
      return;
    }

    this.adminService.setHotelStatus(hotel.hotelId, status).subscribe({
      next: () => this.loadHotels(),
      error: (error: unknown) => {
        this.errorMessage.set(
          error instanceof HttpErrorResponse && error.status === 404
            ? 'This hotel could not be found.'
            : 'Unable to update the hotel status.'
        );
      }
    });
  }

  showDetails(id: number): void {
    this.selectedHotel.set(null);
    this.adminService.getHotel(id).subscribe({
      next: (hotel) => this.selectedHotel.set(hotel),
      error: () => this.errorMessage.set('Unable to load hotel details.')
    });
  }

  changePage(page: number): void {
    if (page < 1 || page > this.totalPages || page === this.page) {
      return;
    }
    this.page = page;
    this.loadHotels();
  }

  statusName(status: HotelStatus): string {
    return HotelStatus[status] ?? 'Unknown';
  }
}

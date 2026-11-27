import { Component, OnInit, signal } from '@angular/core';
import { finalize } from 'rxjs';
import { Hotel, HotelService } from '../../../../core/services/hotel.service';
import { RouterLink } from '@angular/router';
@Component({
  selector: 'app-hotel-list',
  standalone: true,
  imports: [RouterLink],
  templateUrl: './hotel-list.html',
  styleUrl: './hotel-list.css'
})
export class HotelList implements OnInit {

  hotels = signal<Hotel[]>([]);
  loading = signal(true);
  errorMessage = signal('');
  searchText = signal('');

  constructor(private hotelService: HotelService) {}

  ngOnInit(): void {
    this.loadHotels();
  }

  onSearchChange(event: Event): void {
    const input = event.target as HTMLInputElement;
    this.searchText.set(input.value);
  }

  searchHotels(): void {
    this.loadHotels(this.searchText());
  }

  clearSearch(): void {
    this.searchText.set('');
    this.loadHotels();
  }

  loadHotels(search?: string): void {

    this.loading.set(true);
    this.errorMessage.set('');

    this.hotelService.getHotels(search)
      .pipe(
        finalize(() => this.loading.set(false))
      )
      .subscribe({

        next: (data) => {
          this.hotels.set(data);
        },

        error: () => {
          this.hotels.set([]);
          this.errorMessage.set(
            'Unable to load hotels. Please try again.'
          );
        }

      });
  }
}
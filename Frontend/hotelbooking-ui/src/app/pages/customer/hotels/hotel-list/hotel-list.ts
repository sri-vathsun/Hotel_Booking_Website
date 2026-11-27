import { Component, OnInit, signal } from '@angular/core';
import { RouterLink } from '@angular/router';
import { Hotel, HotelService } from '../../../../core/services/hotel.service';

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
  city = signal('');
  minStarRating = signal('');

  constructor(private hotelService: HotelService) {}

  ngOnInit(): void {
    this.loadHotels();
  }

  loadHotels(): void {

    this.loading.set(true);
    this.errorMessage.set('');

    const rating = this.minStarRating()
      ? Number(this.minStarRating())
      : null;

    this.hotelService.getHotels(
      this.searchText(),
      this.city(),
      rating
    ).subscribe({

      next: (data) => {
        this.hotels.set(data);
        this.loading.set(false);
      },

      error: () => {
        this.hotels.set([]);
        this.errorMessage.set('Unable to load hotels. Please try again.');
        this.loading.set(false);
      }

    });
  }

  onSearchChange(value: string): void {
    this.searchText.set(value);
  }

  onCityChange(value: string): void {
    this.city.set(value);
  }

  onRatingChange(value: string): void {
    this.minStarRating.set(value);
  }

  applyFilters(): void {
    this.loadHotels();
  }

  clearFilters(): void {
    this.searchText.set('');
    this.city.set('');
    this.minStarRating.set('');
    this.loadHotels();
  }
}
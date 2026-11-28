import { Component, OnInit, signal } from '@angular/core';
import { Hotel, HotelService } from '../../../../core/services/hotel.service';

@Component({
  selector: 'app-hotel-list',
  standalone: true,
  imports: [],
  templateUrl: './hotel-list.html',
  styleUrl: './hotel-list.css'
})
export class HotelList implements OnInit {

  hotels = signal<Hotel[]>([]);
  loading = signal(true);
  errorMessage = signal('');

  constructor(private hotelService: HotelService) {}

  ngOnInit(): void {
    this.loadHotels();
  }

  loadHotels(): void {

    this.loading.set(true);
    this.errorMessage.set('');

    this.hotelService.getHotels().subscribe({

      next: (data) => {
        this.hotels.set(data);
        this.loading.set(false);
      },

      error: () => {
        this.errorMessage.set('Unable to load hotels. Please try again.');
        this.loading.set(false);
      }

    });
  }
}
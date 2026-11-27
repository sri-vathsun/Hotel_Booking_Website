import { Component, OnInit, signal } from '@angular/core';
import { ActivatedRoute, RouterLink } from '@angular/router';
import { Hotel, HotelService } from '../../../../core/services/hotel.service';

@Component({
  selector: 'app-hotel-details',
  standalone: true,
  imports: [RouterLink],
  templateUrl: './hotel-details.html',
  styleUrl: './hotel-details.css'
})
export class HotelDetails implements OnInit {

  hotel = signal<Hotel | null>(null);
  loading = signal(true);
  errorMessage = signal('');

  constructor(
    private route: ActivatedRoute,
    private hotelService: HotelService
  ) {}

  ngOnInit(): void {
    const hotelId = Number(this.route.snapshot.paramMap.get('id'));

    if (!hotelId || hotelId <= 0) {
      this.errorMessage.set('Invalid hotel ID.');
      this.loading.set(false);
      return;
    }

    this.loadHotel(hotelId);
  }

  loadHotel(hotelId: number): void {

    this.loading.set(true);
    this.errorMessage.set('');

    this.hotelService.getHotelById(hotelId).subscribe({

      next: (data) => {
        this.hotel.set(data);
        this.loading.set(false);
      },

      error: (error) => {
        this.hotel.set(null);

        if (error.status === 404) {
          this.errorMessage.set('Hotel not found.');
        } else {
          this.errorMessage.set('Unable to load hotel details. Please try again.');
        }

        this.loading.set(false);
      }

    });
  }
}

import { Component, OnInit, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ActivatedRoute, RouterLink } from '@angular/router';
import {
  CustomerHotel,
  CustomerHotelService
} from '../../../../core/services/customer/customer-hotel.service';

@Component({
  selector: 'app-hotel-details',
  standalone: true,
  imports: [CommonModule, RouterLink],
  templateUrl: './hotel-details.html',
  styleUrl: './hotel-details.css'
})
export class HotelDetails implements OnInit {

  hotel = signal<CustomerHotel | null>(null);
  loading = signal(true);
  errorMessage = signal('');

  constructor(
    private route: ActivatedRoute,
    private hotelService: CustomerHotelService
  ) {}

  ngOnInit(): void {
    const id = Number(this.route.snapshot.paramMap.get('id'));

    if (!id || id <= 0) {
      this.errorMessage.set('Invalid hotel ID.');
      this.loading.set(false);
      return;
    }

    this.hotelService.getHotelById(id).subscribe({
      next: data => {
        this.hotel.set(data);
        this.loading.set(false);
      },
      error: () => {
        this.errorMessage.set('Unable to find this hotel.');
        this.loading.set(false);
      }
    });
  }
}

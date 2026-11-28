
import { Component, OnInit, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { RouterLink } from '@angular/router';
import {
  CustomerHotel,
  CustomerHotelService
} from '../../../../core/services/customer/customer-hotel.service';

@Component({
  selector: 'app-customer-hotel-list',
  standalone: true,
  imports: [CommonModule, FormsModule, RouterLink],
  templateUrl: './customer-hotel-list.html',
  styleUrl: './customer-hotel-list.css'
})
export class CustomerHotelList implements OnInit {

  hotels = signal<CustomerHotel[]>([]);
  loading = signal(false);
  errorMessage = signal('');

  searchText = '';
  city = '';
  minStarRating: number | null = null;

  constructor(private hotelService: CustomerHotelService) {}

  ngOnInit(): void {
    this.loadHotels();
  }

  loadHotels(): void {
    this.loading.set(true);
    this.errorMessage.set('');

    this.hotelService.getHotels(
      this.searchText.trim(),
      this.city.trim(),
      this.minStarRating
    ).subscribe({
      next: data => {
        this.hotels.set(data);
        this.loading.set(false);
      },
      error: () => {
        this.errorMessage.set('Unable to load hotels. Please try again.');
        this.loading.set(false);
      }
    });
  }

  clearSearch(): void {
    this.searchText = '';
    this.city = '';
    this.minStarRating = null;
    this.loadHotels();
  }
}

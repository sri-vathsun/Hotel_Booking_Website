import { HttpClient, HttpParams } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';

export interface Hotel {
  hotelId: number;
  name: string;
  description: string;
  address: string;
  city: string;
  state: string;
  country: string;
  starRating: number;
}

@Injectable({
  providedIn: 'root'
})
export class HotelService {

  private apiUrl = 'http://localhost:5269/api/hotels';

  constructor(private http: HttpClient) {}

  getHotels(
    search: string = '',
    city: string = '',
    minStarRating: number | null = null
  ): Observable<Hotel[]> {

    let params = new HttpParams();

    if (search.trim()) {
      params = params.set('search', search.trim());
    }

    if (city.trim()) {
      params = params.set('city', city.trim());
    }

    if (minStarRating !== null) {
      params = params.set('minStarRating', minStarRating);
    }

    return this.http.get<Hotel[]>(this.apiUrl, { params });
  }

  getHotelById(hotelId: number): Observable<Hotel> {
    return this.http.get<Hotel>(`${this.apiUrl}/${hotelId}`);
  }
}

import { HttpClient, HttpParams } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';

export interface CustomerHotel {
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
export class CustomerHotelService {

  private apiUrl = '/api/Hotels';

  constructor(private http: HttpClient) {}

  getHotels(
    search?: string,
    city?: string,
    minStarRating?: number | null
  ): Observable<CustomerHotel[]> {

    let params = new HttpParams();

    if (search) {
      params = params.set('search', search);
    }

    if (city) {
      params = params.set('city', city);
    }

    if (minStarRating !== null && minStarRating !== undefined) {
      params = params.set('minStarRating', minStarRating);
    }

    return this.http.get<CustomerHotel[]>(this.apiUrl, { params });
  }

  getHotelById(id: number): Observable<CustomerHotel> {
    return this.http.get<CustomerHotel>(`${this.apiUrl}/${id}`);
  }
}

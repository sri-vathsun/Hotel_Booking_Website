
import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

export interface MyBooking {
  bookingId: number;
  hotelName: string;
  roomNumber: string;
  roomType: string;
  checkInDate: string;
  checkOutDate: string;
  numberOfGuests: number;
  totalAmount: number;
  status: number;
  bookedAt: string;
}

export interface CustomerProfile {
  userId: number;
  fullName: string;
  email: string;
  phoneNumber: string | null;
}

export interface UpdateCustomerProfile {
  fullName: string;
  phoneNumber: string;
}

@Injectable({
  providedIn: 'root'
})
export class CustomerAccountService {

  private bookingUrl = '/api/customer/bookings';
  private profileUrl = '/api/customer/profile';

  constructor(private http: HttpClient) {}

  getMyBookings(): Observable<MyBooking[]> {
    return this.http.get<MyBooking[]>(
      `${this.bookingUrl}/my-bookings`
    );
  }

  cancelBooking(id: number): Observable<any> {
    return this.http.patch(
      `${this.bookingUrl}/${id}/cancel`,
      {}
    );
  }

  getProfile(): Observable<CustomerProfile> {
    return this.http.get<CustomerProfile>(
      this.profileUrl
    );
  }

  updateProfile(data: UpdateCustomerProfile): Observable<CustomerProfile> {
    return this.http.put<CustomerProfile>(
      this.profileUrl,
      data
    );
  }
}

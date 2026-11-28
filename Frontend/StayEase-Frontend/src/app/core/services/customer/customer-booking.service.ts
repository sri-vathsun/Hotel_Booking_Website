
import { HttpClient, HttpParams } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';

export interface AvailableRoom {
  roomId: number;
  hotelId: number;
  roomNumber: string;
  roomType: string;
  capacity: number;
  pricePerNight: number;
  description: string | null;
}

export interface CreateBooking {
  roomId: number;
  checkInDate: string;
  checkOutDate: string;
  numberOfGuests: number;
}

export interface BookingConfirmation {
  bookingId: number;
  hotelName: string;
  roomNumber: string;
  roomType: string;
  checkInDate: string;
  checkOutDate: string;
  numberOfGuests: number;
  numberOfNights: number;
  pricePerNight: number;
  totalAmount: number;
  status: number;
  bookedAt: string;
}

@Injectable({
  providedIn: 'root'
})
export class CustomerBookingService {

  private readonly roomsUrl = '/api/customer/rooms';
  private readonly bookingsUrl = '/api/customer/bookings';

  constructor(private http: HttpClient) {}

  getAvailableRooms(
    hotelId: number,
    checkIn: string,
    checkOut: string,
    guests: number
  ): Observable<AvailableRoom[]> {

    const params = new HttpParams()
      .set('hotelId', hotelId)
      .set('checkIn', `${checkIn}T00:00:00`)
      .set('checkOut', `${checkOut}T00:00:00`)
      .set('guests', guests);

    return this.http.get<AvailableRoom[]>(
      `${this.roomsUrl}/availability`,
      { params }
    );
  }

  createBooking(data: CreateBooking): Observable<BookingConfirmation> {
    return this.http.post<BookingConfirmation>(
      this.bookingsUrl,
      data
    );
  }

  getBookingConfirmation(id: number): Observable<BookingConfirmation> {
    return this.http.get<BookingConfirmation>(
      `${this.bookingsUrl}/${id}/confirmation`
    );
  }
}

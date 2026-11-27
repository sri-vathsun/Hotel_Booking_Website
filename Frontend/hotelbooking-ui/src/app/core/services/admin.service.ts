import { HttpClient, HttpHeaders, HttpParams } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { AdminAuthService } from './admin-auth.service';

export interface AdminDashboardStats {
  totalCustomers: number;
  totalHotelManagers: number;
  totalHotels: number;
  pendingApprovals: number;
  totalRooms: number;
  totalBookings: number;
  pendingBookings: number;
}

export interface PagedResult<T> {
  items: T[];
  page: number;
  pageSize: number;
  totalCount: number;
}

export interface UserSummary {
  userId: number;
  fullName: string;
  email: string;
  phoneNumber: string | null;
  role: number;
  isActive: boolean;
  createdAt: string;
}

export interface HotelSummary {
  hotelId: number;
  hotelName: string;
  city: string;
  country: string;
  managerName: string;
  status: HotelStatus;
  rating: number;
  roomCount: number;
}

export interface HotelDetail {
  hotelId: number;
  hotelName: string;
  description: string | null;
  address: string;
  city: string;
  state: string;
  country: string;
  postalCode: string | null;
  contactNumber: string;
  email: string | null;
  managerName: string;
  status: HotelStatus;
  rating: number;
  rooms: RoomDetail[];
  facilities: string[];
}

export interface RoomDetail {
  roomId: number;
  roomNumber: string;
  roomType: string;
  description: string | null;
  capacity: number;
  pricePerNight: number;
  status: number;
}

export interface BookingSummary {
  bookingId: number;
  customerName: string;
  hotelName: string;
  roomNumber: string;
  checkInDate: string;
  checkOutDate: string;
  totalAmount: number;
  status: BookingStatus;
}

export interface BookingDetail extends BookingSummary {
  customerId: number;
  customerEmail: string;
  hotelId: number;
  roomId: number;
  numberOfGuests: number;
  bookedAt: string;
}

export interface Facility {
  facilityId: number;
  facilityName: string;
  description: string | null;
}

export interface Category {
  categoryId: number;
  categoryName: string;
  description: string | null;
  hotelCount: number;
}

export interface ReportPoint {
  label: string;
  bookings: number;
  revenue: number;
}

export interface AdminProfile {
  userId: number;
  fullName: string;
  email: string;
  phoneNumber: string | null;
  role: number;
}

export interface CreateCatalogItem {
  facilityName?: string;
  categoryName?: string;
  description: string | null;
}

export interface ChangeAdminProfile {
  fullName: string;
  email: string;
  phoneNumber: string | null;
}

export interface ChangeAdminPassword {
  currentPassword: string;
  newPassword: string;
}

export enum HotelStatus {
  Pending = 1,
  Approved = 2,
  Rejected = 3,
  Inactive = 4
}

export enum BookingStatus {
  Pending = 1,
  Confirmed = 2,
  Rejected = 3,
  Cancelled = 4,
  CheckedIn = 5,
  CheckedOut = 6
}

@Injectable({
  providedIn: 'root'
})
export class AdminService {
  private readonly apiUrl = '/api/admin';

  constructor(
    private http: HttpClient,
    private auth: AdminAuthService
  ) {}

  getDashboard(): Observable<AdminDashboardStats> {
    return this.http.get<AdminDashboardStats>(
      `${this.apiUrl}/dashboard`,
      this.options()
    );
  }

  getUsers(
    resource: 'customers' | 'managers',
    search: string,
    page: number
  ): Observable<PagedResult<UserSummary>> {
    return this.http.get<PagedResult<UserSummary>>(
      `${this.apiUrl}/${resource}`,
      {
        ...this.options(),
        params: new HttpParams()
          .set('page', page)
          .set('pageSize', 20)
          .set('search', search)
      }
    );
  }

  setUserActive(
    resource: 'customers' | 'managers',
    id: number,
    isActive: boolean
  ): Observable<void> {
    return this.http.patch<void>(
      `${this.apiUrl}/${resource}/${id}/status`,
      { isActive },
      this.options()
    );
  }

  getHotels(
    search: string,
    status: HotelStatus | null,
    page: number
  ): Observable<PagedResult<HotelSummary>> {
    let params = new HttpParams()
      .set('page', page)
      .set('pageSize', 20)
      .set('search', search);
    if (status !== null) {
      params = params.set('status', status);
    }

    return this.http.get<PagedResult<HotelSummary>>(
      `${this.apiUrl}/hotels`,
      { ...this.options(), params }
    );
  }

  getHotel(id: number): Observable<HotelDetail> {
    return this.http.get<HotelDetail>(
      `${this.apiUrl}/hotels/${id}`,
      this.options()
    );
  }

  setHotelStatus(id: number, status: HotelStatus): Observable<void> {
    return this.http.patch<void>(
      `${this.apiUrl}/hotels/${id}/status`,
      null,
      {
        ...this.options(),
        params: new HttpParams().set('status', status)
      }
    );
  }

  getBookings(
    search: string,
    status: BookingStatus | null,
    page: number
  ): Observable<PagedResult<BookingSummary>> {
    let params = new HttpParams()
      .set('page', page)
      .set('pageSize', 20)
      .set('search', search);
    if (status !== null) {
      params = params.set('status', status);
    }

    return this.http.get<PagedResult<BookingSummary>>(
      `${this.apiUrl}/bookings`,
      { ...this.options(), params }
    );
  }

  getBooking(id: number): Observable<BookingDetail> {
    return this.http.get<BookingDetail>(
      `${this.apiUrl}/bookings/${id}`,
      this.options()
    );
  }

  setBookingStatus(id: number, status: BookingStatus): Observable<void> {
    return this.http.patch<void>(
      `${this.apiUrl}/bookings/${id}/status`,
      { status },
      this.options()
    );
  }

  getFacilities(page: number): Observable<PagedResult<Facility>> {
    return this.http.get<PagedResult<Facility>>(
      `${this.apiUrl}/facilities`,
      {
        ...this.options(),
        params: new HttpParams().set('page', page).set('pageSize', 20)
      }
    );
  }

  createFacility(item: CreateCatalogItem): Observable<Facility> {
    return this.http.post<Facility>(
      `${this.apiUrl}/facilities`,
      { facilityName: item.facilityName, description: item.description },
      this.options()
    );
  }

  updateFacility(id: number, item: CreateCatalogItem): Observable<Facility> {
    return this.http.put<Facility>(
      `${this.apiUrl}/facilities/${id}`,
      { facilityName: item.facilityName, description: item.description },
      this.options()
    );
  }

  deleteFacility(id: number): Observable<void> {
    return this.http.delete<void>(
      `${this.apiUrl}/facilities/${id}`,
      this.options()
    );
  }

  getCategories(page: number): Observable<PagedResult<Category>> {
    return this.http.get<PagedResult<Category>>(
      `${this.apiUrl}/categories`,
      {
        ...this.options(),
        params: new HttpParams().set('page', page).set('pageSize', 20)
      }
    );
  }

  createCategory(item: CreateCatalogItem): Observable<Category> {
    return this.http.post<Category>(
      `${this.apiUrl}/categories`,
      { categoryName: item.categoryName, description: item.description },
      this.options()
    );
  }

  updateCategory(id: number, item: CreateCatalogItem): Observable<Category> {
    return this.http.put<Category>(
      `${this.apiUrl}/categories/${id}`,
      { categoryName: item.categoryName, description: item.description },
      this.options()
    );
  }

  deleteCategory(id: number): Observable<void> {
    return this.http.delete<void>(
      `${this.apiUrl}/categories/${id}`,
      this.options()
    );
  }

  getMonthlyReport(year: number): Observable<ReportPoint[]> {
    return this.http.get<ReportPoint[]>(
      `${this.apiUrl}/reports/monthly`,
      {
        ...this.options(),
        params: new HttpParams().set('year', year)
      }
    );
  }

  getPopularHotels(): Observable<ReportPoint[]> {
    return this.http.get<ReportPoint[]>(
      `${this.apiUrl}/reports/popular-hotels`,
      this.options()
    );
  }

  getProfile(): Observable<AdminProfile> {
    return this.http.get<AdminProfile>(
      `${this.apiUrl}/profile`,
      this.options()
    );
  }

  updateProfile(profile: ChangeAdminProfile): Observable<AdminProfile> {
    return this.http.put<AdminProfile>(
      `${this.apiUrl}/profile`,
      profile,
      this.options()
    );
  }

  changePassword(password: ChangeAdminPassword): Observable<void> {
    return this.http.put<void>(
      `${this.apiUrl}/profile/password`,
      password,
      this.options()
    );
  }

  private options(): { headers: HttpHeaders } {
    const token = this.auth.getToken();
    return {
      headers: new HttpHeaders(
        token ? { Authorization: `Bearer ${token}` } : {}
      )
    };
  }
}

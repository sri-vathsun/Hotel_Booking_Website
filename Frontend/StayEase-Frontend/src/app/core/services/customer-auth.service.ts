
import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable, map } from 'rxjs';

export interface CustomerAuthResponse {
  token: string;
  userId: number;
  fullName: string;
  email: string;
  role: string;
}

export interface CustomerRegister {
  fullName: string;
  email: string;
  password: string;
  phoneNumber: string;
}

@Injectable({
  providedIn: 'root'
})
export class CustomerAuthService {

  private readonly apiUrl = '/api/auth';

  private readonly tokenKey = 'stayease_customer_token';
  private readonly roleKey = 'stayease_customer_role';
  private readonly nameKey = 'stayease_customer_name';

  constructor(private http: HttpClient) {}

  login(email: string, password: string): Observable<CustomerAuthResponse> {

    return this.http
      .post<CustomerAuthResponse>(`${this.apiUrl}/login`, {
        email,
        password
      })
      .pipe(
        map((response) => {

          if (response.role !== 'Customer') {
            throw new Error('This account does not have Customer access.');
          }

          sessionStorage.setItem(this.tokenKey, response.token);
          sessionStorage.setItem(this.roleKey, response.role);
          sessionStorage.setItem(this.nameKey, response.fullName);

          return response;
        })
      );
  }

  register(data: CustomerRegister): Observable<any> {
    return this.http.post(
      `${this.apiUrl}/register`,
      data
    );
  }

  getToken(): string | null {
    return sessionStorage.getItem(this.tokenKey);
  }

  isCustomerAuthenticated(): boolean {
    return (
      Boolean(this.getToken()) &&
      sessionStorage.getItem(this.roleKey) === 'Customer'
    );
  }

  getCustomerName(): string {
    return sessionStorage.getItem(this.nameKey) ?? 'Customer';
  }

  logout(): void {
    sessionStorage.removeItem(this.tokenKey);
    sessionStorage.removeItem(this.roleKey);
    sessionStorage.removeItem(this.nameKey);
  }
}

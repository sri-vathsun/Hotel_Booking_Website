import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable, map } from 'rxjs';

export interface AdminLoginResponse {
  token: string;
  userId: number;
  fullName: string;
  email: string;
  role: string;
}

@Injectable({
  providedIn: 'root'
})
export class AdminAuthService {
  private readonly apiUrl = '/api/auth';
  private readonly tokenKey = 'stayease_admin_token';
  private readonly roleKey = 'stayease_admin_role';
  private readonly nameKey = 'stayease_admin_name';

  constructor(private http: HttpClient) {}

  login(email: string, password: string): Observable<AdminLoginResponse> {
    return this.http
      .post<AdminLoginResponse>(`${this.apiUrl}/login`, { email, password })
      .pipe(
        map((response) => {
          if (response.role !== 'Admin') {
            throw new Error('This account does not have admin access.');
          }

          sessionStorage.setItem(this.tokenKey, response.token);
          sessionStorage.setItem(this.roleKey, response.role);
          sessionStorage.setItem(this.nameKey, response.fullName);
          return response;
        })
      );
  }

  isAdminAuthenticated(): boolean {
    return (
      Boolean(this.getToken()) &&
      sessionStorage.getItem(this.roleKey) === 'Admin'
    );
  }

  getToken(): string | null {
    return sessionStorage.getItem(this.tokenKey);
  }

  getAdminName(): string {
    return sessionStorage.getItem(this.nameKey) ?? 'Administrator';
  }

  updateAdminName(name: string): void {
    sessionStorage.setItem(this.nameKey, name);
  }

  logout(): void {
    sessionStorage.removeItem(this.tokenKey);
    sessionStorage.removeItem(this.roleKey);
    sessionStorage.removeItem(this.nameKey);
  }
}

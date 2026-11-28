
import { HttpErrorResponse } from '@angular/common/http';
import { Component, signal } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { Router, RouterLink } from '@angular/router';

import { AdminAuthService } from '../../../core/services/admin-auth.service';
import { CustomerAuthService } from '../../../core/services/customer-auth.service';

@Component({
  selector: 'app-login',
  standalone: true,
  imports: [FormsModule, RouterLink],
  templateUrl: './login.html',
  styleUrl: './login.css'
})
export class Login {

  loginType: 'Customer' | 'Admin' = 'Customer';

  email = '';
  password = '';

  loading = signal(false);
  errorMessage = signal('');

  constructor(
    private adminAuth: AdminAuthService,
    private customerAuth: CustomerAuthService,
    private router: Router
  ) {}

  switchLogin(type: 'Customer' | 'Admin'): void {
    this.loginType = type;
    this.email = '';
    this.password = '';
    this.errorMessage.set('');
  }

  submit(): void {

    if (this.loading()) {
      return;
    }

    this.loading.set(true);
    this.errorMessage.set('');

    const loginRequest =
      this.loginType === 'Admin'
        ? this.adminAuth.login(this.email.trim(), this.password)
        : this.customerAuth.login(this.email.trim(), this.password);

    loginRequest.subscribe({
      next: () => {

        this.loading.set(false);

        if (this.loginType === 'Admin') {
          void this.router.navigate(['/admin/dashboard']);
        } else {
          void this.router.navigate(['/customer/dashboard']);
        }
      },

      error: (error: unknown) => {
        this.loading.set(false);
        this.errorMessage.set(this.getErrorMessage(error));
      }
    });
  }

  private getErrorMessage(error: unknown): string {

    if (error instanceof Error && !(error instanceof HttpErrorResponse)) {
      return error.message;
    }

    if (error instanceof HttpErrorResponse) {

      if (error.status === 401) {
        return 'Invalid email or password.';
      }

      if (error.status === 400) {
        return error.error?.message || 'Please check your login details.';
      }
    }

    return 'Unable to sign in. Check that the API is running and try again.';
  }
}

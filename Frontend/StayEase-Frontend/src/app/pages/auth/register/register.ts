
import { HttpErrorResponse } from '@angular/common/http';
import { Component, signal } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { Router, RouterLink } from '@angular/router';

import { CustomerAuthService } from '../../../core/services/customer-auth.service';

@Component({
  selector: 'app-register',
  standalone: true,
  imports: [FormsModule, RouterLink],
  templateUrl: './register.html',
  styleUrl: './register.css'
})
export class Register {

  fullName = '';
  email = '';
  phoneNumber = '';
  password = '';
  confirmPassword = '';

  loading = signal(false);
  errorMessage = signal('');
  successMessage = signal('');

  constructor(
    private auth: CustomerAuthService,
    private router: Router
  ) {}

  submit(): void {

    if (this.loading()) {
      return;
    }

    this.errorMessage.set('');
    this.successMessage.set('');

    if (this.password !== this.confirmPassword) {
      this.errorMessage.set('Passwords do not match.');
      return;
    }

    if (!this.fullName.trim()) {
      this.errorMessage.set('Full name is required.');
      return;
    }

    this.loading.set(true);

    this.auth.register({
      fullName: this.fullName.trim(),
      email: this.email.trim(),
      phoneNumber: this.phoneNumber.trim(),
      password: this.password
    }).subscribe({
      next: () => {
        this.loading.set(false);
        this.successMessage.set(
          'Registration successful. You can now sign in.'
        );

        setTimeout(() => {
          void this.router.navigate(['/login']);
        }, 1200);
      },
      error: (error: unknown) => {
        this.loading.set(false);

        if (error instanceof HttpErrorResponse) {
          this.errorMessage.set(
            error.error?.message ||
            error.error?.title ||
            'Registration failed. Please check your details.'
          );
        } else {
          this.errorMessage.set('Unable to register. Please try again.');
        }
      }
    });
  }
}

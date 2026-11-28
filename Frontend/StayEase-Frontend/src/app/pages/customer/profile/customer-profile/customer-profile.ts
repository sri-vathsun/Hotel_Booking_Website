
import { Component, OnInit, signal } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { CommonModule } from '@angular/common';

import {
  CustomerAccountService,
  CustomerProfile as CustomerProfileModel
} from '../../../../core/services/customer/customer-account.service';

@Component({
  selector: 'app-customer-profile',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './customer-profile.html',
  styleUrl: './customer-profile.css'
})
export class CustomerProfile implements OnInit {

  profile = signal<CustomerProfileModel | null>(null);

  fullName = '';
  phoneNumber = '';

  loading = signal(true);
  saving = signal(false);
  errorMessage = signal('');
  successMessage = signal('');

  constructor(
    private accountService: CustomerAccountService
  ) {}

  ngOnInit(): void {
    this.loadProfile();
  }

  loadProfile(): void {
    this.loading.set(true);
    this.errorMessage.set('');

    this.accountService.getProfile().subscribe({
      next: (data) => {
        this.profile.set(data);
        this.fullName = data.fullName;
        this.phoneNumber = data.phoneNumber || '';
        this.loading.set(false);
      },
      error: (error) => {
        this.loading.set(false);

        if (error.status === 401 || error.status === 403) {
          this.errorMessage.set(
            'Please sign in with your Customer account to view your profile.'
          );
        } else {
          this.errorMessage.set(
            'Unable to load your profile.'
          );
        }
      }
    });
  }

  updateProfile(): void {

    if (!this.fullName.trim()) {
      this.errorMessage.set('Full name is required.');
      return;
    }

    if (this.fullName.trim().length > 100) {
      this.errorMessage.set('Full name cannot exceed 100 characters.');
      return;
    }

    if (this.phoneNumber.length > 15) {
      this.errorMessage.set('Phone number cannot exceed 15 characters.');
      return;
    }

    this.saving.set(true);
    this.errorMessage.set('');
    this.successMessage.set('');

    this.accountService.updateProfile({
      fullName: this.fullName.trim(),
      phoneNumber: this.phoneNumber.trim()
    }).subscribe({
      next: (data) => {
        this.profile.set(data);
        this.fullName = data.fullName;
        this.phoneNumber = data.phoneNumber || '';

        this.saving.set(false);
        this.successMessage.set(
          'Your profile has been updated successfully.'
        );
      },
      error: (error) => {
        this.saving.set(false);

        if (error.status === 401 || error.status === 403) {
          this.errorMessage.set(
            'Please sign in with your Customer account.'
          );
        } else {
          this.errorMessage.set(
            error.error?.message ||
            'Unable to update your profile.'
          );
        }
      }
    });
  }
}

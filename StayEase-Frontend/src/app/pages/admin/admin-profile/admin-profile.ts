import { HttpErrorResponse } from '@angular/common/http';
import { Component, OnInit, signal } from '@angular/core';
import { FormsModule } from '@angular/forms';
import {
  AdminProfile as AdminProfileData,
  AdminService
} from '../../../core/services/admin.service';
import { AdminAuthService } from '../../../core/services/admin-auth.service';

@Component({
  selector: 'app-admin-profile',
  standalone: true,
  imports: [FormsModule],
  templateUrl: './admin-profile.html',
  styleUrl: './admin-profile.css'
})
export class AdminProfile implements OnInit {
  profile = signal<AdminProfileData | null>(null);
  loading = signal(true);
  savingProfile = signal(false);
  savingPassword = signal(false);
  errorMessage = signal('');
  successMessage = signal('');
  fullName = '';
  email = '';
  phoneNumber = '';
  currentPassword = '';
  newPassword = '';
  confirmPassword = '';

  constructor(
    private adminService: AdminService,
    private auth: AdminAuthService
  ) {}

  ngOnInit(): void {
    this.loadProfile();
  }

  loadProfile(): void {
    this.loading.set(true);
    this.errorMessage.set('');
    this.adminService.getProfile().subscribe({
      next: (profile) => {
        this.profile.set(profile);
        this.fullName = profile.fullName;
        this.email = profile.email;
        this.phoneNumber = profile.phoneNumber ?? '';
        this.loading.set(false);
      },
      error: () => {
        this.loading.set(false);
        this.errorMessage.set('Unable to load your admin profile.');
      }
    });
  }

  saveProfile(): void {
    this.savingProfile.set(true);
    this.errorMessage.set('');
    this.successMessage.set('');
    this.adminService.updateProfile({
      fullName: this.fullName.trim(),
      email: this.email.trim(),
      phoneNumber: this.phoneNumber.trim() || null
    }).subscribe({
      next: (profile) => {
        this.profile.set(profile);
        this.auth.updateAdminName(profile.fullName);
        this.savingProfile.set(false);
        this.successMessage.set('Profile updated.');
      },
      error: (error: unknown) => {
        this.savingProfile.set(false);
        this.errorMessage.set(
          error instanceof HttpErrorResponse && error.status === 409
            ? 'That email address is already in use.'
            : 'Unable to save your profile.'
        );
      }
    });
  }

  changePassword(): void {
    this.errorMessage.set('');
    this.successMessage.set('');
    if (this.newPassword !== this.confirmPassword) {
      this.errorMessage.set('The new passwords do not match.');
      return;
    }

    this.savingPassword.set(true);
    this.adminService.changePassword({
      currentPassword: this.currentPassword,
      newPassword: this.newPassword
    }).subscribe({
      next: () => {
        this.savingPassword.set(false);
        this.currentPassword = '';
        this.newPassword = '';
        this.confirmPassword = '';
        this.successMessage.set('Password changed.');
      },
      error: (error: unknown) => {
        this.savingPassword.set(false);
        this.errorMessage.set(
          error instanceof HttpErrorResponse && error.status === 400
            ? 'The current password is incorrect.'
            : 'Unable to change your password.'
        );
      }
    });
  }
}

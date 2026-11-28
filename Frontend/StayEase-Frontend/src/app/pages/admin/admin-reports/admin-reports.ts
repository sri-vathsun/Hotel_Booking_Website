import { HttpErrorResponse } from '@angular/common/http';
import { CommonModule } from '@angular/common';
import { Component, OnInit, signal } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { forkJoin } from 'rxjs';
import { AdminService, ReportPoint } from '../../../core/services/admin.service';

@Component({
  selector: 'app-admin-reports',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './admin-reports.html',
  styleUrl: './admin-reports.css'
})
export class AdminReports implements OnInit {
  monthly = signal<ReportPoint[]>([]);
  popularHotels = signal<ReportPoint[]>([]);
  loading = signal(false);
  errorMessage = signal('');
  year = new Date().getFullYear();

  constructor(private adminService: AdminService) {}

  ngOnInit(): void {
    this.loadReports();
  }

  loadReports(): void {
    this.loading.set(true);
    this.errorMessage.set('');
    forkJoin({
      monthly: this.adminService.getMonthlyReport(this.year),
      popularHotels: this.adminService.getPopularHotels()
    }).subscribe({
      next: (result) => {
        this.monthly.set(result.monthly);
        this.popularHotels.set(result.popularHotels);
        this.loading.set(false);
      },
      error: (error: unknown) => {
        this.loading.set(false);
        this.errorMessage.set(
          error instanceof HttpErrorResponse && error.status === 403
            ? 'Your account cannot access reports.'
            : 'Unable to load reports. Check the API connection and try again.'
        );
      }
    });
  }

  monthName(label: string): string {
    const month = Number(label);
    return Number.isInteger(month) && month >= 1 && month <= 12
      ? new Intl.DateTimeFormat('en', { month: 'long' }).format(new Date(2024, month - 1, 1))
      : label;
  }

  revenueWidth(revenue: number): number {
    const maximum = Math.max(1, ...this.monthly().map((item) => item.revenue));
    return Math.max(3, (revenue / maximum) * 100);
  }

  get totalRevenue(): number {
    return this.monthly().reduce((total, item) => total + item.revenue, 0);
  }

  get totalBookings(): number {
    return this.monthly().reduce((total, item) => total + item.bookings, 0);
  }
}

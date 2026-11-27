import { HttpErrorResponse } from '@angular/common/http';
import { CommonModule } from '@angular/common';
import { Component, OnInit, signal } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { ActivatedRoute } from '@angular/router';
import { AdminService, PagedResult, UserSummary } from '../../../core/services/admin.service';

type UserResource = 'customers' | 'managers';

@Component({
  selector: 'app-admin-users',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './admin-users.html',
  styleUrl: './admin-users.css'
})
export class AdminUsers implements OnInit {
  users = signal<UserSummary[]>([]);
  loading = signal(false);
  errorMessage = signal('');
  resource: UserResource = 'customers';
  searchText = '';
  page = 1;
  pageSize = 20;
  totalCount = 0;

  constructor(
    private route: ActivatedRoute,
    private adminService: AdminService
  ) {}

  get title(): string {
    return this.resource === 'customers' ? 'Customers' : 'Hotel managers';
  }

  get totalPages(): number {
    return Math.max(1, Math.ceil(this.totalCount / this.pageSize));
  }

  ngOnInit(): void {
    this.route.paramMap.subscribe((params) => {
      const userType = params.get('userType');
      if (userType !== 'customers' && userType !== 'managers') {
        this.errorMessage.set('Unknown user list.');
        return;
      }

      this.resource = userType;
      this.page = 1;
      this.loadUsers();
    });
  }

  loadUsers(): void {
    this.loading.set(true);
    this.errorMessage.set('');
    this.adminService.getUsers(this.resource, this.searchText.trim(), this.page).subscribe({
      next: (result: PagedResult<UserSummary>) => {
        this.users.set(result.items);
        this.page = result.page;
        this.pageSize = result.pageSize;
        this.totalCount = result.totalCount;
        this.loading.set(false);
      },
      error: () => {
        this.loading.set(false);
        this.errorMessage.set('Unable to load users. Try again.');
      }
    });
  }

  setUserActive(user: UserSummary): void {
    const newStatus = !user.isActive;
    this.adminService.setUserActive(this.resource, user.userId, newStatus).subscribe({
      next: () => {
        this.users.update((items) =>
          items.map((item) =>
            item.userId === user.userId ? { ...item, isActive: newStatus } : item
          )
        );
      },
      error: (error: unknown) => {
        this.errorMessage.set(
          error instanceof HttpErrorResponse && error.status === 404
            ? 'This user could not be found.'
            : 'Unable to update this user’s status.'
        );
      }
    });
  }

  changePage(page: number): void {
    if (page < 1 || page > this.totalPages || page === this.page) {
      return;
    }

    this.page = page;
    this.loadUsers();
  }
}

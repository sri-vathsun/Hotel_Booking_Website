import { HttpErrorResponse } from '@angular/common/http';
import { Component, OnInit, signal } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { ActivatedRoute } from '@angular/router';
import { Observable } from 'rxjs';
import {
  AdminService,
  Category,
  CreateCatalogItem,
  Facility,
  PagedResult
} from '../../../core/services/admin.service';

type CatalogType = 'facilities' | 'categories';

interface CatalogEntry {
  id: number;
  name: string;
  description: string | null;
  linkedHotels: number | null;
}

@Component({
  selector: 'app-admin-catalog',
  standalone: true,
  imports: [FormsModule],
  templateUrl: './admin-catalog.html',
  styleUrl: './admin-catalog.css'
})
export class AdminCatalog implements OnInit {
  entries = signal<CatalogEntry[]>([]);
  loading = signal(false);
  saving = signal(false);
  errorMessage = signal('');
  successMessage = signal('');
  type: CatalogType = 'facilities';
  name = '';
  description = '';
  editingId: number | null = null;
  page = 1;
  pageSize = 20;
  totalCount = 0;

  constructor(
    private route: ActivatedRoute,
    private adminService: AdminService
  ) {}

  get title(): string {
    return this.type === 'facilities' ? 'Facilities' : 'Categories';
  }

  get singularTitle(): string {
    return this.type === 'facilities' ? 'facility' : 'category';
  }

  get totalPages(): number {
    return Math.max(1, Math.ceil(this.totalCount / this.pageSize));
  }

  ngOnInit(): void {
    this.route.paramMap.subscribe((params) => {
      const catalogType = params.get('catalogType');
      if (catalogType !== 'facilities' && catalogType !== 'categories') {
        this.errorMessage.set('Unknown catalog.');
        return;
      }

      this.type = catalogType;
      this.page = 1;
      this.resetForm();
      this.loadItems();
    });
  }

  loadItems(): void {
    this.loading.set(true);
    this.errorMessage.set('');
    if (this.type === 'facilities') {
      this.adminService.getFacilities(this.page).subscribe({
        next: (result) => this.setItems(result, (item) => this.toFacilityEntry(item)),
        error: () => this.handleLoadError()
      });
    } else {
      this.adminService.getCategories(this.page).subscribe({
        next: (result) => this.setItems(result, (item) => this.toCategoryEntry(item)),
        error: () => this.handleLoadError()
      });
    }
  }

  save(): void {
    if (this.saving() || !this.name.trim()) {
      return;
    }
    this.saving.set(true);
    this.errorMessage.set('');
    this.successMessage.set('');

    const payload: CreateCatalogItem = {
      ...(this.type === 'facilities'
        ? { facilityName: this.name.trim() }
        : { categoryName: this.name.trim() }),
      description: this.description.trim() || null
    };

    const action = this.editingId === null ? 'created' : 'updated';
    if (this.type === 'facilities') {
      const request = this.editingId === null
        ? this.adminService.createFacility(payload)
        : this.adminService.updateFacility(this.editingId, payload);
      this.saveRequest(request, action);
    } else {
      const request = this.editingId === null
        ? this.adminService.createCategory(payload)
        : this.adminService.updateCategory(this.editingId, payload);
      this.saveRequest(request, action);
    }
  }

  private saveRequest<T>(request: Observable<T>, action: string): void {
    request.subscribe({
      next: () => {
        this.saving.set(false);
        this.successMessage.set(`${this.singularTitle} ${action}.`);
        this.resetForm();
        this.loadItems();
      },
      error: (error: unknown) => {
        this.saving.set(false);
        this.errorMessage.set(
          error instanceof HttpErrorResponse && error.status === 409
            ? `A ${this.singularTitle} with that name already exists.`
            : `Unable to save this ${this.singularTitle}.`
        );
      }
    });
  }

  edit(entry: CatalogEntry): void {
    this.editingId = entry.id;
    this.name = entry.name;
    this.description = entry.description ?? '';
    this.errorMessage.set('');
    this.successMessage.set('');
  }

  cancelEdit(): void {
    this.resetForm();
  }

  remove(entry: CatalogEntry): void {
    if (!window.confirm(`Delete "${entry.name}"?`)) {
      return;
    }
    this.errorMessage.set('');

    const request = this.type === 'facilities'
      ? this.adminService.deleteFacility(entry.id)
      : this.adminService.deleteCategory(entry.id);

    request.subscribe({
      next: () => {
        this.successMessage.set(`${this.title.slice(0, -1)} deleted.`);
        this.loadItems();
      },
      error: () => this.errorMessage.set(`Unable to delete "${entry.name}". It may still be in use.`)
    });
  }

  changePage(page: number): void {
    if (page < 1 || page > this.totalPages || page === this.page) {
      return;
    }
    this.page = page;
    this.loadItems();
  }

  private toFacilityEntry(item: Facility): CatalogEntry {
    return {
      id: item.facilityId,
      name: item.facilityName,
      description: item.description,
      linkedHotels: null
    };
  }

  private toCategoryEntry(item: Category): CatalogEntry {
    return {
      id: item.categoryId,
      name: item.categoryName,
      description: item.description,
      linkedHotels: item.hotelCount
    };
  }

  private setItems<T>(
    result: PagedResult<T>,
    mapItem: (item: T) => CatalogEntry
  ): void {
    this.entries.set(result.items.map(mapItem));
    this.page = result.page;
    this.pageSize = result.pageSize;
    this.totalCount = result.totalCount;
    this.loading.set(false);
  }

  private handleLoadError(): void {
    this.loading.set(false);
    this.errorMessage.set(`Unable to load ${this.title.toLowerCase()}. Try again.`);
  }

  private resetForm(): void {
    this.editingId = null;
    this.name = '';
    this.description = '';
  }
}

import { Routes } from '@angular/router';
import { HotelList } from './pages/customer/hotels/hotel-list/hotel-list';
import { adminGuard } from './core/guards/admin.guard';
import { AdminLayout } from './pages/admin/admin-layout/admin-layout';
import { AdminDashboard } from './pages/admin/admin-dashboard/admin-dashboard';
import { AdminLogin } from './pages/admin/admin-login/admin-login';
import { AdminUsers } from './pages/admin/admin-users/admin-users';
import { AdminHotels } from './pages/admin/admin-hotels/admin-hotels';
import { AdminBookings } from './pages/admin/admin-bookings/admin-bookings';
import { AdminCatalog } from './pages/admin/admin-catalog/admin-catalog';
import { AdminReports } from './pages/admin/admin-reports/admin-reports';
import { AdminProfile } from './pages/admin/admin-profile/admin-profile';
import { CustomerLayout } from './pages/customer/layout/customer-layout';
import { CustomerDashboard } from './pages/customer/dashboard/customer-dashboard';
import { CustomerHotelList } from './pages/customer/hotels/customer-hotel-list/customer-hotel-list';
import { HotelDetails } from './pages/customer/hotels/hotel-details/hotel-details';

export const routes: Routes = [
  {
    path: 'admin/login',
    component: AdminLogin
  },
  {
    path: 'admin',
    component: AdminLayout,
    canActivate: [adminGuard],
    children: [
      { path: '', pathMatch: 'full', redirectTo: 'dashboard' },
      { path: 'dashboard', component: AdminDashboard },
      { path: 'users/:userType', component: AdminUsers },
      { path: 'hotels', component: AdminHotels },
      { path: 'bookings', component: AdminBookings },
      { path: 'catalog/:catalogType', component: AdminCatalog },
      { path: 'reports', component: AdminReports },
      { path: 'profile', component: AdminProfile }
    ]
  },{
  path: 'customer',
  component: CustomerLayout,
  children: [
    { path: '', pathMatch: 'full', redirectTo: 'dashboard' },
    { path: 'dashboard', component: CustomerDashboard },
    { path: 'hotels', component: CustomerHotelList },
    { path: 'hotels/:id', component: HotelDetails }
  ]
},
  {
    path: '',
    component: HotelList
  },
  {
    path: '**',
    redirectTo: ''
  }
];
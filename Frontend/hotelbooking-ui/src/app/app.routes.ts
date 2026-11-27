import { Routes } from '@angular/router';
import { HotelList } from './pages/customer/hotels/hotel-list/hotel-list';
import { HotelDetails } from './pages/customer/hotels/hotel-details/hotel-details';

export const routes: Routes = [
  {
    path: 'hotels/:id',
    component: HotelDetails
  },
  {
    path: '',
    component: HotelList
  }
];

import { HttpInterceptorFn } from '@angular/common/http';
import { inject } from '@angular/core';
import { CustomerAuthService } from '../services/customer-auth.service';

export const customerAuthInterceptor: HttpInterceptorFn = (req, next) => {

  const auth = inject(CustomerAuthService);

  // Attach Customer token only to Customer API requests.
  if (req.url.startsWith('/api/customer/')) {

    const token = auth.getToken();

    if (token) {
      const authRequest = req.clone({
        setHeaders: {
          Authorization: `Bearer ${token}`
        }
      });

      return next(authRequest);
    }
  }

  return next(req);
};

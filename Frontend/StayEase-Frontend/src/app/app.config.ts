import { ApplicationConfig } from '@angular/core';
import { provideRouter } from '@angular/router';
import { provideHttpClient } from '@angular/common/http';
import { withInterceptors } from '@angular/common/http';
import { customerAuthInterceptor } from './core/interceptors/customer-auth.interceptor';
//route 
import { routes } from './app.routes';

export const appConfig: ApplicationConfig = {
  providers: [
    provideRouter(routes),
    provideHttpClient(withInterceptors([customerAuthInterceptor]))
  ]
};
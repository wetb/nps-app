import { HttpErrorResponse, HttpInterceptorFn } from '@angular/common/http';
import { inject } from '@angular/core';
import { Router } from '@angular/router';
import { catchError, throwError } from 'rxjs';
import { TokenService } from '../services/token.service';

export const errorInterceptor: HttpInterceptorFn = (req, next) => {
  const router = inject(Router);
  const tokenService = inject(TokenService);
  
  return next(req).pipe(
    catchError((error: HttpErrorResponse) => {
      if (error.status === 401) {
        // Si no es una petición de login o refresh token
        if (!req.url.includes('/login') && !req.url.includes('/refresh')) {
          tokenService.clearTokens();
          router.navigate(['/auth/login'], { 
            queryParams: { unauthorized: true } 
          });
        }
      }
      
      if (error.status === 403) {
        router.navigate(['/auth/login'], { 
          queryParams: { forbidden: true } 
        });
      }

      // Mensaje de error personalizado
      let errorMessage = 'Ocurrió un error';
      if (error.error instanceof ErrorEvent) {
        // Error del cliente
        errorMessage = error.error.message;
      } else if (error.error && error.error.message) {
        // Error del servidor con mensaje
        errorMessage = error.error.message;
      } else if (error.message) {
        // Mensaje de error general
        errorMessage = error.message;
      }

      return throwError(() => new Error(errorMessage));
    })
  );
};

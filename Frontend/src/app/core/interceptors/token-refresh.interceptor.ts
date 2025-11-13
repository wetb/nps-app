import { HttpInterceptorFn } from '@angular/common/http';
import { inject } from '@angular/core';
import { catchError, switchMap, throwError } from 'rxjs';
import { AuthService } from '../services/auth.service';
import { TokenService } from '../services/token.service';

// Variable para controlar si estamos refrescando el token
let isRefreshing = false;

export const tokenRefreshInterceptor: HttpInterceptorFn = (req, next) => {
  const authService = inject(AuthService);
  const tokenService = inject(TokenService);
  
  // No interceptar peticiones de login, refresh o logout
  if (req.url.includes('/login') || 
      req.url.includes('/refresh') || 
      req.url.includes('/logout')) {
    return next(req);
  }

  // Verificar si el token está por expirar
  if (tokenService.isAuthenticated() && 
      !tokenService.isTokenExpired() && 
      tokenService.shouldRefreshToken() && 
      !isRefreshing) {
    
    isRefreshing = true;
    
    return authService.refreshToken().pipe(
      switchMap(() => {
        isRefreshing = false;
        
        // Clonar la petición con el nuevo token
        const token = tokenService.getAccessToken();
        const authReq = req.clone({
          setHeaders: {
            Authorization: `Bearer ${token}`
          }
        });
        
        return next(authReq);
      }),
      catchError(error => {
        isRefreshing = false;
        return throwError(() => error);
      })
    );
  }
  
  return next(req);
};

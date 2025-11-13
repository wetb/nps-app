import { Injectable } from '@angular/core';
import { CanActivate, Router, UrlTree } from '@angular/router';
import { Observable } from 'rxjs';
import { AuthService } from '../services/auth.service';

@Injectable({
  providedIn: 'root'
})
export class NoAuthGuard implements CanActivate {
  
  constructor(
    private authService: AuthService,
    private router: Router
  ) {}

  canActivate(): Observable<boolean | UrlTree> | Promise<boolean | UrlTree> | boolean | UrlTree {
    if (this.authService.isLoggedIn()) {
      // Si ya está autenticado, redirigir según el rol
      const role = this.authService.getUserRole();
      
      if (role === 'Admin') {
        return this.router.createUrlTree(['/admin/dashboard']);
      } else {
        return this.router.createUrlTree(['/voter/survey']);
      }
    }
    
    // Si no está autenticado, permitir acceso
    return true;
  }
}

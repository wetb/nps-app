import { Injectable } from '@angular/core';
import { ActivatedRouteSnapshot, CanActivate, Router, UrlTree } from '@angular/router';
import { Observable } from 'rxjs';
import { AuthService } from '../services/auth.service';

@Injectable({
  providedIn: 'root'
})
export class RoleGuard implements CanActivate {
  
  constructor(
    private authService: AuthService,
    private router: Router
  ) {}

  canActivate(route: ActivatedRouteSnapshot): Observable<boolean | UrlTree> | Promise<boolean | UrlTree> | boolean | UrlTree {
    // Obtener roles permitidos de la ruta
    const allowedRoles = route.data['roles'] as Array<string>;
    
    if (!allowedRoles || allowedRoles.length === 0) {
      return true; // Si no hay roles especificados, permitir acceso
    }

    const userRole = this.authService.getUserRole();
    
    if (!userRole || !allowedRoles.includes(userRole)) {
      // Redirigir según el rol
      if (userRole === 'Admin') {
        return this.router.createUrlTree(['/admin/dashboard']);
      } else if (userRole === 'Voter') {
        return this.router.createUrlTree(['/voter/survey']);
      } else {
        return this.router.createUrlTree(['/auth/login']);
      }
    }

    return true;
  }
}

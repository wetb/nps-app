import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable, tap, throwError } from 'rxjs';
import { environment } from '../../../environments/environment';
import { AuthResponse, LoginRequest, RefreshTokenRequest, TokenResponse } from '../models/auth-response.model';
import { TokenService } from './token.service';

@Injectable({
  providedIn: 'root'
})
export class AuthService {
  private apiUrl = `${environment.apiUrl}/Authentication`;
  private loginAttempts = 0;
  private readonly MAX_LOGIN_ATTEMPTS = 3;

  constructor(
    private http: HttpClient,
    private tokenService: TokenService
  ) {}

  login(username: string, password: string): Observable<AuthResponse> {
    if (this.loginAttempts >= this.MAX_LOGIN_ATTEMPTS) {
      return throwError(() => new Error('Cuenta bloqueada. Demasiados intentos fallidos.'));
    }

    const request: LoginRequest = { username, password };
    
    return this.http.post<AuthResponse>(`${this.apiUrl}/login`, request)
      .pipe(
        tap({
          next: (response) => {
            this.tokenService.saveTokens(response);
            this.loginAttempts = 0; // Reiniciar contador de intentos
          },
          error: () => {
            this.loginAttempts++;
          }
        })
      );
  }

  logout(): Observable<any> {
    const refreshToken = this.tokenService.getRefreshToken();
    if (!refreshToken) {
      this.tokenService.clearTokens();
      return throwError(() => new Error('No hay sesión activa'));
    }

    const request: RefreshTokenRequest = { refreshToken };
    
    return this.http.post(`${this.apiUrl}/logout`, request)
      .pipe(
        tap(() => {
          this.tokenService.clearTokens();
        })
      );
  }

  refreshToken(): Observable<TokenResponse> {
    const refreshToken = this.tokenService.getRefreshToken();
    if (!refreshToken) {
      return throwError(() => new Error('No hay token de refresco disponible'));
    }

    const request: RefreshTokenRequest = { refreshToken };
    
    return this.http.post<TokenResponse>(`${this.apiUrl}/refresh`, request)
      .pipe(
        tap(response => {
          this.tokenService.saveTokens(response);
        })
      );
  }

  isLoggedIn(): boolean {
    return this.tokenService.isAuthenticated() && !this.tokenService.isTokenExpired();
  }

  getUserRole(): string | null {
    const user = this.tokenService.getUser();
    return user ? user.role : null;
  }
}

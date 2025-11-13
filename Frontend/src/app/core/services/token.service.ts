import { Injectable } from '@angular/core';
import { AuthResponse, TokenResponse } from '../models/auth-response.model';
import { User } from '../models/user.model';

@Injectable({
  providedIn: 'root'
})
export class TokenService {
  private readonly ACCESS_TOKEN_KEY = 'access_token';
  private readonly REFRESH_TOKEN_KEY = 'refresh_token';
  private readonly USER_KEY = 'user';
  private readonly TOKEN_EXPIRATION_KEY = 'token_expiration';
  private readonly SESSION_TIMEOUT = 5 * 60 * 1000; // 5 minutos en milisegundos
  private sessionTimeoutId: any;

  constructor() {
    this.initSessionTimeout();
  }

  saveTokens(authResponse: AuthResponse | TokenResponse): void {
    localStorage.setItem(this.ACCESS_TOKEN_KEY, authResponse.accessToken);
    localStorage.setItem(this.REFRESH_TOKEN_KEY, authResponse.refreshToken);
    
    // Calcular tiempo de expiración
    const expirationDate = new Date();
    expirationDate.setSeconds(expirationDate.getSeconds() + authResponse.expiresIn);
    localStorage.setItem(this.TOKEN_EXPIRATION_KEY, expirationDate.toISOString());
    
    if ('userId' in authResponse) {
      const user: User = {
        userId: authResponse.userId,
        username: authResponse.username,
        role: authResponse.role as 'Admin' | 'Voter'
      };
      localStorage.setItem(this.USER_KEY, JSON.stringify(user));
    }

    // Reiniciar el timeout de sesión
    this.resetSessionTimeout();
  }

  getAccessToken(): string | null {
    return localStorage.getItem(this.ACCESS_TOKEN_KEY);
  }

  getRefreshToken(): string | null {
    return localStorage.getItem(this.REFRESH_TOKEN_KEY);
  }

  getUser(): User | null {
    const userStr = localStorage.getItem(this.USER_KEY);
    return userStr ? JSON.parse(userStr) : null;
  }

  getTokenExpiration(): Date | null {
    const expiration = localStorage.getItem(this.TOKEN_EXPIRATION_KEY);
    return expiration ? new Date(expiration) : null;
  }

  clearTokens(): void {
    localStorage.removeItem(this.ACCESS_TOKEN_KEY);
    localStorage.removeItem(this.REFRESH_TOKEN_KEY);
    localStorage.removeItem(this.USER_KEY);
    localStorage.removeItem(this.TOKEN_EXPIRATION_KEY);
    this.clearSessionTimeout();
  }

  isAuthenticated(): boolean {
    return !!this.getAccessToken();
  }

  isTokenExpired(): boolean {
    const expiration = this.getTokenExpiration();
    if (!expiration) return true;
    return expiration <= new Date();
  }

  shouldRefreshToken(): boolean {
    const expiration = this.getTokenExpiration();
    if (!expiration) return false;
    
    // Refrescar si queda menos de 1 minuto para expirar
    const now = new Date();
    const refreshThreshold = new Date(now.getTime() + 60000); // 1 minuto en ms
    return expiration <= refreshThreshold;
  }

  // Manejo de timeout de sesión
  private initSessionTimeout(): void {
    if (this.isAuthenticated()) {
      this.resetSessionTimeout();
    }

    // Escuchar eventos de usuario para reiniciar el timeout
    window.addEventListener('mousemove', () => this.resetSessionTimeout());
    window.addEventListener('keypress', () => this.resetSessionTimeout());
    window.addEventListener('click', () => this.resetSessionTimeout());
  }

  private resetSessionTimeout(): void {
    this.clearSessionTimeout();
    this.sessionTimeoutId = setTimeout(() => {
      if (this.isAuthenticated()) {
        this.clearTokens();
        window.location.href = '/auth/login?timeout=true';
      }
    }, this.SESSION_TIMEOUT);
  }

  private clearSessionTimeout(): void {
    if (this.sessionTimeoutId) {
      clearTimeout(this.sessionTimeoutId);
    }
  }
}

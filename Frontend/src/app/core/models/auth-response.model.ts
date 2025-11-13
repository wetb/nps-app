export interface AuthResponse {
  userId: number;
  username: string;
  role: string;
  accessToken: string;
  refreshToken: string;
  expiresIn: number; // en segundos
}

export interface TokenResponse {
  accessToken: string;
  refreshToken: string;
  expiresIn: number;
}

export interface LoginRequest {
  username: string;
  password: string;
}

export interface RefreshTokenRequest {
  refreshToken: string;
}

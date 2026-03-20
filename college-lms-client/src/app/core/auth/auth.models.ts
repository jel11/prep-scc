export interface LoginRequest {
  email: string;
  password: string;
}

export interface LoginResponse {
  accessToken: string;
  expiresAt: string;
  user: UserDto;
}

export interface UserDto {
  id: string;
  email: string;
  fullName: string;
  role: string;
  photoUrl?: string;
}

export interface RegisterRequest {
  email: string;
  password: string;
  fullName: string;
}

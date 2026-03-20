import { Injectable, signal, computed } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Router } from '@angular/router';
import { tap } from 'rxjs/operators';
import { environment } from '@env/environment';
import { LoginRequest, LoginResponse, UserDto, RegisterRequest } from './auth.models';

@Injectable({ providedIn: 'root' })
export class AuthService {
  private currentUser = signal<UserDto | null>(null);
  private accessToken = signal<string | null>(null);

  readonly user = this.currentUser.asReadonly();
  readonly isLoggedIn = computed(() => !!this.currentUser());
  readonly isAdmin = computed(() => this.currentUser()?.role === 'Admin');
  readonly isTeacher = computed(() => this.currentUser()?.role === 'Teacher');
  readonly isStudent = computed(() => this.currentUser()?.role === 'Student');
  readonly isTeacherOrAdmin = computed(() => this.isAdmin() || this.isTeacher());

  constructor(private http: HttpClient, private router: Router) {}

  getToken(): string | null {
    return this.accessToken();
  }

  login(request: LoginRequest) {
    return this.http.post<LoginResponse>(`${environment.apiUrl}/auth/login`, request, {
      withCredentials: true
    }).pipe(
      tap(response => {
        this.accessToken.set(response.accessToken);
        this.currentUser.set(response.user);
      })
    );
  }

  logout() {
    return this.http.post(`${environment.apiUrl}/auth/logout`, {}, {
      withCredentials: true
    }).pipe(
      tap(() => {
        this.accessToken.set(null);
        this.currentUser.set(null);
        this.router.navigate(['/login']);
      })
    );
  }

  refresh() {
    return this.http.post<LoginResponse>(`${environment.apiUrl}/auth/refresh`, {}, {
      withCredentials: true
    }).pipe(
      tap(response => {
        this.accessToken.set(response.accessToken);
        this.currentUser.set(response.user);
      })
    );
  }

  fetchCurrentUser() {
    return this.http.get<UserDto>(`${environment.apiUrl}/auth/me`).pipe(
      tap(user => this.currentUser.set(user))
    );
  }

  registerStudent(request: RegisterRequest) {
    return this.http.post<UserDto>(`${environment.apiUrl}/auth/register-student`, request);
  }

  registerTeacher(request: RegisterRequest) {
    return this.http.post<UserDto>(`${environment.apiUrl}/auth/register-teacher`, request);
  }
}

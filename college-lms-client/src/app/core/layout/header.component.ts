import { Component, EventEmitter, Output } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterLink } from '@angular/router';
import { MatToolbarModule } from '@angular/material/toolbar';
import { MatIconModule } from '@angular/material/icon';
import { MatButtonModule } from '@angular/material/button';
import { MatMenuModule } from '@angular/material/menu';
import { AuthService } from '../auth/auth.service';

@Component({
  selector: 'app-header',
  standalone: true,
  imports: [CommonModule, RouterLink, MatToolbarModule, MatIconModule, MatButtonModule, MatMenuModule],
  template: `
    <mat-toolbar color="primary" class="header">
      <button mat-icon-button (click)="toggleSidebar.emit()">
        <mat-icon>menu</mat-icon>
      </button>

      <a routerLink="/" class="logo">
        <mat-icon>school</mat-icon>
        <span class="logo-text">College LMS</span>
      </a>

      <span class="spacer"></span>

      <nav class="nav-links">
        <a mat-button routerLink="/teachers">Преподаватели</a>
        <a mat-button routerLink="/disciplines">Дисциплины</a>
        <a mat-button routerLink="/schedule">Расписание</a>
      </nav>

      <span class="spacer"></span>

      @if (auth.isLoggedIn()) {
        <button mat-icon-button [matMenuTriggerFor]="userMenu">
          <mat-icon>account_circle</mat-icon>
        </button>
        <mat-menu #userMenu="matMenu">
          <div class="user-info">
            <strong>{{ auth.user()?.fullName }}</strong>
            <small>{{ auth.user()?.role }}</small>
          </div>
          <mat-divider></mat-divider>
          @if (auth.isTeacherOrAdmin()) {
            <button mat-menu-item routerLink="/admin">
              <mat-icon>dashboard</mat-icon>
              <span>Управление</span>
            </button>
          }
          <button mat-menu-item (click)="onLogout()">
            <mat-icon>logout</mat-icon>
            <span>Выйти</span>
          </button>
        </mat-menu>
      } @else {
        <a mat-stroked-button routerLink="/login">
          <mat-icon>login</mat-icon>
          Войти
        </a>
      }
    </mat-toolbar>
  `,
  styles: [`
    .header {
      position: fixed;
      top: 0;
      left: 0;
      right: 0;
      z-index: 1000;
    }
    .logo {
      display: flex;
      align-items: center;
      gap: 8px;
      color: white;
      text-decoration: none;
      margin-left: 8px;
    }
    .logo-text {
      font-size: 18px;
      font-weight: 500;
    }
    .spacer {
      flex: 1;
    }
    .nav-links a {
      color: white;
    }
    .user-info {
      padding: 12px 16px;
      display: flex;
      flex-direction: column;
      gap: 4px;
    }
    .user-info small {
      color: #757575;
    }
    @media (max-width: 768px) {
      .nav-links {
        display: none;
      }
      .logo-text {
        display: none;
      }
    }
  `]
})
export class HeaderComponent {
  @Output() toggleSidebar = new EventEmitter<void>();

  constructor(public auth: AuthService) {}

  onLogout() {
    this.auth.logout().subscribe();
  }
}

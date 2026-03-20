import { Component, Input, Output, EventEmitter } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterLink, RouterLinkActive } from '@angular/router';
import { MatListModule } from '@angular/material/list';
import { MatIconModule } from '@angular/material/icon';
import { AuthService } from '../auth/auth.service';

@Component({
  selector: 'app-sidebar',
  standalone: true,
  imports: [CommonModule, RouterLink, RouterLinkActive, MatListModule, MatIconModule],
  template: `
    <aside class="sidebar" [class.open]="isOpen">
      <mat-nav-list>
        <a mat-list-item routerLink="/" routerLinkActive="active" [routerLinkActiveOptions]="{exact: true}" (click)="closeSidebar.emit()">
          <mat-icon matListItemIcon>home</mat-icon>
          <span matListItemTitle>Главная</span>
        </a>
        <a mat-list-item routerLink="/teachers" routerLinkActive="active" (click)="closeSidebar.emit()">
          <mat-icon matListItemIcon>people</mat-icon>
          <span matListItemTitle>Преподаватели</span>
        </a>
        <a mat-list-item routerLink="/disciplines" routerLinkActive="active" (click)="closeSidebar.emit()">
          <mat-icon matListItemIcon>menu_book</mat-icon>
          <span matListItemTitle>Дисциплины</span>
        </a>
        <a mat-list-item routerLink="/schedule" routerLinkActive="active" (click)="closeSidebar.emit()">
          <mat-icon matListItemIcon>calendar_today</mat-icon>
          <span matListItemTitle>Расписание</span>
        </a>

        @if (auth.isLoggedIn()) {
          <mat-divider></mat-divider>
          <a mat-list-item routerLink="/journal" routerLinkActive="active" (click)="closeSidebar.emit()">
            <mat-icon matListItemIcon>assignment</mat-icon>
            <span matListItemTitle>Журнал</span>
          </a>
          <a mat-list-item routerLink="/rating" routerLinkActive="active" (click)="closeSidebar.emit()">
            <mat-icon matListItemIcon>star</mat-icon>
            <span matListItemTitle>Рейтинг</span>
          </a>
        }

        @if (auth.isTeacherOrAdmin()) {
          <mat-divider></mat-divider>
          <a mat-list-item routerLink="/admin" routerLinkActive="active" (click)="closeSidebar.emit()">
            <mat-icon matListItemIcon>admin_panel_settings</mat-icon>
            <span matListItemTitle>Управление</span>
          </a>
        }
      </mat-nav-list>
    </aside>
  `,
  styles: [`
    .sidebar {
      width: 260px;
      background: white;
      border-right: 1px solid #e0e0e0;
      position: fixed;
      top: 64px;
      bottom: 0;
      left: -260px;
      transition: left 0.3s ease;
      z-index: 999;
      overflow-y: auto;
    }
    .sidebar.open {
      left: 0;
    }
    .active {
      background-color: rgba(21, 101, 192, 0.08) !important;
    }
  `]
})
export class SidebarComponent {
  @Input() isOpen = false;
  @Output() closeSidebar = new EventEmitter<void>();

  constructor(public auth: AuthService) {}
}

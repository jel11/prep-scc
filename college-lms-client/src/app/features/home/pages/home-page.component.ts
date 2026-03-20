import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterLink } from '@angular/router';
import { MatCardModule } from '@angular/material/card';
import { MatIconModule } from '@angular/material/icon';
import { MatButtonModule } from '@angular/material/button';
import { MatChipsModule } from '@angular/material/chips';
import { ApiService } from '@core/api/api.service';

@Component({
  selector: 'app-home-page',
  standalone: true,
  imports: [CommonModule, RouterLink, MatCardModule, MatIconModule, MatButtonModule, MatChipsModule],
  template: `
    <div class="page-container">
      <section class="hero">
        <h1>College LMS</h1>
        <p>Система управления обучением</p>
      </section>

      <div class="grid">
        <mat-card class="info-card">
          <mat-card-header>
            <mat-icon mat-card-avatar>people</mat-icon>
            <mat-card-title>Преподаватели</mat-card-title>
          </mat-card-header>
          <mat-card-content>
            <p>{{ teachers.length }} преподавателей</p>
          </mat-card-content>
          <mat-card-actions>
            <a mat-button routerLink="/teachers" color="primary">Смотреть всех</a>
          </mat-card-actions>
        </mat-card>

        <mat-card class="info-card">
          <mat-card-header>
            <mat-icon mat-card-avatar>menu_book</mat-icon>
            <mat-card-title>Дисциплины</mat-card-title>
          </mat-card-header>
          <mat-card-content>
            <p>{{ disciplines.length }} дисциплин</p>
          </mat-card-content>
          <mat-card-actions>
            <a mat-button routerLink="/disciplines" color="primary">Все дисциплины</a>
          </mat-card-actions>
        </mat-card>

        <mat-card class="info-card">
          <mat-card-header>
            <mat-icon mat-card-avatar>calendar_today</mat-icon>
            <mat-card-title>Расписание</mat-card-title>
          </mat-card-header>
          <mat-card-content>
            <p>Расписание занятий по группам</p>
          </mat-card-content>
          <mat-card-actions>
            <a mat-button routerLink="/schedule" color="primary">Открыть</a>
          </mat-card-actions>
        </mat-card>
      </div>

      @if (announcements.length > 0) {
        <section class="announcements">
          <h2>Объявления</h2>
          @for (announcement of announcements; track announcement.id) {
            <mat-card class="announcement-card" [class.pinned]="announcement.isPinned">
              @if (announcement.isPinned) {
                <mat-chip color="accent" selected>Закреплено</mat-chip>
              }
              <mat-card-header>
                <mat-card-title>{{ announcement.title }}</mat-card-title>
                <mat-card-subtitle>{{ announcement.author }} &middot; {{ announcement.createdAt | date:'dd.MM.yyyy HH:mm' }}</mat-card-subtitle>
              </mat-card-header>
              <mat-card-content>
                <p>{{ announcement.content }}</p>
              </mat-card-content>
            </mat-card>
          }
        </section>
      }
    </div>
  `,
  styles: [`
    .hero {
      text-align: center;
      padding: 48px 0 32px;
    }
    .hero h1 {
      font-size: 36px;
      font-weight: 300;
      color: #1565c0;
    }
    .hero p {
      font-size: 18px;
      color: #757575;
      margin-top: 8px;
    }
    .grid {
      display: grid;
      grid-template-columns: repeat(auto-fit, minmax(280px, 1fr));
      gap: 16px;
      margin-bottom: 32px;
    }
    .info-card mat-icon[mat-card-avatar] {
      font-size: 40px;
      width: 40px;
      height: 40px;
      color: #1565c0;
    }
    .announcements h2 {
      margin-bottom: 16px;
      font-weight: 400;
    }
    .announcement-card {
      margin-bottom: 12px;
    }
    .announcement-card.pinned {
      border-left: 4px solid #ff6f00;
    }
  `]
})
export class HomePageComponent implements OnInit {
  teachers: any[] = [];
  disciplines: any[] = [];
  announcements: any[] = [];

  constructor(private api: ApiService) {}

  ngOnInit() {
    this.api.get<any[]>('/teachers').subscribe(data => this.teachers = data);
    this.api.get<any[]>('/disciplines').subscribe(data => this.disciplines = data);
    this.api.get<any>('/announcements').subscribe(data => this.announcements = data.items || []);
  }
}

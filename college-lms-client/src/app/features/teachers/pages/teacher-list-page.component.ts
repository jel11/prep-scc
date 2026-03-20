import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterLink } from '@angular/router';
import { MatCardModule } from '@angular/material/card';
import { MatIconModule } from '@angular/material/icon';
import { MatChipsModule } from '@angular/material/chips';
import { ApiService } from '@core/api/api.service';

@Component({
  selector: 'app-teacher-list-page',
  standalone: true,
  imports: [CommonModule, RouterLink, MatCardModule, MatIconModule, MatChipsModule],
  template: `
    <div class="page-container">
      <h1>Преподаватели</h1>
      <div class="teachers-grid">
        @for (teacher of teachers; track teacher.id) {
          <mat-card class="teacher-card">
            <div class="teacher-photo">
              @if (teacher.photoPath) {
                <img [src]="teacher.photoPath" [alt]="teacher.fullName">
              } @else {
                <mat-icon class="avatar-placeholder">person</mat-icon>
              }
            </div>
            <mat-card-header>
              <mat-card-title>
                <a [routerLink]="['/teachers', teacher.id]">{{ teacher.fullName }}</a>
              </mat-card-title>
            </mat-card-header>
            <mat-card-content>
              <div class="disciplines">
                @for (d of teacher.disciplines; track d.id) {
                  <mat-chip>{{ d.name }}</mat-chip>
                }
              </div>
            </mat-card-content>
          </mat-card>
        }
      </div>
    </div>
  `,
  styles: [`
    h1 { font-weight: 400; margin-bottom: 24px; }
    .teachers-grid {
      display: grid;
      grid-template-columns: repeat(auto-fill, minmax(300px, 1fr));
      gap: 16px;
    }
    .teacher-card { padding: 16px; }
    .teacher-photo {
      text-align: center;
      margin-bottom: 16px;
    }
    .teacher-photo img {
      width: 120px;
      height: 120px;
      border-radius: 50%;
      object-fit: cover;
    }
    .avatar-placeholder {
      font-size: 80px;
      width: 80px;
      height: 80px;
      color: #bdbdbd;
    }
    .disciplines {
      display: flex;
      flex-wrap: wrap;
      gap: 4px;
      margin-top: 8px;
    }
  `]
})
export class TeacherListPageComponent implements OnInit {
  teachers: any[] = [];

  constructor(private api: ApiService) {}

  ngOnInit() {
    this.api.get<any[]>('/teachers').subscribe(data => this.teachers = data);
  }
}

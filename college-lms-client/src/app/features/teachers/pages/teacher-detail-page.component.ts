import { Component, Input, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterLink } from '@angular/router';
import { MatCardModule } from '@angular/material/card';
import { MatIconModule } from '@angular/material/icon';
import { MatListModule } from '@angular/material/list';
import { ApiService } from '@core/api/api.service';

@Component({
  selector: 'app-teacher-detail-page',
  standalone: true,
  imports: [CommonModule, RouterLink, MatCardModule, MatIconModule, MatListModule],
  template: `
    <div class="page-container">
      @if (teacher) {
        <mat-card class="teacher-profile">
          <div class="profile-header">
            <div class="photo">
              @if (teacher.photoPath) {
                <img [src]="teacher.photoPath" [alt]="teacher.fullName">
              } @else {
                <mat-icon class="avatar-placeholder">person</mat-icon>
              }
            </div>
            <div class="info">
              <h1>{{ teacher.fullName }}</h1>
              <p class="email">{{ teacher.email }}</p>
              @if (teacher.bio) {
                <p class="bio">{{ teacher.bio }}</p>
              }
            </div>
          </div>
        </mat-card>

        <h2>Дисциплины</h2>
        <mat-card>
          <mat-nav-list>
            @for (d of teacher.disciplines; track d.id) {
              <a mat-list-item [routerLink]="['/disciplines', d.id]">
                <mat-icon matListItemIcon>menu_book</mat-icon>
                <span matListItemTitle>{{ d.name }}</span>
                @if (d.code) {
                  <span matListItemLine>{{ d.code }}</span>
                }
              </a>
            }
          </mat-nav-list>
        </mat-card>
      }
    </div>
  `,
  styles: [`
    .profile-header {
      display: flex;
      gap: 24px;
      padding: 24px;
      align-items: center;
    }
    .photo img {
      width: 150px;
      height: 150px;
      border-radius: 50%;
      object-fit: cover;
    }
    .avatar-placeholder {
      font-size: 100px;
      width: 100px;
      height: 100px;
      color: #bdbdbd;
    }
    h1 { font-weight: 400; }
    h2 { font-weight: 400; margin: 24px 0 12px; }
    .email { color: #757575; }
    .bio { margin-top: 8px; }
    @media (max-width: 576px) {
      .profile-header { flex-direction: column; text-align: center; }
    }
  `]
})
export class TeacherDetailPageComponent implements OnInit {
  @Input() id!: string;
  teacher: any;

  constructor(private api: ApiService) {}

  ngOnInit() {
    this.api.get<any>(`/teachers/${this.id}`).subscribe(data => this.teacher = data);
  }
}

import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterLink } from '@angular/router';
import { MatCardModule } from '@angular/material/card';
import { MatIconModule } from '@angular/material/icon';
import { MatChipsModule } from '@angular/material/chips';
import { ApiService } from '@core/api/api.service';

@Component({
  selector: 'app-discipline-list-page',
  standalone: true,
  imports: [CommonModule, RouterLink, MatCardModule, MatIconModule, MatChipsModule],
  template: `
    <div class="page-container">
      <h1>Дисциплины</h1>
      <div class="disciplines-grid">
        @for (d of disciplines; track d.id) {
          <mat-card class="discipline-card">
            <mat-card-header>
              <mat-icon mat-card-avatar>menu_book</mat-icon>
              <mat-card-title>
                <a [routerLink]="['/disciplines', d.id]">{{ d.name }}</a>
              </mat-card-title>
              @if (d.code) {
                <mat-card-subtitle>{{ d.code }}</mat-card-subtitle>
              }
            </mat-card-header>
            <mat-card-content>
              @if (d.description) {
                <p>{{ d.description }}</p>
              }
              <div class="meta">
                @for (t of d.teachers; track t.id) {
                  <mat-chip>
                    <mat-icon>person</mat-icon>
                    {{ t.fullName }}
                  </mat-chip>
                }
                @for (g of d.groups; track g.id) {
                  <mat-chip>
                    <mat-icon>group</mat-icon>
                    {{ g.name }}
                  </mat-chip>
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
    .disciplines-grid {
      display: grid;
      grid-template-columns: repeat(auto-fill, minmax(350px, 1fr));
      gap: 16px;
    }
    .meta {
      display: flex;
      flex-wrap: wrap;
      gap: 4px;
      margin-top: 12px;
    }
    mat-icon[mat-card-avatar] { color: #1565c0; }
  `]
})
export class DisciplineListPageComponent implements OnInit {
  disciplines: any[] = [];

  constructor(private api: ApiService) {}

  ngOnInit() {
    this.api.get<any[]>('/disciplines').subscribe(data => this.disciplines = data);
  }
}

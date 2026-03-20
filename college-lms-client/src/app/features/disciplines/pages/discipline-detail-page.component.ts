import { Component, Input, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterLink } from '@angular/router';
import { MatCardModule } from '@angular/material/card';
import { MatIconModule } from '@angular/material/icon';
import { MatTableModule } from '@angular/material/table';
import { MatButtonModule } from '@angular/material/button';
import { MatChipsModule } from '@angular/material/chips';
import { MatExpansionModule } from '@angular/material/expansion';
import { ApiService } from '@core/api/api.service';
import { AuthService } from '@core/auth/auth.service';

@Component({
  selector: 'app-discipline-detail-page',
  standalone: true,
  imports: [
    CommonModule, RouterLink,
    MatCardModule, MatIconModule, MatTableModule, MatButtonModule, MatChipsModule, MatExpansionModule
  ],
  template: `
    <div class="page-container">
      @if (discipline) {
        <div class="header">
          <h1>{{ discipline.name }}</h1>
          @if (discipline.code) {
            <span class="code">{{ discipline.code }}</span>
          }
        </div>

        @if (discipline.description) {
          <p class="description">{{ discipline.description }}</p>
        }

        <div class="meta-chips">
          @for (t of discipline.teachers; track t.id) {
            <mat-chip>
              <mat-icon>person</mat-icon>
              <a [routerLink]="['/teachers', t.id]">{{ t.fullName }}</a>
            </mat-chip>
          }
          @for (g of discipline.groups; track g.id) {
            <mat-chip>
              <mat-icon>group</mat-icon>
              {{ g.name }}
            </mat-chip>
          }
        </div>

        <h2>Календарный план</h2>

        <mat-accordion>
          @for (topic of discipline.topics; track topic.id) {
            <mat-expansion-panel>
              <mat-expansion-panel-header>
                <mat-panel-title>
                  {{ topic.orderIndex }}. {{ topic.title }}
                </mat-panel-title>
                <mat-panel-description>
                  @if (topic.plannedDate) {
                    {{ topic.plannedDate }}
                  }
                  @if (topic.hours) {
                    &middot; {{ topic.hours }} ч.
                  }
                </mat-panel-description>
              </mat-expansion-panel-header>

              @if (topic.description) {
                <p>{{ topic.description }}</p>
              }

              @if (topic.materials.length > 0) {
                <h4>Материалы</h4>
                @for (m of topic.materials; track m.id) {
                  <div class="material-item">
                    <mat-icon>{{ getMaterialIcon(m.type) }}</mat-icon>
                    @if (m.filePath) {
                      <a [href]="'/uploads/' + m.filePath" target="_blank">{{ m.title }}</a>
                    } @else if (m.externalUrl) {
                      <a [href]="m.externalUrl" target="_blank">{{ m.title }}</a>
                    } @else {
                      <span>{{ m.title }}</span>
                    }
                    <mat-chip class="type-chip">{{ m.type }}</mat-chip>
                  </div>
                }
              }

              @if (topic.tests.length > 0) {
                <h4>Тесты</h4>
                @for (test of topic.tests; track test.id) {
                  <a mat-stroked-button [routerLink]="['/tests', test.id]" color="primary">
                    <mat-icon>quiz</mat-icon>
                    {{ test.title }} ({{ test.code }})
                  </a>
                }
              }
            </mat-expansion-panel>
          }
        </mat-accordion>
      }
    </div>
  `,
  styles: [`
    .header { display: flex; align-items: baseline; gap: 12px; margin-bottom: 8px; }
    h1 { font-weight: 400; }
    h2 { font-weight: 400; margin: 24px 0 16px; }
    h4 { margin: 12px 0 8px; color: #757575; }
    .code { color: #757575; font-size: 18px; }
    .description { color: #616161; margin-bottom: 16px; }
    .meta-chips { display: flex; flex-wrap: wrap; gap: 8px; margin-bottom: 16px; }
    .material-item {
      display: flex;
      align-items: center;
      gap: 8px;
      padding: 4px 0;
    }
    .type-chip { font-size: 11px; }
  `]
})
export class DisciplineDetailPageComponent implements OnInit {
  @Input() id!: string;
  discipline: any;

  constructor(private api: ApiService, public auth: AuthService) {}

  ngOnInit() {
    this.api.get<any>(`/disciplines/${this.id}`).subscribe(data => this.discipline = data);
  }

  getMaterialIcon(type: string): string {
    const icons: Record<string, string> = {
      'Lecture': 'description',
      'Practical': 'build',
      'Presentation': 'slideshow',
      'ExamQuestion': 'help_outline',
      'Other': 'attach_file'
    };
    return icons[type] || 'attach_file';
  }
}

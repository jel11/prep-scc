import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { MatCardModule } from '@angular/material/card';
import { MatSelectModule } from '@angular/material/select';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatIconModule } from '@angular/material/icon';
import { ApiService } from '@core/api/api.service';

@Component({
  selector: 'app-rating-page',
  standalone: true,
  imports: [CommonModule, FormsModule, MatCardModule, MatSelectModule, MatFormFieldModule, MatIconModule],
  template: `
    <div class="page-container">
      <h1>Рейтинг</h1>

      <div class="filters">
        <mat-form-field appearance="outline">
          <mat-label>Группа</mat-label>
          <mat-select [(ngModel)]="selectedGroupId" (selectionChange)="loadRating()">
            @for (g of groups; track g.id) {
              <mat-option [value]="g.id">{{ g.name }}</mat-option>
            }
          </mat-select>
        </mat-form-field>

        <mat-form-field appearance="outline">
          <mat-label>Дисциплина</mat-label>
          <mat-select [(ngModel)]="selectedDisciplineId" (selectionChange)="loadRating()">
            @for (d of disciplines; track d.id) {
              <mat-option [value]="d.id">{{ d.name }}</mat-option>
            }
          </mat-select>
        </mat-form-field>
      </div>

      @if (ratingData) {
        <mat-card>
          <div class="table-container">
            <table class="rating-table">
              <thead>
                <tr>
                  <th class="name-col">Студент</th>
                  @for (date of ratingData.dates; track date) {
                    <th>{{ date }}</th>
                  }
                  <th>Средняя</th>
                  <th>Посещаемость</th>
                </tr>
              </thead>
              <tbody>
                @for (student of ratingData.students; track student.studentId) {
                  <tr>
                    <td class="name-col">{{ student.fullName }}</td>
                    @for (grade of student.grades; track grade.date) {
                      <td [class]="getGradeClass(grade)">
                        @if (!grade.isPresent) {
                          <span class="absent">Н</span>
                        } @else {
                          {{ grade.grade || '' }}
                        }
                      </td>
                    }
                    <td class="average">{{ student.averageGrade | number:'1.2-2' }}</td>
                    <td class="attendance" [class.low]="student.attendancePercent < 75">
                      {{ student.attendancePercent | number:'1.0-0' }}%
                    </td>
                  </tr>
                }
              </tbody>
            </table>
          </div>
        </mat-card>
      }
    </div>
  `,
  styles: [`
    h1 { font-weight: 400; margin-bottom: 24px; }
    .filters { display: flex; gap: 16px; flex-wrap: wrap; margin-bottom: 16px; }
    .table-container { overflow-x: auto; }
    .rating-table { width: 100%; border-collapse: collapse; }
    .rating-table th, .rating-table td {
      padding: 8px 12px; border: 1px solid #e0e0e0;
      text-align: center; font-size: 14px;
    }
    .rating-table th { background: #f5f5f5; font-weight: 500; white-space: nowrap; }
    .name-col { text-align: left; min-width: 200px; }
    .average, .attendance { font-weight: 500; background: #f5f5f5; }
    .absent { color: #d32f2f; font-weight: 500; }
    .low { color: #d32f2f; }
  `]
})
export class RatingPageComponent implements OnInit {
  groups: any[] = [];
  disciplines: any[] = [];
  selectedGroupId = '';
  selectedDisciplineId = '';
  ratingData: any;

  constructor(private api: ApiService) {}

  ngOnInit() {
    this.api.get<any[]>('/groups').subscribe(data => this.groups = data);
    this.api.get<any[]>('/disciplines').subscribe(data => this.disciplines = data);
  }

  loadRating() {
    if (!this.selectedGroupId || !this.selectedDisciplineId) return;
    this.api.get<any>('/rating', {
      groupId: this.selectedGroupId,
      disciplineId: this.selectedDisciplineId
    }).subscribe(data => this.ratingData = data);
  }

  getGradeClass(grade: any): string {
    if (!grade.isPresent) return '';
    return `grade-${grade.grade}`;
  }
}

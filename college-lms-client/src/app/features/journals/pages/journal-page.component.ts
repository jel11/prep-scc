import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { MatCardModule } from '@angular/material/card';
import { MatSelectModule } from '@angular/material/select';
import { MatTableModule } from '@angular/material/table';
import { MatFormFieldModule } from '@angular/material/form-field';
import { ApiService } from '@core/api/api.service';

@Component({
  selector: 'app-journal-page',
  standalone: true,
  imports: [CommonModule, FormsModule, MatCardModule, MatSelectModule, MatTableModule, MatFormFieldModule],
  template: `
    <div class="page-container">
      <h1>Журнал</h1>

      <div class="filters">
        <mat-form-field appearance="outline">
          <mat-label>Группа</mat-label>
          <mat-select [(ngModel)]="selectedGroupId" (selectionChange)="loadJournal()">
            @for (g of groups; track g.id) {
              <mat-option [value]="g.id">{{ g.name }}</mat-option>
            }
          </mat-select>
        </mat-form-field>

        <mat-form-field appearance="outline">
          <mat-label>Дисциплина</mat-label>
          <mat-select [(ngModel)]="selectedDisciplineId" (selectionChange)="loadJournal()">
            @for (d of disciplines; track d.id) {
              <mat-option [value]="d.id">{{ d.name }}</mat-option>
            }
          </mat-select>
        </mat-form-field>
      </div>

      @if (journalData) {
        <mat-card>
          <div class="table-container">
            <table class="journal-table">
              <thead>
                <tr>
                  <th class="name-col">Студент</th>
                  @for (date of journalData.dates; track date) {
                    <th>{{ date }}</th>
                  }
                  <th>Средняя</th>
                </tr>
              </thead>
              <tbody>
                @for (student of journalData.students; track student.studentId) {
                  <tr>
                    <td class="name-col">{{ student.fullName }}</td>
                    @for (grade of student.grades; track grade.date) {
                      <td [class]="'grade-' + grade.grade">
                        {{ grade.grade || '' }}
                      </td>
                    }
                    <td class="average">{{ student.average | number:'1.1-1' }}</td>
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
    .journal-table {
      width: 100%;
      border-collapse: collapse;
    }
    .journal-table th, .journal-table td {
      padding: 8px 12px;
      border: 1px solid #e0e0e0;
      text-align: center;
      font-size: 14px;
    }
    .journal-table th {
      background: #f5f5f5;
      font-weight: 500;
      white-space: nowrap;
    }
    .name-col { text-align: left; min-width: 200px; }
    .average { font-weight: 500; background: #f5f5f5; }
  `]
})
export class JournalPageComponent implements OnInit {
  groups: any[] = [];
  disciplines: any[] = [];
  selectedGroupId = '';
  selectedDisciplineId = '';
  journalData: any;

  constructor(private api: ApiService) {}

  ngOnInit() {
    this.api.get<any[]>('/groups').subscribe(data => this.groups = data);
    this.api.get<any[]>('/disciplines').subscribe(data => this.disciplines = data);
  }

  loadJournal() {
    if (!this.selectedGroupId || !this.selectedDisciplineId) return;
    this.api.get<any>('/journal', {
      groupId: this.selectedGroupId,
      disciplineId: this.selectedDisciplineId
    }).subscribe(data => this.journalData = data);
  }
}

import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { MatCardModule } from '@angular/material/card';
import { MatSelectModule } from '@angular/material/select';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatIconModule } from '@angular/material/icon';
import { ApiService } from '@core/api/api.service';

@Component({
  selector: 'app-schedule-page',
  standalone: true,
  imports: [CommonModule, FormsModule, MatCardModule, MatSelectModule, MatFormFieldModule, MatIconModule],
  template: `
    <div class="page-container">
      <h1>Расписание</h1>

      <mat-form-field appearance="outline">
        <mat-label>Группа</mat-label>
        <mat-select [(ngModel)]="selectedGroupId" (selectionChange)="loadSchedule()">
          @for (g of groups; track g.id) {
            <mat-option [value]="g.id">{{ g.name }}</mat-option>
          }
        </mat-select>
      </mat-form-field>

      @if (schedule.length > 0) {
        <mat-card>
          <div class="table-container">
            <table class="schedule-table">
              <thead>
                <tr>
                  <th>Пара</th>
                  @for (day of dayNames; track day; let i = $index) {
                    <th>{{ day }}</th>
                  }
                </tr>
              </thead>
              <tbody>
                @for (slot of timeSlots; track slot) {
                  <tr>
                    <td class="slot-num">{{ slot }}</td>
                    @for (day of [1,2,3,4,5,6]; track day) {
                      <td>
                        @if (getEntry(day, slot); as entry) {
                          <div class="entry">
                            <strong>{{ entry.discipline }}</strong>
                            <small>{{ entry.teacher }}</small>
                            @if (entry.room) {
                              <small class="room">{{ entry.room }}</small>
                            }
                            @if (entry.weekType !== 'Both') {
                              <span class="week-badge">{{ entry.weekType === 'Odd' ? 'нечёт.' : 'чёт.' }}</span>
                            }
                          </div>
                        }
                      </td>
                    }
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
    .table-container { overflow-x: auto; }
    .schedule-table { width: 100%; border-collapse: collapse; }
    .schedule-table th, .schedule-table td {
      padding: 8px; border: 1px solid #e0e0e0; vertical-align: top; min-width: 130px;
    }
    .schedule-table th { background: #1565c0; color: white; text-align: center; }
    .slot-num { text-align: center; font-weight: 500; background: #f5f5f5; width: 50px; }
    .entry { display: flex; flex-direction: column; gap: 2px; }
    .entry strong { font-size: 13px; }
    .entry small { color: #757575; font-size: 12px; }
    .room { color: #1565c0; }
    .week-badge {
      display: inline-block; font-size: 10px; padding: 1px 4px;
      background: #fff3e0; color: #e65100; border-radius: 4px;
    }
  `]
})
export class SchedulePageComponent implements OnInit {
  groups: any[] = [];
  schedule: any[] = [];
  selectedGroupId = '';
  dayNames = ['Пн', 'Вт', 'Ср', 'Чт', 'Пт', 'Сб'];
  timeSlots = [1, 2, 3, 4, 5, 6, 7];

  constructor(private api: ApiService) {}

  ngOnInit() {
    this.api.get<any[]>('/groups').subscribe(data => this.groups = data);
  }

  loadSchedule() {
    if (!this.selectedGroupId) return;
    this.api.get<any[]>('/schedule', { groupId: this.selectedGroupId })
      .subscribe(data => this.schedule = data);
  }

  getEntry(day: number, slot: number): any {
    return this.schedule.find(e => e.dayOfWeek === day && e.timeSlot === slot);
  }
}

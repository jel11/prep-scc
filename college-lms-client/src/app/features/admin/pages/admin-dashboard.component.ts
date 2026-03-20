import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { MatCardModule } from '@angular/material/card';
import { MatTabsModule } from '@angular/material/tabs';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatSelectModule } from '@angular/material/select';
import { MatSnackBar, MatSnackBarModule } from '@angular/material/snack-bar';
import { ApiService } from '@core/api/api.service';
import { AuthService } from '@core/auth/auth.service';

@Component({
  selector: 'app-admin-dashboard',
  standalone: true,
  imports: [
    CommonModule, FormsModule,
    MatCardModule, MatTabsModule, MatFormFieldModule, MatInputModule,
    MatButtonModule, MatIconModule, MatSelectModule, MatSnackBarModule
  ],
  template: `
    <div class="page-container">
      <h1>Панель управления</h1>

      <mat-tab-group>
        @if (auth.isAdmin()) {
          <mat-tab label="Пользователи">
            <div class="tab-content">
              <mat-card>
                <mat-card-header>
                  <mat-card-title>Регистрация нового пользователя</mat-card-title>
                </mat-card-header>
                <mat-card-content>
                  <form (ngSubmit)="registerUser()" class="form-grid">
                    <mat-form-field appearance="outline">
                      <mat-label>ФИО</mat-label>
                      <input matInput [(ngModel)]="newUser.fullName" name="fullName" required>
                    </mat-form-field>
                    <mat-form-field appearance="outline">
                      <mat-label>Email</mat-label>
                      <input matInput type="email" [(ngModel)]="newUser.email" name="email" required>
                    </mat-form-field>
                    <mat-form-field appearance="outline">
                      <mat-label>Пароль</mat-label>
                      <input matInput [(ngModel)]="newUser.password" name="password" required>
                    </mat-form-field>
                    <mat-form-field appearance="outline">
                      <mat-label>Роль</mat-label>
                      <mat-select [(ngModel)]="newUser.role" name="role">
                        <mat-option value="student">Студент</mat-option>
                        <mat-option value="teacher">Преподаватель</mat-option>
                      </mat-select>
                    </mat-form-field>
                    <button mat-raised-button color="primary" type="submit">
                      <mat-icon>person_add</mat-icon>
                      Зарегистрировать
                    </button>
                  </form>
                </mat-card-content>
              </mat-card>
            </div>
          </mat-tab>

          <mat-tab label="Группы">
            <div class="tab-content">
              <mat-card>
                <mat-card-header>
                  <mat-card-title>Создать группу</mat-card-title>
                </mat-card-header>
                <mat-card-content>
                  <form (ngSubmit)="createGroup()" class="form-row">
                    <mat-form-field appearance="outline">
                      <mat-label>Название группы</mat-label>
                      <input matInput [(ngModel)]="newGroup.name" name="groupName" required placeholder="ИП-245">
                    </mat-form-field>
                    <mat-form-field appearance="outline">
                      <mat-label>Год поступления</mat-label>
                      <input matInput type="number" [(ngModel)]="newGroup.enrollmentYear" name="year" required>
                    </mat-form-field>
                    <button mat-raised-button color="primary" type="submit">
                      <mat-icon>group_add</mat-icon>
                      Создать
                    </button>
                  </form>
                </mat-card-content>
              </mat-card>
            </div>
          </mat-tab>

          <mat-tab label="Дисциплины">
            <div class="tab-content">
              <mat-card>
                <mat-card-header>
                  <mat-card-title>Создать дисциплину</mat-card-title>
                </mat-card-header>
                <mat-card-content>
                  <form (ngSubmit)="createDiscipline()" class="form-grid">
                    <mat-form-field appearance="outline">
                      <mat-label>Название</mat-label>
                      <input matInput [(ngModel)]="newDiscipline.name" name="discName" required>
                    </mat-form-field>
                    <mat-form-field appearance="outline">
                      <mat-label>Код</mat-label>
                      <input matInput [(ngModel)]="newDiscipline.code" name="discCode" placeholder="ОП.04">
                    </mat-form-field>
                    <mat-form-field appearance="outline">
                      <mat-label>Описание</mat-label>
                      <textarea matInput [(ngModel)]="newDiscipline.description" name="discDesc"></textarea>
                    </mat-form-field>
                    <button mat-raised-button color="primary" type="submit">
                      <mat-icon>add</mat-icon>
                      Создать
                    </button>
                  </form>
                </mat-card-content>
              </mat-card>
            </div>
          </mat-tab>
        }

        <mat-tab label="Объявления">
          <div class="tab-content">
            <mat-card>
              <mat-card-header>
                <mat-card-title>Новое объявление</mat-card-title>
              </mat-card-header>
              <mat-card-content>
                <form (ngSubmit)="createAnnouncement()" class="form-grid">
                  <mat-form-field appearance="outline">
                    <mat-label>Заголовок</mat-label>
                    <input matInput [(ngModel)]="newAnnouncement.title" name="annTitle" required>
                  </mat-form-field>
                  <mat-form-field appearance="outline">
                    <mat-label>Содержание</mat-label>
                    <textarea matInput [(ngModel)]="newAnnouncement.content" name="annContent" rows="4" required></textarea>
                  </mat-form-field>
                  <button mat-raised-button color="primary" type="submit">
                    <mat-icon>campaign</mat-icon>
                    Опубликовать
                  </button>
                </form>
              </mat-card-content>
            </mat-card>
          </div>
        </mat-tab>
      </mat-tab-group>
    </div>
  `,
  styles: [`
    h1 { font-weight: 400; margin-bottom: 24px; }
    .tab-content { padding: 16px 0; }
    .form-grid {
      display: flex; flex-direction: column; gap: 8px; max-width: 500px;
    }
    .form-row {
      display: flex; gap: 12px; align-items: flex-start; flex-wrap: wrap;
    }
  `]
})
export class AdminDashboardComponent {
  newUser = { fullName: '', email: '', password: '', role: 'student' };
  newGroup = { name: '', enrollmentYear: new Date().getFullYear() };
  newDiscipline = { name: '', code: '', description: '' };
  newAnnouncement = { title: '', content: '', isPinned: false };

  constructor(private api: ApiService, public auth: AuthService, private snackBar: MatSnackBar) {}

  registerUser() {
    const endpoint = this.newUser.role === 'teacher' ? '/auth/register-teacher' : '/auth/register-student';
    this.api.post(endpoint, {
      email: this.newUser.email,
      password: this.newUser.password,
      fullName: this.newUser.fullName
    }).subscribe({
      next: () => {
        this.snackBar.open('Пользователь создан', 'OK', { duration: 3000 });
        this.newUser = { fullName: '', email: '', password: '', role: 'student' };
      },
      error: err => this.snackBar.open(err.error?.message || 'Ошибка', 'OK', { duration: 5000 })
    });
  }

  createGroup() {
    this.api.post('/groups', this.newGroup).subscribe({
      next: () => {
        this.snackBar.open('Группа создана', 'OK', { duration: 3000 });
        this.newGroup = { name: '', enrollmentYear: new Date().getFullYear() };
      },
      error: err => this.snackBar.open(err.error?.message || 'Ошибка', 'OK', { duration: 5000 })
    });
  }

  createDiscipline() {
    this.api.post('/disciplines', this.newDiscipline).subscribe({
      next: () => {
        this.snackBar.open('Дисциплина создана', 'OK', { duration: 3000 });
        this.newDiscipline = { name: '', code: '', description: '' };
      },
      error: err => this.snackBar.open(err.error?.message || 'Ошибка', 'OK', { duration: 5000 })
    });
  }

  createAnnouncement() {
    this.api.post('/announcements', this.newAnnouncement).subscribe({
      next: () => {
        this.snackBar.open('Объявление опубликовано', 'OK', { duration: 3000 });
        this.newAnnouncement = { title: '', content: '', isPinned: false };
      },
      error: err => this.snackBar.open(err.error?.message || 'Ошибка', 'OK', { duration: 5000 })
    });
  }
}

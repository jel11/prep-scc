import { Component, Input, OnInit, OnDestroy } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router } from '@angular/router';
import { MatCardModule } from '@angular/material/card';
import { MatRadioModule } from '@angular/material/radio';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatProgressBarModule } from '@angular/material/progress-bar';
import { MatSnackBar, MatSnackBarModule } from '@angular/material/snack-bar';
import { FormsModule } from '@angular/forms';
import { ApiService } from '@core/api/api.service';

@Component({
  selector: 'app-test-taking-page',
  standalone: true,
  imports: [
    CommonModule, FormsModule,
    MatCardModule, MatRadioModule, MatButtonModule, MatIconModule, MatProgressBarModule, MatSnackBarModule
  ],
  template: `
    <div class="page-container">
      @if (result) {
        <mat-card class="result-card">
          <mat-card-header>
            <mat-icon mat-card-avatar [class]="'grade-' + result.grade">
              {{ result.grade >= 3 ? 'check_circle' : 'cancel' }}
            </mat-icon>
            <mat-card-title>Результат теста</mat-card-title>
          </mat-card-header>
          <mat-card-content>
            <div class="result-grid">
              <div class="result-item">
                <span class="label">Оценка</span>
                <span class="value" [class]="'grade-' + result.grade">{{ result.grade }}</span>
              </div>
              <div class="result-item">
                <span class="label">Правильных</span>
                <span class="value">{{ result.correctCount }} / {{ result.totalQuestions }}</span>
              </div>
              <div class="result-item">
                <span class="label">Процент</span>
                <span class="value">{{ result.score }}%</span>
              </div>
            </div>
            <mat-progress-bar mode="determinate" [value]="result.score" [color]="result.grade >= 3 ? 'primary' : 'warn'"></mat-progress-bar>
          </mat-card-content>
          <mat-card-actions>
            <button mat-button (click)="goBack()">Вернуться к дисциплине</button>
          </mat-card-actions>
        </mat-card>
      } @else if (testData) {
        <div class="test-header">
          <h1>Тест</h1>
          @if (testData.timeLimitMinutes && timeRemaining > 0) {
            <div class="timer" [class.urgent]="timeRemaining < 60">
              <mat-icon>timer</mat-icon>
              {{ formatTime(timeRemaining) }}
            </div>
          }
        </div>

        @for (question of testData.questions; track question.id; let i = $index) {
          <mat-card class="question-card">
            <mat-card-header>
              <mat-card-title>Вопрос {{ i + 1 }}</mat-card-title>
            </mat-card-header>
            <mat-card-content>
              <p class="question-text">{{ question.text }}</p>
              <mat-radio-group [(ngModel)]="answers[question.id]" class="answers">
                @for (answer of question.answers; track answer.id) {
                  <mat-radio-button [value]="answer.id">{{ answer.text }}</mat-radio-button>
                }
              </mat-radio-group>
            </mat-card-content>
          </mat-card>
        }

        <button mat-raised-button color="primary" (click)="submitTest()" [disabled]="submitting" class="submit-btn">
          {{ submitting ? 'Отправка...' : 'Завершить тест' }}
        </button>
      } @else {
        <p>Загрузка теста...</p>
      }
    </div>
  `,
  styles: [`
    .test-header { display: flex; justify-content: space-between; align-items: center; margin-bottom: 16px; }
    h1 { font-weight: 400; }
    .timer {
      display: flex; align-items: center; gap: 4px;
      font-size: 20px; font-weight: 500; color: #1565c0;
    }
    .timer.urgent { color: #d32f2f; }
    .question-card { margin-bottom: 16px; }
    .question-text { font-size: 16px; margin-bottom: 16px; }
    .answers { display: flex; flex-direction: column; gap: 8px; }
    .submit-btn { width: 100%; padding: 12px; font-size: 16px; }
    .result-card { max-width: 500px; margin: 0 auto; }
    .result-grid { display: flex; justify-content: space-around; padding: 24px 0; }
    .result-item { text-align: center; }
    .result-item .label { display: block; color: #757575; font-size: 14px; }
    .result-item .value { display: block; font-size: 32px; font-weight: 500; }
  `]
})
export class TestTakingPageComponent implements OnInit, OnDestroy {
  @Input() testId!: string;

  testData: any;
  answers: Record<string, string> = {};
  result: any;
  submitting = false;
  timeRemaining = 0;
  private timerInterval?: ReturnType<typeof setInterval>;

  constructor(private api: ApiService, private router: Router, private snackBar: MatSnackBar) {}

  ngOnInit() {
    this.api.post<any>(`/tests/${this.testId}/attempts`, {}).subscribe({
      next: data => {
        this.testData = data;
        if (data.timeLimitMinutes) {
          this.timeRemaining = data.timeLimitMinutes * 60;
          this.startTimer();
        }
      },
      error: err => {
        this.snackBar.open(err.error?.message || 'Ошибка загрузки теста', 'OK', { duration: 5000 });
      }
    });
  }

  ngOnDestroy() {
    if (this.timerInterval) clearInterval(this.timerInterval);
  }

  startTimer() {
    this.timerInterval = setInterval(() => {
      this.timeRemaining--;
      if (this.timeRemaining <= 0) {
        clearInterval(this.timerInterval);
        this.submitTest();
      }
    }, 1000);
  }

  formatTime(seconds: number): string {
    const m = Math.floor(seconds / 60);
    const s = seconds % 60;
    return `${m}:${s.toString().padStart(2, '0')}`;
  }

  submitTest() {
    if (this.submitting) return;
    this.submitting = true;
    if (this.timerInterval) clearInterval(this.timerInterval);

    const answersList = Object.entries(this.answers).map(([questionId, selectedAnswerId]) => ({
      questionId,
      selectedAnswerId
    }));

    this.api.post<any>(`/tests/${this.testId}/attempts/${this.testData.attemptId}/submit`, {
      answers: answersList
    }).subscribe({
      next: data => this.result = data,
      error: () => {
        this.submitting = false;
        this.snackBar.open('Ошибка отправки теста', 'OK', { duration: 5000 });
      }
    });
  }

  goBack() {
    this.router.navigate(['/disciplines']);
  }
}

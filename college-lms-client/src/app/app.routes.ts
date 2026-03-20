import { Routes } from '@angular/router';
import { authGuard } from './core/auth/auth.guard';
import { roleGuard } from './core/auth/role.guard';

export const routes: Routes = [
  {
    path: '',
    loadComponent: () => import('./features/home/pages/home-page.component').then(m => m.HomePageComponent)
  },
  {
    path: 'login',
    loadComponent: () => import('./features/auth/pages/login-page.component').then(m => m.LoginPageComponent)
  },
  {
    path: 'teachers',
    loadComponent: () => import('./features/teachers/pages/teacher-list-page.component').then(m => m.TeacherListPageComponent)
  },
  {
    path: 'teachers/:id',
    loadComponent: () => import('./features/teachers/pages/teacher-detail-page.component').then(m => m.TeacherDetailPageComponent)
  },
  {
    path: 'disciplines',
    loadComponent: () => import('./features/disciplines/pages/discipline-list-page.component').then(m => m.DisciplineListPageComponent)
  },
  {
    path: 'disciplines/:id',
    loadComponent: () => import('./features/disciplines/pages/discipline-detail-page.component').then(m => m.DisciplineDetailPageComponent)
  },
  {
    path: 'tests/:testId',
    loadComponent: () => import('./features/testing/pages/test-taking-page.component').then(m => m.TestTakingPageComponent),
    canActivate: [authGuard]
  },
  {
    path: 'journal',
    loadComponent: () => import('./features/journals/pages/journal-page.component').then(m => m.JournalPageComponent),
    canActivate: [authGuard]
  },
  {
    path: 'rating',
    loadComponent: () => import('./features/journals/pages/rating-page.component').then(m => m.RatingPageComponent),
    canActivate: [authGuard]
  },
  {
    path: 'schedule',
    loadComponent: () => import('./features/schedule/pages/schedule-page.component').then(m => m.SchedulePageComponent)
  },
  {
    path: 'admin',
    loadComponent: () => import('./features/admin/pages/admin-dashboard.component').then(m => m.AdminDashboardComponent),
    canActivate: [authGuard, roleGuard],
    data: { roles: ['Admin', 'Teacher'] }
  },
  {
    path: '**',
    redirectTo: ''
  }
];

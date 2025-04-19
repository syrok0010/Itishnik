import { Routes } from '@angular/router';
import { HomeComponent } from './home/home.component';
import { isAdminOrTeacherGuard } from '../api-authorization/auth-guards';

export const routes: Routes = [
  { path: '', component: HomeComponent },
  {
    path: 'courses',
    canMatch: [isAdminOrTeacherGuard],
    loadComponent: () =>
      import('./teacher/courses-page/courses-page.component'),
  },
  {
    path: 'teacher/tasks',
    loadComponent: () => import('./teacher/tasks-page/tasks-page.component'),
  },
];

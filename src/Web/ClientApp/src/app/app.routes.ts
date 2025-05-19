import { Routes } from '@angular/router';
import { isAdminOrTeacherGuard } from '../api-authorization/auth-guards';
import { taskResolver } from './task-resolver';

export const routes: Routes = [
  { path: '', redirectTo: '/courses', pathMatch: 'full' },
  {
    path: 'courses',
    canMatch: [isAdminOrTeacherGuard],
    loadComponent: () =>
      import('./teacher/courses-page/courses-page.component'),
  },
  {
    path: 'tasks',
    canMatch: [isAdminOrTeacherGuard],
    loadComponent: () => import('./teacher/tasks-page.component'),
  },
  {
    path: 'tasks/:id',
    canMatch: [isAdminOrTeacherGuard],
    resolve: {
      task: taskResolver,
    },
    loadComponent: () => import('./teacher/task-page/task-page.component'),
  },
];

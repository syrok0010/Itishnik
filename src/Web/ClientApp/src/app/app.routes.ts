import { Routes } from '@angular/router';
import { isAdminOrTeacherGuard } from '../api-authorization/auth-guards';
import { taskResolver } from './task-resolver';

export const routes: Routes = [
  { path: '', redirectTo: '/courses', pathMatch: 'full' },
  {
    path: 'activate',
    loadComponent: () =>
      import('./activate-student-page/activate-student-page.component'),
  },
  {
    path: 'courses',
    canMatch: [isAdminOrTeacherGuard],
    loadComponent: () =>
      import('./teacher/courses-page/courses-page.component'),
  },
  {
    path: 'courses/:id',
    canMatch: [isAdminOrTeacherGuard],
    loadComponent: () => import('./teacher/course-page/course-page.component'),
  },
  {
    path: 'tasks',
    canMatch: [isAdminOrTeacherGuard],
    loadComponent: () => import('./teacher/tasks-page.component'),
  },
  {
    path: 'tasks/create',
    canMatch: [isAdminOrTeacherGuard],
    loadComponent: () =>
      import('./teacher/create-task-page/create-task-page.component'),
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

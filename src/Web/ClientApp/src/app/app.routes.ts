import { Routes } from '@angular/router';
import {
  isAdminOrTeacherGuard,
  isStudentGuard,
} from '../api-authorization/auth-guards';
import { taskResolver } from './teacher/task-resolver';

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
    path: 'courses',
    canMatch: [isStudentGuard],
    loadComponent: () => import('./student/student-courses-page.component'),
  },
  {
    path: 'courses/:id',
    canMatch: [isAdminOrTeacherGuard],
    canActivateChild: [isAdminOrTeacherGuard],
    loadComponent: () => import('./teacher/course-page/course-page.component'),
    children: [
      {
        path: '',
        redirectTo: 'taskBlocks',
        pathMatch: 'full',
      },
      {
        path: 'taskBlocks',
        loadComponent: () =>
          import(
            './components/task-blocks-accordion/task-blocks-accordion.component'
          ),
      },
      {
        path: 'students',
        loadComponent: () =>
          import('./components/course-students/course-students.component'),
      },
    ],
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

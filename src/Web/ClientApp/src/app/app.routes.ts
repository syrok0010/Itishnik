import { Routes } from '@angular/router';
import {
  isAdminGuard,
  isAdminOrTeacherGuard,
  isStudentGuard,
  isTeacherGuard,
} from '../api-authorization/auth-guards';
import { taskResolver } from './teacher/task-resolver';
import { studentCoursePageResolver } from './student/student-course-page/student-course-page.resolver';
import { coursePageResolver } from './teacher/course-page/course-page.resolver';
import { tuiGenerateDialogableRoute as tuiRouteDialog } from '@taiga-ui/kit';

export const routes: Routes = [
  { path: '', redirectTo: '/courses', pathMatch: 'full' },
  {
    path: 'activate',
    canMatch: [isStudentGuard],
    loadComponent: () =>
      import('./student/activate-student-page/activate-student-page.component'),
  },
  {
    path: 'activate',
    canMatch: [isTeacherGuard],
    loadComponent: () => import('./teacher/activate-teacher-page.component'),
  },
  {
    path: 'teachers',
    canMatch: [isAdminGuard],
    loadComponent: () => import('./admin/teachers-page.component'),
  },
  {
    path: 'courses',
    canMatch: [isAdminOrTeacherGuard],
    loadComponent: () =>
      import('./teacher/courses-page/courses-page.component'),
    children: [
      tuiRouteDialog(
        () => import('./components/create-course-dialog.component'),
        {
          dismissible: true,
          label: 'Создать новый курс',
          path: 'create',
        },
      ),
    ],
  },
  {
    path: 'courses',
    canMatch: [isStudentGuard],
    loadComponent: () => import('./student/student-courses-page.component'),
  },
  {
    path: 'courses/:id',
    canMatch: [isStudentGuard],
    resolve: {
      studentCoursePageResolver,
    },
    loadComponent: () =>
      import('./student/student-course-page/student-course-page.component'),
  },
  {
    path: 'grades',
    canMatch: [isStudentGuard],
    loadComponent: () =>
      import('./student/student-grades-page/student-grades-page.component'),
  },
  {
    path: 'courses/:id',
    canMatch: [isAdminOrTeacherGuard],
    canActivateChild: [isAdminOrTeacherGuard],
    resolve: {
      coursePageResolver,
    },
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
          import('./teacher/course-page/students-tab.component'),
      },
      {
        path: 'grades',
        loadComponent: () =>
          import(
            './components/course-grades-table/course-grades-table.component'
          ),
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

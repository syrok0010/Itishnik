import { Routes } from '@angular/router';
import { HomeComponent } from './home/home.component';

export const routes: Routes = [
  { path: '', component: HomeComponent },
  {
    path: 'teacher/courses',
    loadComponent: () =>
      import('./teacher/courses-page/courses-page.component').then(
        (x) => x.CoursesPageComponent,
      ),
  },
  {
    path: 'teacher/tasks',
    loadComponent: () => import('./teacher/tasks-page/tasks-page.component'),
  },
];

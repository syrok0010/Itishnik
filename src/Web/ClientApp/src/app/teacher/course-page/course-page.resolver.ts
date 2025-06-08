import { ResolveFn, Router } from '@angular/router';
import { CoursesFacadeService } from '../courses-facade.service';
import { inject } from '@angular/core';

export const coursePageResolver: ResolveFn<void> = async (route) => {
  const coursesFacade = inject(CoursesFacadeService);
  const router = inject(Router);

  try {
    await coursesFacade.setCurrentCourseId(route.paramMap.get('id'));
  } catch (error) {
    await router.navigate(['/courses']);
    return Promise.reject('Task loading failed, navigation redirected.');
  }
};

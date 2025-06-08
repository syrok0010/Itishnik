import { ResolveFn, Router } from '@angular/router';
import { inject } from '@angular/core';
import { StudentCoursesFacadeService } from '../student-courses-facade.service';

export const studentCoursePageResolver: ResolveFn<void> = async (route) => {
  const studentCourseFacade = inject(StudentCoursesFacadeService);
  const router = inject(Router);

  try {
    await studentCourseFacade.setCurrentCourseId(route.paramMap.get('id'));
  } catch (error) {
    await router.navigate(['/courses']);
    return Promise.reject('Course loading failed, navigation redirected.');
  }
};

import { ActivatedRouteSnapshot, ResolveFn, Router } from '@angular/router';
import { inject } from '@angular/core';
import { TasksFacadeService } from './tasks-facade.service';

export const taskResolver: ResolveFn<void> = async (
  route: ActivatedRouteSnapshot,
): Promise<void> => {
  const tasksFacade = inject(TasksFacadeService);
  const router = inject(Router);

  try {
    await tasksFacade.setCurrentTaskId(route.paramMap.get('id'));
  } catch (error) {
    await router.navigate(['/tasks']);
    return Promise.reject('Task loading failed, navigation redirected.');
  }
};

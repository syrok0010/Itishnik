import { inject } from '@angular/core';
import { CanActivateFn, CanMatchFn } from '@angular/router';
import { UsersFacadeService } from '../app/users-facade.service';
import { map } from 'rxjs';

const adminRole = 'Administrator';
const teacherRole = 'Teacher';

export const isAdminOrTeacherGuard: CanActivateFn | CanMatchFn = () =>
  inject(UsersFacadeService).authInfo$.pipe(
    map((v) => v.roles.includes(adminRole) || v.roles.includes(teacherRole)),
  );

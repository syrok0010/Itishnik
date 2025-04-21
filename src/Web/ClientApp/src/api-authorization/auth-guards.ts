import { inject } from '@angular/core';
import { CanActivateFn, CanMatchFn } from '@angular/router';
import { AuthClient } from '../app/web-api-client';
import { map } from 'rxjs';

const adminRole = 'Administrator';
const teacherRole = 'Teacher';

export const isAdminOrTeacherGuard: CanActivateFn | CanMatchFn = () => {
  return inject(AuthClient)
    .authInfo()
    .pipe(
      map((v) => v.roles.includes(adminRole) || v.roles.includes(teacherRole)),
    );
};

import { HttpInterceptorFn, HttpErrorResponse } from '@angular/common/http';
import { catchError } from 'rxjs/operators';
import { throwError } from 'rxjs';
import { inject } from '@angular/core';
import { baseUrlToken } from '../app/app.config';

export const authorizeInterceptor: HttpInterceptorFn = (req, next) => {
  const baseUrl = inject(baseUrlToken);
  const loginUrl = `${baseUrl}Identity/Account/Login`;

  return next(req).pipe(
    catchError((error: HttpErrorResponse) => {
      if (error.status === 401) {
        window.location.href = `${loginUrl}?ReturnUrl=${window.location.pathname}`;
      }
      return throwError(() => error);
    }),
  );
};

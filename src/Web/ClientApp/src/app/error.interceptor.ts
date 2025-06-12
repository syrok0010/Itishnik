import { HttpErrorResponse, HttpInterceptorFn } from '@angular/common/http';
import { catchError, mergeMap } from 'rxjs/operators';
import { from, throwError } from 'rxjs';
import { inject } from '@angular/core';
import { TuiAlertService } from '@taiga-ui/core';

export const errorInterceptor: HttpInterceptorFn = (req, next) => {
  const alerts = inject(TuiAlertService);

  return next(req).pipe(
    catchError((error: HttpErrorResponse) =>
      from(error.error.text()).pipe(
        mergeMap((blobText: string) => {
          try {
            let errorMessage: string;
            let parsedError = JSON.parse(blobText);
            switch (error.status) {
              case 400:
                if (parsedError && parsedError.errors) {
                  errorMessage = Object.values(parsedError.errors).join('\n');
                } else if (parsedError && parsedError.title) {
                  errorMessage = parsedError.title;
                } else if (parsedError && parsedError[0]) {
                  errorMessage = (parsedError as string[]).join('\n');
                } else {
                  errorMessage = 'Некорректный запрос.';
                }
                break;
              case 403:
                errorMessage = 'У вас нет прав для выполнения этой операции.';
                break;
            }

            errorMessage ??=
              'Внутренняя ошибка сервера. Пожалуйста, попробуйте позже.';
            alerts
              .open(errorMessage, {
                autoClose: 5000,
                appearance: 'negative',
              })
              .subscribe();
          } finally {
            return throwError(() => error);
          }
        }),
      ),
    ),
  );
};

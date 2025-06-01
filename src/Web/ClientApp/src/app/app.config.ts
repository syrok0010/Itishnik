import {
  ApplicationConfig,
  InjectionToken,
  LOCALE_ID,
  provideZoneChangeDetection,
} from '@angular/core';
import { provideRouter, withViewTransitions } from '@angular/router';

import { routes } from './app.routes';
import { provideHttpClient, withInterceptors } from '@angular/common/http';
import { authorizeInterceptor } from '../api-authorization/authorize.interceptor';
import { provideEventPlugins } from '@taiga-ui/event-plugins';
import { provideAnimations } from '@angular/platform-browser/animations';
import { TUI_LANGUAGE, TUI_RUSSIAN_LANGUAGE } from '@taiga-ui/i18n';
import { of } from 'rxjs';
import { errorInterceptor } from './error.interceptor';

export function getBaseUrl() {
  return document.getElementsByTagName('base')[0].href;
}
export const baseUrlToken = new InjectionToken<string>('BASE_URL');

export const appConfig: ApplicationConfig = {
  providers: [
    { provide: LOCALE_ID, useValue: 'ru-RU' },
    {
      provide: TUI_LANGUAGE,
      useValue: of(TUI_RUSSIAN_LANGUAGE),
    },
    provideZoneChangeDetection({ eventCoalescing: true }),
    provideRouter(routes, withViewTransitions()),
    provideAnimations(),
    { provide: baseUrlToken, useFactory: getBaseUrl, deps: [] },
    provideHttpClient(
      withInterceptors([authorizeInterceptor, errorInterceptor]),
    ),
    provideEventPlugins(),
  ],
};

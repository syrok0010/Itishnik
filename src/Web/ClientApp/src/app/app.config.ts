import { ApplicationConfig, provideZoneChangeDetection } from '@angular/core';
import { provideRouter, withViewTransitions } from '@angular/router';

import { routes } from './app.routes';
import {
  HTTP_INTERCEPTORS,
  provideHttpClient,
  withInterceptorsFromDi,
} from '@angular/common/http';
import { AuthorizeInterceptor } from '../api-authorization/authorize.interceptor';
import { NG_EVENT_PLUGINS } from '@taiga-ui/event-plugins';
import { provideAnimations } from '@angular/platform-browser/animations';

export function getBaseUrl() {
  return document.getElementsByTagName('base')[0].href;
}
export const appConfig: ApplicationConfig = {
  providers: [
    provideZoneChangeDetection({ eventCoalescing: true }),
    provideRouter(routes, withViewTransitions()),
    provideAnimations(),
    { provide: 'BASE_URL', useFactory: getBaseUrl, deps: [] },
    { provide: HTTP_INTERCEPTORS, useClass: AuthorizeInterceptor, multi: true },
    provideHttpClient(withInterceptorsFromDi()),
    NG_EVENT_PLUGINS,
  ],
};

import { Injectable, inject } from '@angular/core';
import {
  Router,
  Event,
  NavigationStart,
  NavigationEnd,
  NavigationCancel,
  NavigationError,
} from '@angular/router';
import {
  BehaviorSubject,
  combineLatest,
  Observable,
  of,
  startWith,
  switchMap,
  takeUntil,
  timer,
} from 'rxjs';
import { map, filter, distinctUntilChanged, shareReplay } from 'rxjs/operators';
import { UntilDestroy, untilDestroyed } from '@ngneat/until-destroy';
import { PolymorpheusContent } from '@taiga-ui/polymorpheus';

@UntilDestroy()
@Injectable({
  providedIn: 'root',
})
export class GlobalLoadingService {
  private readonly routerLoading = new BehaviorSubject<boolean>(false);
  private readonly manualLoading = new BehaviorSubject<boolean>(false);
  private readonly loaderContent = new BehaviorSubject<PolymorpheusContent>(
    null,
  );

  private router = inject(Router);

  private readonly LOADER_DELAY_MS = 200;

  private readonly isLoadingActual$: Observable<boolean> = combineLatest([
    this.routerLoading,
    this.manualLoading,
  ]).pipe(
    map(([routerState, manualState]) => routerState || manualState),
    distinctUntilChanged(),
    untilDestroyed(this),
  );

  public readonly loaderContent$ = this.loaderContent.asObservable();

  public readonly showLoader$: Observable<boolean> = this.isLoadingActual$.pipe(
    switchMap((isLoading) =>
      !isLoading
        ? of(false)
        : timer(this.LOADER_DELAY_MS).pipe(
            map(() => true),
            takeUntil(
              this.isLoadingActual$.pipe(filter((loading) => !loading)),
            ),
            startWith(false),
          ),
    ),
    distinctUntilChanged(),
    shareReplay({ bufferSize: 1, refCount: true }),
    untilDestroyed(this),
  );

  constructor() {
    this.router.events
      .pipe(
        filter(
          (event) =>
            event instanceof NavigationStart ||
            event instanceof NavigationEnd ||
            event instanceof NavigationCancel ||
            event instanceof NavigationError,
        ),
        untilDestroyed(this),
      )
      .subscribe((event: Event) => {
        if (event instanceof NavigationStart) {
          this.routerLoading.next(true);
        }

        if (
          event instanceof NavigationEnd ||
          event instanceof NavigationCancel ||
          event instanceof NavigationError
        ) {
          this.routerLoading.next(false);
        }
      });
  }

  /**
   * Устанавливает состояние ручной загрузки.
   * @param isLoading true, если ручная операция началась, false - если закончилась.
   * @param text Текст, который будет отображен
   */
  public setManualLoading(
    isLoading: boolean,
    text: PolymorpheusContent = null,
  ): void {
    this.manualLoading.next(isLoading);
    this.loaderContent.next(isLoading ? text : null);
  }
}

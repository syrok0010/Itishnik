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
  Subject,
  switchMap,
  takeUntil,
  timer,
} from 'rxjs';
import {
  map,
  filter,
  distinctUntilChanged,
  shareReplay,
  tap,
} from 'rxjs/operators';
import { UntilDestroy, untilDestroyed } from '@ngneat/until-destroy';

@UntilDestroy()
@Injectable({
  providedIn: 'root',
})
export class GlobalLoadingService {
  private readonly routerLoading = new BehaviorSubject<boolean>(false);
  private readonly manualLoading = new BehaviorSubject<boolean>(false);

  private router = inject(Router);

  private readonly loadStarted$ = new Subject<void>();

  private readonly isLoadingActual$: Observable<boolean> = combineLatest([
    this.routerLoading,
    this.manualLoading,
  ]).pipe(
    map(([routerState, manualState]) => routerState || manualState),
    distinctUntilChanged(),
    tap((isLoading) => {
      if (isLoading) {
        this.loadStarted$.next();
      }
    }),
    untilDestroyed(this),
  );

  public readonly showLoader$: Observable<boolean> = this.isLoadingActual$.pipe(
    switchMap((isLoading) =>
      isLoading
        ? of(true)
        : of(true).pipe(
            switchMap(() =>
              timer(400).pipe(
                map(() => false),
                startWith(true),
                takeUntil(
                  this.isLoadingActual$.pipe(filter((loading) => loading)),
                ),
              ),
            ),
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
   */
  public setManualLoading(isLoading: boolean): void {
    this.manualLoading.next(isLoading);
  }
}

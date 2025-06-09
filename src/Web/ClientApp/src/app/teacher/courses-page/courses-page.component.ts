import {
  ChangeDetectionStrategy,
  Component,
  computed,
  effect,
  inject,
  signal,
} from '@angular/core';
import { TuiSortChange, TuiTable } from '@taiga-ui/addon-table';
import { CoursesFacadeService } from '../courses-facade.service';
import {
  TuiButton,
  TuiIcon,
  TuiScrollable,
  TuiScrollbar,
  TuiTitle,
} from '@taiga-ui/core';
import { RouterLink, RouterOutlet } from '@angular/router';
import { TuiCell } from '@taiga-ui/layout';
import {
  CdkVirtualForOf,
  CdkVirtualScrollViewport,
} from '@angular/cdk/scrolling';
import { CdkAutoSizeVirtualScroll } from '@angular/cdk-experimental/scrolling';
import { TuiEditorSocket } from '@taiga-ui/editor';
import { toSignal } from '@angular/core/rxjs-interop';
import { GlobalLoadingService } from '../../global-loading.service';

@Component({
  selector: 'app-courses-page',
  imports: [
    TuiTable,
    TuiButton,
    TuiIcon,
    RouterLink,
    TuiCell,
    TuiTitle,
    CdkAutoSizeVirtualScroll,
    TuiScrollable,
    TuiScrollbar,
    CdkVirtualScrollViewport,
    CdkVirtualForOf,
    TuiEditorSocket,
    RouterOutlet,
  ],
  templateUrl: './courses-page.component.html',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export default class CoursesPageComponent {
  coursesFacade = inject(CoursesFacadeService);
  globalLoading = inject(GlobalLoadingService);

  coursesList = toSignal(this.coursesFacade.coursesList$);
  isLoading = toSignal(this.coursesFacade.isLoading$);
  isAtBottom = signal(false);
  initialLoadDone = signal(false);

  shouldShowLoader = computed(
    () => this.isLoading() && (!this.coursesList() || this.isAtBottom()),
  );

  readonly columns = ['course', 'students', 'taskBlocks', 'description'];

  constructor() {
    effect(() => {
      this.globalLoading.setManualLoading(this.shouldShowLoader());
    });
    effect(() => {
      if (this.coursesList() !== undefined) {
        this.initialLoadDone.set(true);
      }
    });
  }

  sort(e: TuiSortChange<{ name: any }>) {
    this.coursesFacade.setSorting(e.sortDirection === 1);
  }

  async onViewportScroll(viewport: CdkVirtualScrollViewport) {
    const end = viewport.getRenderedRange().end;
    const total = viewport.getDataLength();

    const prefetchThreshold = 10;
    if (total > 0 && end >= total - prefetchThreshold)
      await this.coursesFacade.nextPage();

    this.isAtBottom.set(total > 0 && end === total);
  }
}

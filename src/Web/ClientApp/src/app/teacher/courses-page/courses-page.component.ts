import { ChangeDetectionStrategy, Component, inject } from '@angular/core';
import { TuiSortChange, TuiTable } from '@taiga-ui/addon-table';
import { CoursesFacadeService } from '../courses-facade.service';
import { AsyncPipe } from '@angular/common';
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
  CdkFixedSizeVirtualScroll,
  CdkVirtualForOf,
  CdkVirtualScrollViewport,
} from '@angular/cdk/scrolling';
import { pageSize } from '../tasks-facade.service';
import { TuiEditorSocket } from '@taiga-ui/editor';

@Component({
  selector: 'app-courses-page',
  imports: [
    TuiTable,
    AsyncPipe,
    TuiButton,
    TuiIcon,
    RouterLink,
    TuiCell,
    TuiTitle,
    CdkFixedSizeVirtualScroll,
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
  coursesList$ = this.coursesFacade.coursesList$;

  readonly columns = ['course', 'students', 'taskBlocks', 'description'];

  sort(e: TuiSortChange<{ name: any }>) {
    this.coursesFacade.setSorting(e.sortDirection === 1);
  }

  async scrollChanged(currentElement: number, total: number) {
    if (total - currentElement <= pageSize) await this.coursesFacade.nextPage();
  }
}

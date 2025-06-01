import { ChangeDetectionStrategy, Component, inject } from '@angular/core';
import { TuiSortChange, TuiTable } from '@taiga-ui/addon-table';
import { CoursesFacadeService } from '../courses-facade.service';
import { AsyncPipe } from '@angular/common';
import {
  TuiButton,
  tuiDialog,
  TuiIcon,
  TuiScrollable,
  TuiScrollbar,
  TuiTitle,
} from '@taiga-ui/core';
import { CreateCourseDialogComponent } from '../../components/create-course-dialog.component';
import { RouterLink } from '@angular/router';
import { TuiCell } from '@taiga-ui/layout';
import {
  CdkFixedSizeVirtualScroll,
  CdkVirtualForOf,
  CdkVirtualScrollViewport,
} from '@angular/cdk/scrolling';
import { pageSize } from '../tasks-facade.service';

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
  ],
  templateUrl: './courses-page.component.html',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export default class CoursesPageComponent {
  coursesFacade = inject(CoursesFacadeService);
  coursesList$ = this.coursesFacade.coursesList$;

  readonly columns = ['course', 'students', 'taskBlocks', 'description'];

  addCourse() {
    this.dialog(undefined).subscribe();
  }

  private readonly dialog = tuiDialog(CreateCourseDialogComponent, {
    dismissible: true,
    label: 'Создать новый курс',
  });

  sort(e: TuiSortChange<{ name: any }>) {
    this.coursesFacade.setSorting(e.sortDirection === 1);
  }

  async scrollChanged(currentElement: number, total: number) {
    if (total - currentElement <= pageSize) await this.coursesFacade.nextPage();
  }
}

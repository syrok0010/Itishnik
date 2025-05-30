import { ChangeDetectionStrategy, Component, inject } from '@angular/core';
import { TuiSortChange, TuiTable } from '@taiga-ui/addon-table';
import { CoursesFacadeService } from '../../courses-facade.service';
import { AsyncPipe } from '@angular/common';
import { TuiButton, tuiDialog, TuiIcon, TuiTitle } from '@taiga-ui/core';
import { CreateCourseDialogComponent } from '../../components/create-course-dialog.component';
import { RouterLink } from '@angular/router';
import { TuiCell } from '@taiga-ui/layout';

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
}

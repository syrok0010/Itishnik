import { ChangeDetectionStrategy, Component, inject } from '@angular/core';
import { TuiTable } from '@taiga-ui/addon-table';
import { CoursesFacadeService } from '../../courses-facade.service';
import { AsyncPipe } from '@angular/common';
import { TuiButton, tuiDialog, TuiIcon } from '@taiga-ui/core';
import { CreateCourseDialogComponent } from '../create-course-dialog.component';

@Component({
  selector: 'app-courses-page',
  imports: [TuiTable, AsyncPipe, TuiButton, TuiIcon],
  templateUrl: './courses-page.component.html',
  changeDetection: ChangeDetectionStrategy.OnPush,
  styles: ':host { @apply grow flex flex-col }',
})
export class CoursesPageComponent {
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
}

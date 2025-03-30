import { ChangeDetectionStrategy, Component, inject } from '@angular/core';
import { TuiTable } from '@taiga-ui/addon-table';
import { CoursesFacadeService } from '../../courses-facade.service';
import { AsyncPipe, NgForOf } from '@angular/common';

@Component({
  selector: 'app-courses-page',
  imports: [TuiTable, AsyncPipe],
  templateUrl: './courses-page.component.html',
  changeDetection: ChangeDetectionStrategy.OnPush,
  styles: ':host { @apply grow flex flex-col *:grow }',
})
export class CoursesPageComponent {
  coursesFacade = inject(CoursesFacadeService);
  coursesList$ = this.coursesFacade.coursesList$;

  readonly columns = [
    'course',
    'groups',
    'students',
    'taskBlocks',
    'description',
  ];
}

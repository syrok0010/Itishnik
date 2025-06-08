import {
  ChangeDetectionStrategy,
  Component,
  computed,
  inject,
} from '@angular/core';
import {
  TuiTable,
  TuiTableCell,
  TuiTableDirective,
} from '@taiga-ui/addon-table';
import { CoursesFacadeService } from '../../teacher/courses-facade.service';
import { toSignal } from '@angular/core/rxjs-interop';
import { map } from 'rxjs/operators';
import {
  TuiAutoColorPipe,
  TuiButton,
  TuiInitialsPipe,
  TuiScrollbar,
  TuiTitle,
} from '@taiga-ui/core';
import { TuiAvatar } from '@taiga-ui/kit';

@Component({
  selector: 'app-course-grades-table',
  imports: [
    TuiTableCell,
    TuiTableDirective,
    TuiTable,
    TuiAutoColorPipe,
    TuiAvatar,
    TuiTitle,
    TuiInitialsPipe,
    TuiButton,
    TuiScrollbar,
  ],
  templateUrl: './course-grades-table.component.html',
  styles: ``,
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export default class CourseGradesTableComponent {
  private readonly coursesFacade = inject(CoursesFacadeService);
  studentsAndGrades = toSignal(this.coursesFacade.currentCourseGrades$);
  taskBlocksNames = toSignal(
    this.coursesFacade.currentCourse$.pipe(
      map((c) =>
        c.taskBlocks
          .filter((tb) => tb.isPublic === true)
          .sort((a, b) => a.startTime.getTime() - b.startTime.getTime())
          .map((tb) => tb.name),
      ),
    ),
  );
  columns = computed(() => [
    'student',
    ...(!!this.studentsAndGrades()
      ? this.studentsAndGrades()[0].grades.map((g, i) => i.toString())
      : []),
  ]);
}

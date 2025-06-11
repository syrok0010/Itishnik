import {
  ChangeDetectionStrategy,
  Component,
  computed,
  effect,
  inject,
} from '@angular/core';
import {
  TuiTable,
  TuiTableCell,
  TuiTableDirective,
} from '@taiga-ui/addon-table';
import { CoursesFacadeService } from '../../teacher/courses-facade.service';
import { toSignal } from '@angular/core/rxjs-interop';
import { concatMap, debounceTime, map } from 'rxjs/operators';
import {
  TuiAutoColorPipe,
  TuiButton,
  TuiInitialsPipe,
  TuiScrollbar,
  TuiTextfield,
  TuiTitle,
} from '@taiga-ui/core';
import { TuiAvatar, TuiInputNumber } from '@taiga-ui/kit';
import { FormArray, FormControl, ReactiveFormsModule } from '@angular/forms';
import { of } from 'rxjs';
import { UntilDestroy, untilDestroyed } from '@ngneat/until-destroy';
import { GradedTaskBlockResponse } from '../../web-api-client';

@UntilDestroy()
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
    ReactiveFormsModule,
    TuiTextfield,
    TuiInputNumber,
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
  courseGradeControls = computed(
    () =>
      new FormArray(
        this.studentsAndGrades().map((g) => new FormControl(g.courseGrade)),
      ),
  );
  columns = computed(() => [
    'student',
    'courseGrade',
    'averageGrade',
    ...this.taskBlocksNames().map((n, i) => i.toString()),
  ]);

  average(array: GradedTaskBlockResponse[]) {
    const finalArray = array?.filter((e) => !!e.grade) ?? [];
    return finalArray.length === 0
      ? 0
      : finalArray.reduce((acc, t) => acc + t.grade, 0) / finalArray.length;
  }

  constructor() {
    effect(() => {
      const controls = this.courseGradeControls();
      if (controls.length === 0) return;
      const data = this.studentsAndGrades();
      controls.valueChanges
        .pipe(
          debounceTime(1000),
          concatMap((a) => {
            for (let i = 0; i < a.length; i++) {
              if (a[i] === data[i].courseGrade) continue;
              const element = data[i];
              return this.coursesFacade.setStudentCourseGrade(
                element.courseId,
                element.studentId,
                a[i],
              );
            }
            return of(null);
          }),
          untilDestroyed(this),
        )
        .subscribe();
    });
    this.coursesFacade.refetchGrades();
  }
}

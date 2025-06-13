import {
  ChangeDetectionStrategy,
  Component,
  computed,
  effect,
  inject,
} from '@angular/core';
import { TuiTable } from '@taiga-ui/addon-table';
import { CoursesFacadeService } from '../../teacher/courses-facade.service';
import { toSignal } from '@angular/core/rxjs-interop';
import { concatMap, debounceTime, map } from 'rxjs/operators';
import {
  TuiAutoColorPipe,
  TuiButton,
  tuiDialog,
  TuiHint,
  TuiIcon,
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
import GradeTaskBlockDialogComponent from '../grade-task-block-dialog.component';
import { DecimalPipe } from '@angular/common';

@UntilDestroy()
@Component({
  selector: 'app-course-grades-table',
  imports: [
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
    TuiIcon,
    TuiHint,
    DecimalPipe,
  ],
  templateUrl: './course-grades-table.component.html',
  styles: ``,
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export default class CourseGradesTableComponent {
  private readonly coursesFacade = inject(CoursesFacadeService);
  private readonly gradeDialog = tuiDialog(GradeTaskBlockDialogComponent, {
    size: 'auto',
  });
  studentsAndGrades = toSignal(
    this.coursesFacade.currentCourseGrades$.pipe(
      map((v) => v.sort((a, b) => a.fullName.localeCompare(b.fullName))),
    ),
  );
  course = toSignal(this.coursesFacade.currentCourse$);
  studentList = toSignal(this.coursesFacade.currentCourseStudents$);
  courseGradeControls = computed(
    () =>
      new FormArray(
        this.studentsAndGrades().map(
          (g) =>
            new FormControl({
              value: g.courseGrade,
              disabled: g.grades.some((b) => !b.grade),
            }),
        ),
      ),
  );
  taskBlocks = computed(() => {
    const maxBlocksLength = Math.max(
      0,
      ...this.studentsAndGrades().map((s) => s.grades.length),
    );
    return this.studentsAndGrades()
      .find((s) => s.grades.length === maxBlocksLength)
      .grades.map((g) => ({
        id: g.taskBlockId,
        name: g.name,
      }));
  });
  columns = computed(() => [
    'student',
    'courseGrade',
    'averageGrade',
    ...this.taskBlocks().map((g) => g.id),
  ]);

  find(array: GradedTaskBlockResponse[], taskBlockId: string) {
    return array.find((gtb) => gtb.taskBlockId === taskBlockId);
  }

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

  showGradeDialog(
    taskBlockId: string,
    studentId: string,
    gradedTaskBlockId: string,
    hittable = true,
  ) {
    if (!hittable) return;
    this.coursesFacade.fetchGradedTaskBlock(
      this.course().id,
      taskBlockId,
      studentId,
    );

    this.gradeDialog({
      gradedTaskBlockId,
      courseId: this.course().id,
      taskBlockId: taskBlockId,
    }).subscribe();
  }

  inFuture(taskBlockId: string) {
    const date = this.course().taskBlocks.find(
      (tb) => tb.id === taskBlockId,
    )?.endTime;
    return Date.now() > date.getTime();
  }

  getHint(email: string) {
    return this.studentList().find((t) => t.email === email)?.group ?? '';
  }
}

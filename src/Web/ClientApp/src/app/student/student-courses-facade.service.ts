import { inject, Injectable } from '@angular/core';
import {
  EditSolutionCommand,
  GradedCourseResponse,
  GradedTaskBlockDto,
  SendFeedbackCommand,
  SolutionDto,
  StudentCourseResponse,
  StudentGradesResponse,
  StudentsClient,
} from '../web-api-client';
import { UntilDestroy, untilDestroyed } from '@ngneat/until-destroy';
import { map } from 'rxjs/operators';
import { BehaviorSubject, firstValueFrom } from 'rxjs';
import { TuiAlertService } from '@taiga-ui/core';

export interface StudentCoursesState {
  coursesList: GradedCourseResponse[];
  currentCourse: StudentCourseResponse | null;
  studentAllGrades: StudentGradesResponse[];
}

let _state: StudentCoursesState = {
  coursesList: [],
  currentCourse: null,
  studentAllGrades: [],
};

@UntilDestroy()
@Injectable({
  providedIn: 'root',
})
export class StudentCoursesFacadeService {
  studentClient = inject(StudentsClient);
  private readonly alerts = inject(TuiAlertService);
  private _store: BehaviorSubject<StudentCoursesState> = new BehaviorSubject(
    _state,
  );

  coursesList$ = this._store.pipe(map((state) => state.coursesList));
  currentCourse$ = this._store.pipe(map((state) => state.currentCourse));
  studentAllGrades$ = this._store.pipe(map((state) => state.studentAllGrades));

  constructor() {
    this.studentClient
      .getStudentGrades()
      .pipe(untilDestroyed(this))
      .subscribe((x) =>
        this._store.next(
          (_state = {
            ..._state,
            studentAllGrades: x,
          }),
        ),
      );
    this.studentClient
      .getCourses(1, 30)
      .pipe(untilDestroyed(this))
      .subscribe((x) =>
        this._store.next(
          (_state = {
            ..._state,
            coursesList: x.items,
          }),
        ),
      );
  }

  async setCurrentCourseId(id: string) {
    const currentCourse = await firstValueFrom(
      this.studentClient.getCourse(id),
    );
    this._store.next((_state = { ..._state, currentCourse }));
  }

  async startSolution(courseId: string, taskBlockId: string) {
    const response = await firstValueFrom(
      this.studentClient.startTaskBlock(courseId, taskBlockId),
    );
    this._store.next(
      (_state = {
        ..._state,
        currentCourse: new StudentCourseResponse({
          ..._state.currentCourse,
          taskBlocks: [
            ..._state.currentCourse.taskBlocks.filter(
              (tb) => tb.id !== taskBlockId,
            ),
            response,
          ],
        }),
      }),
    );
    this.alerts
      .open('Вы начали решение работы', {
        autoClose: 3000,
        appearance: 'positive',
      })
      .subscribe();
  }

  async saveSolution(
    courseId: string,
    taskBlockId: string,
    taskId: string,
    text: string,
  ) {
    const response = await firstValueFrom(
      this.studentClient.editSolution(
        courseId,
        taskBlockId,
        taskId,
        new EditSolutionCommand({
          id: courseId,
          blockId: taskBlockId,
          taskId,
          text,
        }),
      ),
    );
    this.alerts
      .open('Решение успешно сохранено.', {
        autoClose: 3000,
        appearance: 'positive',
      })
      .subscribe();

    this._store.next(
      (_state = {
        ..._state,
        currentCourse: new StudentCourseResponse({
          ..._state.currentCourse,
          taskBlocks: _state.currentCourse.taskBlocks.map((block) =>
            block.id === taskBlockId
              ? new GradedTaskBlockDto({
                  ...block,
                  solutions: block.solutions.map((solution) =>
                    solution.task.id === taskId
                      ? new SolutionDto({
                          ...response,
                          weight: solution.weight,
                          position: solution.position,
                        })
                      : solution,
                  ),
                })
              : block,
          ),
        }),
      }),
    );
  }

  async saveFeedback(courseId: string, taskBlockId: string, text: string) {
    await firstValueFrom(
      this.studentClient.sendFeedback(
        courseId,
        taskBlockId,
        new SendFeedbackCommand({
          courseId,
          blockId: taskBlockId,
          text,
        }),
      ),
    );
    this.alerts
      .open('Комментарий сохранен', {
        appearance: 'positive',
        autoClose: 3000,
      })
      .subscribe();
  }
}

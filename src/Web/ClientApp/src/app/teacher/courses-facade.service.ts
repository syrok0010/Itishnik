import { inject, Injectable } from '@angular/core';
import {
  AddTaskToBlockCommand,
  ChangeCourseDescriptionCommand,
  ChangeCourseTeacherCommand,
  ChangeTaskBlockDescriptionCommand,
  ChangeTaskBlockNameCommand,
  ChangeTaskBlockTimelineCommand,
  ChangeWeightsInBlockCommand,
  CourseListResponse,
  CourseResponse,
  CoursesClient,
  CourseStudentListResponse,
  CreateCourseCommand,
  CreateTaskBlockCommand,
  DeleteTaskFromBlockCommand,
  GradedTaskBlockDto,
  InviteStudentsToCourseCommand,
  SetStudentCourseGradeCommand,
  StudentGradesResponse,
  TaskBlockResponse,
} from '../web-api-client';
import {
  BehaviorSubject,
  firstValueFrom,
  Observable,
  switchMap,
  tap,
} from 'rxjs';
import { distinctUntilChanged, map } from 'rxjs/operators';
import { UntilDestroy, untilDestroyed } from '@ngneat/until-destroy';
import { TuiAlertService } from '@taiga-ui/core';

export const coursesPageSize = 20;

export interface CoursesState {
  coursesList: CourseListResponse[];
  totalPages: number | null;
  currentPage: number | null;
  ascending: boolean;
  isLoading: boolean;

  currentCourse: CourseResponse | null;
  currentCourseStudents: CourseStudentListResponse | null;
  currentCourseGrades: StudentGradesResponse[] | null;
  currentCourseFeedbacks: [string, string[]][];

  studentGradedBlocks: GradedTaskBlockDto[];
}

let _state: CoursesState = {
  coursesList: [],
  currentPage: null,
  totalPages: null,
  ascending: true,
  isLoading: true,

  currentCourse: null,
  currentCourseGrades: null,
  currentCourseStudents: null,
  currentCourseFeedbacks: [],

  studentGradedBlocks: [],
};

@UntilDestroy()
@Injectable({
  providedIn: 'root',
})
export class CoursesFacadeService {
  coursesClient = inject(CoursesClient);
  private readonly alerts = inject(TuiAlertService);

  private _store: BehaviorSubject<CoursesState> = new BehaviorSubject(_state);

  isLoading$ = this._store.pipe(map((state) => state.isLoading));
  coursesList$ = this._store.pipe(map((state) => state.coursesList));
  currentCourse$ = this._store.pipe(map((state) => state.currentCourse));
  currentCourseStudents$ = this._store.pipe(
    map((state) => state.currentCourseStudents.students),
  );
  currentCourseGrades$ = this._store.pipe(
    map((state) => state.currentCourseGrades),
  );
  studentTaskBlocks$ = this._store.pipe(
    map((state) => state.studentGradedBlocks),
  );

  constructor() {
    this._store
      .pipe(
        map((s) => s.ascending),
        distinctUntilChanged(),
        tap(() => this._store.next((_state = { ..._state, isLoading: true }))),
        switchMap((a) =>
          this.coursesClient.getCoursesList(a, 1, coursesPageSize),
        ),
        untilDestroyed(this),
      )
      .subscribe((x) =>
        this._store.next(
          (_state = {
            ..._state,
            totalPages: x.totalPages,
            currentPage: x.pageNumber,
            coursesList: x.items,
            isLoading: false,
          }),
        ),
      );
  }

  async setCurrentCourseId(courseId: string) {
    const course = await firstValueFrom(
      this.coursesClient.getCourseById(courseId),
    );
    const students = await firstValueFrom(
      this.coursesClient.getStudents(courseId),
    );
    const grades = await firstValueFrom(
      this.coursesClient.getStudentsAndGrades(courseId),
    );

    this._store.next(
      (_state = {
        ..._state,
        currentCourse: course,
        currentCourseStudents: students,
        currentCourseGrades: grades,
        currentCourseFeedbacks: [],
      }),
    );
  }

  async createCourse(name: string): Promise<void> {
    const response = await firstValueFrom(
      this.coursesClient.createCourse(
        new CreateCourseCommand({ name: name, description: '' }),
      ),
    );
    this.alerts
      .open('Курс создан', {
        autoClose: 3000,
        appearance: 'positive',
      })
      .subscribe();
    this._store.next(
      (_state = {
        ..._state,
        coursesList: [
          ..._state.coursesList,
          new CourseListResponse({
            id: response.id,
            name: response.name,
            description: response.description,
            studentsCount: 0,
            taskBlocksCount: 0,
          }),
        ],
      }),
    );
  }

  async updateDescription(
    courseId: string,
    description: string,
  ): Promise<void> {
    await firstValueFrom(
      this.coursesClient.changeDescription(
        courseId,
        new ChangeCourseDescriptionCommand({ id: courseId, description }),
      ),
    );
    this.alerts
      .open('Описание курса обновлено', {
        autoClose: 3000,
        appearance: 'positive',
      })
      .subscribe();
    this._store.next(
      (_state = {
        ..._state,
        coursesList: [
          ..._state.coursesList.map((c) =>
            c.id !== courseId
              ? c
              : new CourseListResponse({
                  ...c,
                  description,
                }),
          ),
        ],
      }),
    );
  }

  async addTasksToTaskBlock(
    courseId: string,
    taskBlockId: string,
    taskIds: string[],
  ): Promise<void> {
    if (taskIds.length === 0) return;
    let response: TaskBlockResponse;
    for (const taskId of taskIds) {
      response = await firstValueFrom(
        this.coursesClient.addTaskToBlock(
          courseId,
          taskBlockId,
          new AddTaskToBlockCommand({
            id: courseId,
            blockId: taskBlockId,
            taskId,
          }),
        ),
      );
    }
    this.alerts
      .open('Задачи добавлены в работу', {
        autoClose: 3000,
        appearance: 'positive',
      })
      .subscribe();
    this._store.next(
      (_state = {
        ..._state,
        currentCourse: new CourseResponse({
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
  }

  async removeTasksFromTaskBlock(
    courseId: string,
    taskBlockId: string,
    taskIds: string[],
  ): Promise<void> {
    await Promise.all(
      taskIds.map((taskId) =>
        firstValueFrom(
          this.coursesClient.deleteTaskFromBlock(
            courseId,
            taskBlockId,
            new DeleteTaskFromBlockCommand({
              id: courseId,
              blockId: taskBlockId,
              taskId,
            }),
          ),
        ),
      ),
    );
    this.alerts
      .open('Задача удалены из работы', {
        autoClose: 3000,
        appearance: 'positive',
      })
      .subscribe();
    this._store.next(
      (_state = {
        ..._state,
        currentCourse: new CourseResponse({
          ..._state.currentCourse,
          taskBlocks: [
            ..._state.currentCourse.taskBlocks.map((tb) =>
              tb.id !== taskBlockId
                ? tb
                : new TaskBlockResponse({
                    ...tb,
                    tasks: tb.tasks.filter((t) => !taskIds.includes(t.id)),
                  }),
            ),
          ],
        }),
      }),
    );
    this._store.next(_state);
  }

  async updateTaskBlock(
    courseId: string,
    taskBlockId: string,
    name: string,
    description: string,
    startTime: Date,
    endTime: Date,
    timeAllowed: string,
  ) {
    await firstValueFrom(
      this.coursesClient.changeTaskBlockName(
        courseId,
        taskBlockId,
        new ChangeTaskBlockNameCommand({
          name,
          taskBlockId,
          courseId,
        }),
      ),
    );
    await firstValueFrom(
      this.coursesClient.changeTaskBlockDescription(
        courseId,
        taskBlockId,
        new ChangeTaskBlockDescriptionCommand({
          description,
          taskBlockId,
          courseId,
        }),
      ),
    );
    const response = await firstValueFrom(
      this.coursesClient.changeTaskBlockTimeline(
        courseId,
        taskBlockId,
        new ChangeTaskBlockTimelineCommand({
          startTime,
          endTime,
          timeAllowed,
          taskBlockId,
          courseId,
        }),
      ),
    );
    this.alerts
      .open('Данные работы обновлены', {
        autoClose: 3000,
        appearance: 'positive',
      })
      .subscribe();
    this._store.next(
      (_state = {
        ..._state,
        currentCourse: new CourseResponse({
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
  }

  async updateTaskBlockWeights(
    courseId: string,
    taskBlockId: string,
    weights: number[],
  ): Promise<void> {
    const response = await firstValueFrom(
      this.coursesClient.changeWeights(
        courseId,
        taskBlockId,
        new ChangeWeightsInBlockCommand({
          id: courseId,
          blockId: taskBlockId,
          weights: weights,
        }),
      ),
    );
    this.alerts
      .open('Задачи и баллы сохранены', {
        autoClose: 3000,
        appearance: 'positive',
      })
      .subscribe();
    this._store.next(
      (_state = {
        ..._state,
        currentCourse: new CourseResponse({
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
  }

  setSorting(ascending: boolean) {
    this._store.next((_state = { ..._state, ascending }));
  }

  async updateCourseTeacher(
    courseId: string,
    teacherId: string,
  ): Promise<void> {
    const response = await firstValueFrom(
      this.coursesClient.changeTeacher(
        courseId,
        new ChangeCourseTeacherCommand({ courseId, newTeacherId: teacherId }),
      ),
    );
    this.alerts
      .open('Преподаватель на курсе изменен', {
        autoClose: 3000,
        appearance: 'positive',
      })
      .subscribe();
    this._store.next((_state = { ..._state, currentCourse: response }));
  }

  async publishTaskBlock(courseId: string, taskBlockId: string) {
    const response = await firstValueFrom(
      this.coursesClient.publishBlock(courseId, taskBlockId),
    );
    this.alerts
      .open('Работа опубликована для студентов', {
        autoClose: 3000,
        appearance: 'positive',
      })
      .subscribe();
    const grades = await firstValueFrom(
      this.coursesClient.getStudentsAndGrades(courseId),
    );
    this._store.next(
      (_state = {
        ..._state,
        currentCourseGrades: grades,
        currentCourse: new CourseResponse({
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
  }

  async createTaskBlock(courseId: string, name: string) {
    const response = await firstValueFrom(
      this.coursesClient.createTaskBlock(
        courseId,
        new CreateTaskBlockCommand({
          courseId,
          name,
          taskIds: [],
          weights: [],
        }),
      ),
    );
    this.alerts
      .open('Работа создана', {
        autoClose: 3000,
        appearance: 'positive',
      })
      .subscribe();
    this._store.next(
      (_state = {
        ..._state,
        currentCourse: new CourseResponse({
          ..._state.currentCourse,
          taskBlocks: [..._state.currentCourse.taskBlocks, response],
        }),
        coursesList: _state.coursesList.map((c) =>
          c.id !== courseId
            ? c
            : new CourseListResponse({
                ...c,
                taskBlocksCount: c.taskBlocksCount + 1,
              }),
        ),
      }),
    );
  }

  async nextPage() {
    if (_state.currentPage === _state.totalPages || _state.isLoading) return;

    this._store.next((_state = { ..._state, isLoading: true }));
    try {
      const response = await firstValueFrom(
        this.coursesClient.getCoursesList(
          _state.ascending,
          _state.currentPage + 1,
          coursesPageSize,
        ),
      );
      this._store.next(
        (_state = {
          ..._state,
          coursesList: [..._state.coursesList, ...response.items],
          currentPage: _state.currentPage + 1,
        }),
      );
    } finally {
      this._store.next((_state = { ..._state, isLoading: false }));
    }
  }

  async inviteStudents(courseId: string, emails: string[]) {
    const response = await firstValueFrom(
      this.coursesClient.inviteStudents(
        courseId,
        new InviteStudentsToCourseCommand({
          id: courseId,
          emails,
        }),
      ),
    );
    this._store.next((_state = { ..._state, currentCourseStudents: response }));
  }

  getFeedback(courseId: string, taskBlockId: string): Observable<string[]> {
    if (!_state.currentCourseFeedbacks[taskBlockId]) {
      this.coursesClient.getFeedbacks(courseId, taskBlockId).subscribe((r) =>
        this._store.next(
          (_state = {
            ..._state,
            currentCourseFeedbacks: [
              ..._state.currentCourseFeedbacks.filter(
                (e) => e[0] !== taskBlockId,
              ),
              [taskBlockId, r],
            ],
          }),
        ),
      );
    }

    return this._store.pipe(
      map(
        (state) =>
          (state.currentCourseFeedbacks.find((a) => a[0] === taskBlockId) ?? [
            '',
            [],
          ])[1],
      ),
    );
  }

  async setStudentCourseGrade(
    courseId: string,
    studentId: string,
    grade: number,
  ) {
    grade = !!grade ? grade : 0;
    await firstValueFrom(
      this.coursesClient.setStudentCourseGrade(
        courseId,
        new SetStudentCourseGradeCommand({ grade, studentId, courseId }),
      ),
    );
    this.alerts
      .open('Оценка студента за курс обновлена', {
        autoClose: 3000,
        appearance: 'positive',
      })
      .subscribe();
  }

  refetchGrades() {
    if (!_state.currentCourse?.id) return;
    this.coursesClient
      .getStudentsAndGrades(_state.currentCourse.id)
      .pipe(untilDestroyed(this))
      .subscribe((grades) =>
        this._store.next(
          (_state = {
            ..._state,
            currentCourseGrades: grades,
          }),
        ),
      );
  }

  fetchGradedTaskBlock(
    courseId: string,
    taskBlockId: string,
    studentId: string,
  ) {
    this.coursesClient
      .getGradedTaskBlock(courseId, taskBlockId, studentId)
      .pipe(untilDestroyed(this))
      .subscribe((g) =>
        this._store.next(
          (_state = {
            ..._state,
            studentGradedBlocks: [
              ..._state.studentGradedBlocks.filter((s) => s.id !== g.id),
              g,
            ],
          }),
        ),
      );
  }
}

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
  CoursesClient,
  CreateCourseCommand,
  CreateTaskBlockCommand,
  DeleteTaskFromBlockCommand,
  InviteStudentsToCourseCommand,
} from '../web-api-client';
import { BehaviorSubject, firstValueFrom, switchMap } from 'rxjs';
import { distinctUntilChanged, filter, map, shareReplay } from 'rxjs/operators';
import { UntilDestroy, untilDestroyed } from '@ngneat/until-destroy';
import { TuiAlertService } from '@taiga-ui/core';

export const coursesPageSize = 20;

export interface CoursesState {
  coursesList: CourseListResponse[];
  currentCourseId: string | null;
  totalPages: number | null;
  currentPage: number | null;
  ascending: boolean;
}

let _state: CoursesState = {
  coursesList: [],
  currentCourseId: null,
  currentPage: null,
  totalPages: null,
  ascending: true,
};

@UntilDestroy()
@Injectable({
  providedIn: 'root',
})
export class CoursesFacadeService {
  coursesClient = inject(CoursesClient);
  private readonly alerts = inject(TuiAlertService);

  private _store: BehaviorSubject<CoursesState> = new BehaviorSubject(_state);

  coursesList$ = this._store.pipe(map((state) => state.coursesList));
  currentCourse$ = this._store.pipe(
    map((state) => state.currentCourseId),
    filter((courseId) => !!courseId),
    switchMap((courseId) => this.coursesClient.getCourseById(courseId)),
    shareReplay(1),
  );
  currentCourseStudents$ = this._store.pipe(
    map((state) => state.currentCourseId),
    filter((courseId) => !!courseId),
    switchMap((courseId) => this.coursesClient.getStudents(courseId)),
    map((r) => r.students),
    shareReplay(1),
  );
  currentCourseGrades$ = this._store.pipe(
    map((state) => state.currentCourseId),
    filter((courseId) => !!courseId),
    switchMap((courseId) => this.coursesClient.getStudentsAndGrades(courseId)),
    shareReplay(1),
  );

  constructor() {
    this._store
      .pipe(
        map((s) => s.ascending),
        distinctUntilChanged(),
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
          }),
        ),
      );
  }

  setCurrentCourseId(courseId: string) {
    this._store.next((_state = { ..._state, currentCourseId: courseId }));
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
    this._store.next(_state);
  }

  async addTasksToTaskBlock(
    courseId: string,
    taskBlockId: string,
    taskIds: string[],
  ): Promise<void> {
    if (taskIds.length === 0) return;
    for (const taskId of taskIds) {
      await firstValueFrom(
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
    this._store.next(_state);
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
    await firstValueFrom(
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
    this._store.next(_state);
  }

  async updateTaskBlockWeights(
    courseId: string,
    taskBlockId: string,
    weights: number[],
  ): Promise<void> {
    await firstValueFrom(
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
    this._store.next(_state);
  }

  setSorting(ascending: boolean) {
    if (ascending === _state.ascending) return;
    this._store.next((_state = { ..._state, ascending }));
  }

  async updateCourseTeacher(
    courseId: string,
    teacherId: string,
  ): Promise<void> {
    await firstValueFrom(
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
    this._store.next(_state);
  }

  async publishTaskBlock(courseId: string, taskBlockId: string) {
    await firstValueFrom(
      this.coursesClient.publishBlock(courseId, taskBlockId),
    );
    this.alerts
      .open('Работа опубликована для студентов', {
        autoClose: 3000,
        appearance: 'positive',
      })
      .subscribe();
    this._store.next(_state);
  }

  async createTaskBlock(courseId: string, name: string) {
    await firstValueFrom(
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
    this._store.next(_state);
  }

  async nextPage() {
    if (_state.currentPage === _state.totalPages) return;
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
  }

  async inviteStudents(courseId: string, emails: string[]) {
    await firstValueFrom(
      this.coursesClient.inviteStudents(
        courseId,
        new InviteStudentsToCourseCommand({
          id: courseId,
          emails,
        }),
      ),
    );
    this._store.next(_state);
  }
}

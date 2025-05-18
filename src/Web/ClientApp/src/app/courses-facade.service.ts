import { inject, Injectable } from '@angular/core';
import {
  AddTaskToBlockCommand,
  ChangeCourseDescriptionCommand,
  ChangeTaskBlockDescriptionCommand,
  ChangeTaskBlockNameCommand,
  ChangeTaskBlockTimelineCommand,
  CourseListResponse,
  CoursesClient,
  CreateCourseCommand,
  DeleteTaskFromBlockCommand,
} from './web-api-client';
import { BehaviorSubject, firstValueFrom, switchMap } from 'rxjs';
import { filter, map, shareReplay } from 'rxjs/operators';
import { UntilDestroy, untilDestroyed } from '@ngneat/until-destroy';
import { TuiAlertService } from '@taiga-ui/core';

export interface CoursesState {
  coursesList: CourseListResponse[];
  currentCourseId: string | null;
}

let _state: CoursesState = {
  coursesList: [],
  currentCourseId: null,
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

  constructor() {
    this.coursesClient
      .getCoursesList(1, 25)
      .pipe(untilDestroyed(this))
      .subscribe((x) =>
        this._store.next(
          (_state = {
            ..._state,
            coursesList: [..._state.coursesList, ...x.items],
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
      .open('Курс успешно создан', {
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
    await Promise.all(
      taskIds.map((taskId) =>
        firstValueFrom(
          this.coursesClient.addTaskToBlock(
            courseId,
            taskBlockId,
            new AddTaskToBlockCommand({
              id: courseId,
              blockId: taskBlockId,
              taskId,
            }),
          ),
        ),
      ),
    );
    this.alerts
      .open('Задачи успешно добавлены', {
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
      .open('Задача успешно удалена', {
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
}

import { inject, Injectable } from '@angular/core';
import {
  CreateTaskCommand,
  TaskListResponse,
  TasksClient,
} from './web-api-client';
import { BehaviorSubject, firstValueFrom } from 'rxjs';
import { map } from 'rxjs/operators';
import { UntilDestroy, untilDestroyed } from '@ngneat/until-destroy';

export interface TasksState {
  taskList: TaskListResponse[];
}

let _state: TasksState = {
  taskList: [],
};

@UntilDestroy()
@Injectable({
  providedIn: 'root',
})
export class TasksFacadeService {
  private readonly tasksClient = inject(TasksClient);

  private _store: BehaviorSubject<TasksState> = new BehaviorSubject(_state);

  taskList$ = this._store.pipe(map((state) => state.taskList));

  constructor() {
    this.tasksClient
      .getTaskList(1, 50)
      .pipe(untilDestroyed(this))
      .subscribe((x) =>
        this._store.next(
          (_state = {
            ..._state,
            taskList: [..._state.taskList, ...x.items],
          }),
        ),
      );
  }

  async createTask(
    name: string,
    text: string,
    isPublic: boolean,
  ): Promise<void> {
    const response = await firstValueFrom(
      this.tasksClient.createTaskRequest(
        new CreateTaskCommand({
          name,
          text,
          isPublic,
        }),
      ),
    );

    this._store.next(
      (_state = {
        ..._state,
        taskList: [
          ..._state.taskList,
          new TaskListResponse({
            ...response,
          }),
        ],
      }),
    );
  }
}

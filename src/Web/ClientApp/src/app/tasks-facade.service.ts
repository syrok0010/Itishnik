import { inject, Injectable } from '@angular/core';
import {
  CreateTaskCommand,
  SetTaskTagsCommand,
  TaskListResponse,
  TaskResponse,
  TasksClient,
} from './web-api-client';
import { BehaviorSubject, firstValueFrom } from 'rxjs';
import { filter, map } from 'rxjs/operators';
import { UntilDestroy, untilDestroyed } from '@ngneat/until-destroy';

export interface TasksState {
  taskList: TaskListResponse[];
  tasks: TaskResponse[];
  currentTaskId: string | null;
}

let _state: TasksState = {
  taskList: [],
  tasks: [],
  currentTaskId: null,
};

@UntilDestroy()
@Injectable({
  providedIn: 'root',
})
export class TasksFacadeService {
  private readonly tasksClient = inject(TasksClient);

  private _store: BehaviorSubject<TasksState> = new BehaviorSubject(_state);

  taskList$ = this._store.pipe(map((state) => state.taskList));
  currentTaskChain$ = this._store.pipe(
    filter((state) => !!state.currentTaskId),
    map((state) => {
      const currentTask = state.tasks.find((t) => t.id === state.currentTaskId);
      if (!currentTask) return [];
      if (!currentTask.firstTaskId) return [currentTask];
      return state.tasks
        .filter(
          (t) =>
            t.id == currentTask.firstTaskId ||
            t.firstTaskId == currentTask.firstTaskId,
        )
        .sort((a, b) => b.created.getTime() - a.created.getTime());
    }),
  );

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
    previousTaskId: string | null = null,
  ): Promise<void> {
    const response = await firstValueFrom(
      this.tasksClient.createTaskRequest(
        new CreateTaskCommand({
          name,
          text,
          isPublic,
          previousTaskId,
        }),
      ),
    );

    this._store.next(
      (_state = {
        ..._state,
        taskList: [
          ..._state.taskList,
          ...response.map(
            (t) =>
              new TaskListResponse({
                ...t,
              }),
          ),
        ],
      }),
    );
  }

  async setCurrentTaskId(taskId: string): Promise<void> {
    const response = await firstValueFrom(
      this.tasksClient.getTaskWithAllVersions(taskId),
    );
    this._store.next(
      (_state = {
        ..._state,
        currentTaskId: taskId,
        tasks: [
          ..._state.tasks.filter(
            (task) => !response.some((r) => r.id === task.id),
          ),
          ...response,
        ],
        taskList: [
          ..._state.taskList.filter(
            (task) => !response.some((r) => r.id === task.id),
          ),
          new TaskListResponse({ ...response[response.length - 1] }),
        ],
      }),
    );
  }

  async updateTags(taskId: string, tagIds: string[]) {
    const response = await firstValueFrom(
      this.tasksClient.setTaskTags(
        taskId,
        new SetTaskTagsCommand({ taskId, tagIds }),
      ),
    );
    this._store.next(
      (_state = {
        ..._state,
        tasks: [
          ..._state.tasks.filter((t) => response.every((rt) => rt.id !== t.id)),
          ...response,
        ],
      }),
    );
  }
}

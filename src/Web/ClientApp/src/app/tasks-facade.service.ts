import { inject, Injectable } from '@angular/core';
import {
  CreateTaskCommand,
  PaginatedListOfTaskListResponse,
  SetTaskTagsCommand,
  SwaggerException,
  TaskListResponse,
  TaskResponse,
  TasksClient,
} from './web-api-client';
import {
  BehaviorSubject,
  firstValueFrom,
  of,
  switchMap,
  throwError,
} from 'rxjs';
import { filter, map, distinctUntilChanged, catchError } from 'rxjs/operators';
import { UntilDestroy, untilDestroyed } from '@ngneat/until-destroy';

export interface FilterState {
  authorIds: string[];
  tagIds: string[];
  name: string;
  sortBy: string;
  ascending: boolean;
}

export interface TasksState {
  taskList: TaskListResponse[];
  tasks: TaskResponse[];
  currentTaskId: string | null;
  filterState: FilterState;
}

let _state: TasksState = {
  taskList: [],
  tasks: [],
  currentTaskId: null,
  filterState: {
    authorIds: [],
    tagIds: [],
    name: '',
    sortBy: '',
    ascending: true,
  },
};

@UntilDestroy()
@Injectable({
  providedIn: 'root',
})
export class TasksFacadeService {
  private readonly tasksClient = inject(TasksClient);

  private _store: BehaviorSubject<TasksState> = new BehaviorSubject(_state);

  taskList$ = this._store.pipe(
    map((state) => state.taskList.sort((a, b) => a.name.localeCompare(b.name))),
  );
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
    this._store
      .pipe(
        map((state) => state.filterState),
        distinctUntilChanged(),
        switchMap((filters) =>
          this.tasksClient.getTaskList(
            filters.tagIds,
            filters.authorIds,
            filters.name,
            filters.sortBy,
            filters.ascending,
            1,
            50,
          ),
        ),
        catchError((err) =>
          SwaggerException.isSwaggerException(err)
            ? of(
                new PaginatedListOfTaskListResponse({
                  items: [],
                  pageNumber: 1,
                  totalPages: 1,
                  totalCount: 0,
                  hasPreviousPage: false,
                  hasNextPage: false,
                }),
              )
            : throwError(() => err),
        ),
        untilDestroyed(this),
      )
      .subscribe((x) =>
        this._store.next(
          (_state = {
            ..._state,
            taskList: [...x?.items],
          }),
        ),
      );
  }

  async createTask(
    name: string,
    text: string,
    solutionText: string,
    isPublic: boolean,
    previousTaskId: string | null = null,
  ): Promise<void> {
    const response = await firstValueFrom(
      this.tasksClient.createTask(
        new CreateTaskCommand({
          name,
          text,
          solutionText,
          isPublic,
          previousTaskId,
        }),
      ),
    );

    this._store.next(
      (_state = {
        ..._state,
        taskList: [
          ..._state.taskList.filter(
            (task) => !response.some((r) => r.id === task.id),
          ),
          ...response.map(
            (t) =>
              new TaskListResponse({
                ...t,
              }),
          ),
        ],
        tasks: [
          ..._state.tasks.filter(
            (task) => !response.some((r) => r.id === task.id),
          ),
          ...response,
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

  async publishTask(id: string) {
    const response = await firstValueFrom(this.tasksClient.publish(id));

    this._store.next(
      (_state = {
        ..._state,
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

  setFilters(filters: FilterState) {
    this._store.next(
      (_state = {
        ..._state,
        filterState: { ..._state.filterState, ...filters },
      }),
    );
  }

  setSorting(sortBy: string, ascending: boolean) {
    if (
      sortBy === _state.filterState.sortBy &&
      ascending === _state.filterState.ascending
    )
      return;

    this._store.next(
      (_state = {
        ..._state,
        filterState: { ..._state.filterState, sortBy, ascending },
      }),
    );
  }
}

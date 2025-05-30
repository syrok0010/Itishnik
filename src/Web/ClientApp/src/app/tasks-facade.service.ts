import { inject, Injectable } from '@angular/core';
import {
  CreateTaskCommand,
  EditReferenceSolutionCommand,
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
import { TuiAlertService } from '@taiga-ui/core';

export const pageSize = 20;

export interface FilterState {
  authorIds: string[];
  tagIds: string[];
  name: string;
  sortBy: string;
  ascending: boolean;
}

export interface TasksState {
  taskList: TaskListResponse[];
  totalPages: number | null;
  currentPage: number | null;
  tasks: TaskResponse[];
  currentTaskId: string | null;
  filterState: FilterState;
}

let _state: TasksState = {
  taskList: [],
  currentPage: null,
  totalPages: null,
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
  private readonly alerts = inject(TuiAlertService);

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
            pageSize,
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
            totalPages: x.totalPages,
            currentPage: x.pageNumber,
            taskList: x?.items,
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
          new TaskListResponse({ ...response[response.length - 1] }),
        ],
        tasks: [
          ..._state.tasks.filter(
            (task) => !response.some((r) => r.id === task.id),
          ),
          ...response,
        ],
      }),
    );

    this.alerts
      .open(
        previousTaskId === null
          ? 'Задача создана'
          : 'Новая версия задачи создана',
        {
          autoClose: 3000,
          appearance: 'positive',
        },
      )
      .subscribe();
  }

  async editReferenceSolution(taskId: string, solutionText: string) {
    const response = await firstValueFrom(
      this.tasksClient.editReferenceSolution(
        taskId,
        new EditReferenceSolutionCommand({
          taskId,
          text: solutionText,
        }),
      ),
    );
    this._store.next(
      (_state = {
        ..._state,
        tasks: [..._state.tasks.filter((t) => t.id !== taskId), response],
      }),
    );

    this.alerts
      .open('Решение изменено', {
        autoClose: 3000,
        appearance: 'positive',
      })
      .subscribe();
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
        taskList: [
          ..._state.taskList.filter((t) =>
            response.every((rt) => rt.id !== t.id),
          ),
          new TaskListResponse({ ...response[response.length - 1] }),
        ],
      }),
    );
    this.alerts
      .open('Теги установлены', {
        autoClose: 3000,
        appearance: 'positive',
      })
      .subscribe();
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

    this.alerts
      .open('Задача опубликована', {
        autoClose: 3000,
        appearance: 'positive',
      })
      .subscribe();
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

  async nextPage() {
    if (_state.currentPage === _state.totalPages) return;
    const response = await firstValueFrom(
      this.tasksClient.getTaskList(
        _state.filterState.authorIds,
        _state.filterState.authorIds,
        _state.filterState.name,
        _state.filterState.sortBy,
        _state.filterState.ascending,
        _state.currentPage + 1,
        pageSize,
      ),
    );
    this._store.next(
      (_state = {
        ..._state,
        taskList: [..._state.taskList, ...response.items],
        currentPage: _state.currentPage + 1,
      }),
    );
  }
}

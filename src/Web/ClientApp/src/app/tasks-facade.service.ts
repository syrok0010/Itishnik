import { Injectable } from '@angular/core';
import { Tag, TaskListResponse } from './web-api-client';
import { BehaviorSubject } from 'rxjs';
import { map } from 'rxjs/operators';

export interface TasksState {
  taskList: TaskListResponse[];
}

const _state: TasksState = {
  taskList: [
    new TaskListResponse({
      id: '',
      name: 'Some course name',
      teacherFullName: 'Бычков Илья Сергеевич',
      teacherEmail: 'ibychkov@hse.ru',
      isPublic: true,
      tags: [
        new Tag({
          id: '',
          text: 'Массивы',
        }),
        new Tag({
          id: '',
          text: 'Стэк',
        }),
        new Tag({
          id: '',
          text: 'BFS',
        }),
      ],
    }),
  ],
};

@Injectable({
  providedIn: 'root',
})
export class TasksFacadeService {
  private _store: BehaviorSubject<TasksState> = new BehaviorSubject(_state);

  taskList$ = this._store.pipe(map((state) => state.taskList));

  createTask(name: string): void {}
}

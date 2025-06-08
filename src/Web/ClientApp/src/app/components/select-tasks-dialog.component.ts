import {
  ChangeDetectionStrategy,
  Component,
  computed,
  viewChild,
} from '@angular/core';
import TasksTableComponent from './tasks-table/tasks-table.component';
import { TuiButton, TuiDialogContext } from '@taiga-ui/core';
import { injectContext } from '@taiga-ui/polymorpheus';
import { map } from 'rxjs/operators';
import { AsyncPipe } from '@angular/common';

@Component({
  selector: 'app-select-tasks-dialog',
  imports: [TasksTableComponent, TuiButton, AsyncPipe],
  template: `
    <app-tasks-table
      #taskTable
      [selectMode]="true"
      [excludeTaskIds]="context.data"
    />
    <div class="mt-4 flex">
      <button
        class="ml-auto"
        tuiButton
        type="button"
        (click)="addTasks()"
        [disabled]="!(anySelectedTasks() | async)"
      >
        Добавить задания
      </button>
    </div>
  `,
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export default class SelectTasksDialogComponent {
  taskTableTemplate = viewChild<TasksTableComponent>('taskTable');
  public readonly context =
    injectContext<TuiDialogContext<string[], string[]>>();

  anySelectedTasks = computed(() =>
    this.taskTableTemplate()
      .selectedArray()
      .valueChanges.pipe(map((a) => a.some((v) => v))),
  );

  addTasks() {
    const table = this.taskTableTemplate();
    const selected = table
      .selectedArray()
      .value.map((e, i) => [e, i])
      .filter(([e]) => e)
      .map(([, i]) => table.filteredTasks()[i as number].id);
    this.context.completeWith(selected);
  }
}

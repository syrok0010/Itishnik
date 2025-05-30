import { ChangeDetectionStrategy, Component, viewChild } from '@angular/core';
import TasksTableComponent from './tasks-table/tasks-table.component';
import { TuiButton, TuiDialogContext } from '@taiga-ui/core';
import { injectContext } from '@taiga-ui/polymorpheus';

@Component({
  selector: 'app-select-tasks-dialog',
  imports: [TasksTableComponent, TuiButton],
  template: `
    <app-tasks-table
      #taskTable
      [selectMode]="true"
      [excludeTaskIds]="context.data"
    />
    <button tuiButton type="button" (click)="addTasks()">
      Добавить задания
    </button>
  `,
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export default class SelectTasksDialogComponent {
  taskTableTemplate = viewChild<TasksTableComponent>('taskTable');
  public readonly context =
    injectContext<TuiDialogContext<string[], string[]>>();

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

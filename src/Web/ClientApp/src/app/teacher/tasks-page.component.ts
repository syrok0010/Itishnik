import { ChangeDetectionStrategy, Component } from '@angular/core';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { TuiInputModule } from '@taiga-ui/legacy';
import TaskTableComponent from '../components/task-table/task-table.component';

@Component({
  selector: 'app-tasks-page',
  imports: [
    FormsModule,
    ReactiveFormsModule,
    TuiInputModule,
    TaskTableComponent,
  ],
  template: `<app-task-table />`,
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export default class TasksPageComponent {}

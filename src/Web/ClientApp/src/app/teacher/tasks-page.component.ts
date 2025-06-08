import { ChangeDetectionStrategy, Component } from '@angular/core';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { TuiInputModule } from '@taiga-ui/legacy';
import TasksTableComponent from '../components/tasks-table/tasks-table.component';
import { TuiButton, TuiIcon } from '@taiga-ui/core';
import { RouterLink } from '@angular/router';

@Component({
  selector: 'app-tasks-page',
  imports: [
    FormsModule,
    ReactiveFormsModule,
    TuiInputModule,
    TasksTableComponent,
    TuiButton,
    TuiIcon,
    RouterLink,
  ],
  styles: [':host { @apply relative flex grow flex-col }'],
  template: ` <app-tasks-table />
    <button
      style="view-transition-name: create-task-block"
      appearance="primary"
      tuiButton
      type="button"
      class="!absolute bottom-4 right-4 shadow-sm"
      [routerLink]="['create']"
    >
      <tui-icon icon="@tui.plus" />
    </button>`,
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export default class TasksPageComponent {}

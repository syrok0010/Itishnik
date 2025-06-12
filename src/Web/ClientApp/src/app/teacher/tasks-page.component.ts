import { ChangeDetectionStrategy, Component } from '@angular/core';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { TuiInputModule } from '@taiga-ui/legacy';
import TasksTableComponent from '../components/tasks-table/tasks-table.component';
import { TuiButton, tuiDialog, TuiIcon } from '@taiga-ui/core';
import { RouterLink } from '@angular/router';
import TagsManagementDialogComponent from '../components/tags-management-dialog.component';

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
  template: `
    <app-tasks-table />

    <div class="!absolute bottom-6 right-6 flex flex-col items-center gap-4">
      <button
        appearance="outline"
        tuiButton
        type="button"
        size="m"
        class="grow-0"
        (click)="openTagsManagementDialog()"
      >
        <tui-icon icon="@tui.tag" />
      </button>
      <button
        style="view-transition-name: create-task-block"
        appearance="primary"
        tuiButton
        type="button"
        [routerLink]="['create']"
      >
        <tui-icon icon="@tui.plus" />
      </button>
    </div>
  `,
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export default class TasksPageComponent {
  private readonly tagsManagementDialog = tuiDialog(
    TagsManagementDialogComponent,
    {
      label: 'Создание тэга',
      size: 'l',
    },
  );

  openTagsManagementDialog() {
    this.tagsManagementDialog().subscribe();
  }
}

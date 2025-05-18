import {
  ChangeDetectionStrategy,
  Component,
  inject,
  input,
  signal,
} from '@angular/core';
import { FilterState, TasksFacadeService } from '../../tasks-facade.service';
import { Role } from '../../users-facade.service';
import {
  FormControl,
  FormGroup,
  FormsModule,
  ReactiveFormsModule,
} from '@angular/forms';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { TuiSortChange, TuiTable } from '@taiga-ui/addon-table';
import { TaskListResponse } from '../../web-api-client';
import { AsyncPipe } from '@angular/common';
import { TuiAvatar, TuiBadge, TuiChip, TuiStatus } from '@taiga-ui/kit';
import { TuiAutoColorPipe, TuiInitialsPipe, TuiTitle } from '@taiga-ui/core';
import { TuiCell } from '@taiga-ui/layout';
import { RouterLink } from '@angular/router';
import TagMultiselectInputComponent from '../tag-multiselect-input.component';
import { TuiInputModule } from '@taiga-ui/legacy';
import UserMultiselectInputComponent from '../user-multiselect-input.component';

@Component({
  selector: 'app-task-table',
  imports: [
    TuiTable,
    AsyncPipe,
    TuiChip,
    TuiAutoColorPipe,
    TuiAvatar,
    TuiInitialsPipe,
    TuiTitle,
    TuiCell,
    FormsModule,
    RouterLink,
    TuiBadge,
    TuiStatus,
    TagMultiselectInputComponent,
    ReactiveFormsModule,
    TuiInputModule,
    UserMultiselectInputComponent,
  ],
  templateUrl: './task-table.component.html',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export default class TaskTableComponent {
  filtering = input(true);

  taskFacade = inject(TasksFacadeService);

  currentSortBy = signal<string | null>(null);
  roles = ['Teacher'] as Role[];
  filterForm = new FormGroup({
    name: new FormControl(''),
    authorIds: new FormControl<string[]>([]),
    tagIds: new FormControl<string[]>([]),
  });

  columns = ['isPublic', 'name', 'author', 'tags'] as readonly string[];

  constructor() {
    this.filterForm.valueChanges
      .pipe(takeUntilDestroyed())
      .subscribe((filters) =>
        this.taskFacade.setFilters(filters as FilterState),
      );
  }

  sortChanged(e: TuiSortChange<Partial<Record<keyof TaskListResponse, any>>>) {
    this.taskFacade.setSorting(e.sortKey, e.sortDirection === 1);
    this.currentSortBy.set(e.sortKey);
  }
}

import {
  ChangeDetectionStrategy,
  Component,
  inject,
  signal,
} from '@angular/core';
import { TuiSortChange, TuiTable } from '@taiga-ui/addon-table';
import { FilterState, TasksFacadeService } from '../../tasks-facade.service';
import { AsyncPipe } from '@angular/common';
import { TuiAvatar, TuiBadge, TuiChip, TuiStatus } from '@taiga-ui/kit';
import { TuiAutoColorPipe, TuiInitialsPipe, TuiTitle } from '@taiga-ui/core';
import { TuiCell } from '@taiga-ui/layout';
import {
  FormControl,
  FormGroup,
  FormsModule,
  ReactiveFormsModule,
} from '@angular/forms';
import { RouterLink } from '@angular/router';
import TagMultiselectInputComponent from '../../components/tag-multiselect-input.component';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { TuiInputModule } from '@taiga-ui/legacy';
import UserMultiselectInputComponent from '../../components/user-multiselect-input.component';
import { Role } from '../../users-facade.service';
import { TaskListResponse } from '../../web-api-client';

@Component({
  selector: 'app-tasks-page',
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
  templateUrl: './tasks-page.component.html',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export default class TasksPageComponent {
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

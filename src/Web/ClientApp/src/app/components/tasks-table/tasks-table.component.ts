import {
  ChangeDetectionStrategy,
  Component,
  computed,
  inject,
  input,
  signal,
} from '@angular/core';
import {
  FilterState,
  pageSize,
  TasksFacadeService,
} from '../../teacher/tasks-facade.service';
import { Role } from '../../users-facade.service';
import {
  FormArray,
  FormControl,
  FormGroup,
  FormsModule,
  ReactiveFormsModule,
} from '@angular/forms';
import { takeUntilDestroyed, toSignal } from '@angular/core/rxjs-interop';
import { TuiSortChange, TuiTable } from '@taiga-ui/addon-table';
import { TaskListResponse } from '../../web-api-client';
import {
  TuiAvatar,
  TuiBadge,
  TuiCheckbox,
  TuiChip,
  TuiStatus,
} from '@taiga-ui/kit';
import {
  TuiAutoColorPipe,
  TuiInitialsPipe,
  TuiScrollable,
  TuiScrollbar,
  TuiTitle,
} from '@taiga-ui/core';
import { TuiCell } from '@taiga-ui/layout';
import { RouterLink } from '@angular/router';
import TagMultiselectInputComponent from '../tag-multiselect-input.component';
import { TuiInputModule } from '@taiga-ui/legacy';
import UserMultiselectInputComponent from '../user-multiselect-input.component';
import {
  CdkFixedSizeVirtualScroll,
  CdkVirtualForOf,
  CdkVirtualScrollViewport,
} from '@angular/cdk/scrolling';
import { tuiPure } from '@taiga-ui/cdk';

@Component({
  selector: 'app-tasks-table',
  imports: [
    TuiTable,
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
    TuiCheckbox,
    TuiScrollbar,
    CdkVirtualScrollViewport,
    CdkFixedSizeVirtualScroll,
    CdkVirtualForOf,
    TuiScrollable,
  ],
  templateUrl: './tasks-table.component.html',
  styles: [':host { @apply flex grow flex-col gap-4}'],
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export default class TasksTableComponent {
  readonly filtering = input(true);
  readonly selectMode = input(false);
  readonly excludeTaskIds = input<string[]>([]);

  taskFacade = inject(TasksFacadeService);
  tasks = toSignal(this.taskFacade.taskList$);
  filteredTasks = computed(() =>
    this.tasks().filter((t) => !this.excludeTaskIds()?.includes(t.id)),
  );

  selectedArray = computed(
    () => new FormArray(this.filteredTasks().map(() => new FormControl(false))),
  );

  currentSortBy = signal<string | null>(null);
  roles = ['Teacher'] as Role[];
  filterForm = new FormGroup({
    name: new FormControl(''),
    authorIds: new FormControl<string[]>([]),
    tagIds: new FormControl<string[]>([]),
  });

  columns = computed(() =>
    this.selectMode()
      ? ([
          'checkBox',
          'isPublic',
          'name',
          'author',
          'tags',
        ] as readonly string[])
      : (['isPublic', 'name', 'author', 'tags'] as readonly string[]),
  );

  columnWidths = computed(() =>
    this.selectMode()
      ? {
          checkBox: 'w-1/12',
          isPublic: 'w-1/12',
          name: 'w-4/12',
          author: 'w-3/12',
          tags: 'w-3/12',
        }
      : {
          isPublic: 'w-1/12',
          name: 'w-4/12',
          author: 'w-3/12',
          tags: 'w-4/12',
        },
  );

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

  async scrollChanged(currentElement: number) {
    const totalLoadedElements = this.filteredTasks().length;
    if (totalLoadedElements - currentElement <= pageSize)
      await this.taskFacade.nextPage();
  }

  @tuiPure
  tableHeightClass() {
    if (this.filteredTasks().length === 0) {
      return 'h-14';
    }
    if (!this.selectMode()) {
      return 'h-[calc(100dvh-60px-56px-5rem)]';
    }
    return 'h-[70dvh]';
  }
}

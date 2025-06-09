import {
  ChangeDetectionStrategy,
  Component,
  computed,
  effect,
  inject,
  input,
  signal,
} from '@angular/core';
import {
  FilterState,
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
  CdkVirtualForOf,
  CdkVirtualScrollViewport,
} from '@angular/cdk/scrolling';
import { startWith } from 'rxjs';
import { CdkAutoSizeVirtualScroll } from '@angular/cdk-experimental/scrolling';
import { GlobalLoadingService } from '../../global-loading.service';

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
    CdkAutoSizeVirtualScroll,
    CdkVirtualForOf,
    TuiScrollable,
  ],
  templateUrl: './tasks-table.component.html',
  styles: [
    `
      :host {
        @apply flex grow flex-col;
      }
      :host ::ng-deep tui-scrollbar .t-content {
        height: auto !important;
      }
    `,
  ],
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export default class TasksTableComponent {
  readonly filtering = input(true);
  readonly selectMode = input(false);
  readonly excludeTaskIds = input<string[]>([]);

  taskFacade = inject(TasksFacadeService);
  globalLoading = inject(GlobalLoadingService);
  tasks = toSignal(this.taskFacade.taskList$);
  filteredTasks = computed(() =>
    this.excludeTaskIds() !== null && this.excludeTaskIds().length > 0
      ? this.tasks().filter((t) => !this.excludeTaskIds().includes(t.id))
      : this.tasks(),
  );
  isLoading = toSignal(this.taskFacade.isLoading$);
  isAtBottom = signal(false);
  initialLoadDone = signal(false);

  shouldShowLoader = computed(
    () => this.isLoading() && (!this.tasks() || this.isAtBottom()),
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
      .pipe(
        startWith({ name: '', authorIds: [], tagIds: [] }),
        takeUntilDestroyed(),
      )
      .subscribe((filters) =>
        this.taskFacade.setFilters(filters as FilterState),
      );
    effect(() => {
      this.globalLoading.setManualLoading(this.shouldShowLoader());
    });
    effect(() => {
      if (this.tasks() !== undefined) {
        this.initialLoadDone.set(true);
      }
    });
  }

  sortChanged(e: TuiSortChange<Partial<Record<keyof TaskListResponse, any>>>) {
    this.taskFacade.setSorting(e.sortKey, e.sortDirection === 1);
    this.currentSortBy.set(e.sortKey);
  }

  onViewportScroll(viewport: CdkVirtualScrollViewport) {
    const end = viewport.getRenderedRange().end;
    const total = viewport.getDataLength();

    const prefetchThreshold = 10;
    if (total > 0 && end >= total - prefetchThreshold)
      this.taskFacade.nextPage();

    this.isAtBottom.set(total > 0 && end === total);
  }
}

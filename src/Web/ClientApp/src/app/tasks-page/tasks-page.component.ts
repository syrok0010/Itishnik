import { ChangeDetectionStrategy, Component, inject } from '@angular/core';
import { TuiTable } from '@taiga-ui/addon-table';
import { TasksFacadeService } from '../tasks-facade.service';
import { AsyncPipe } from '@angular/common';
import { TuiAvatar, TuiCheckbox, TuiChip } from '@taiga-ui/kit';
import { TuiAutoColorPipe, TuiInitialsPipe, TuiTitle } from '@taiga-ui/core';
import { TuiCell } from '@taiga-ui/layout';
import { FormsModule } from '@angular/forms';

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
    TuiCheckbox,
    FormsModule,
  ],
  templateUrl: './tasks-page.component.html',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export default class TasksPageComponent {
  taskFacade = inject(TasksFacadeService);

  columns = ['isPublic', 'title', 'author', 'tags'] as readonly string[];
}

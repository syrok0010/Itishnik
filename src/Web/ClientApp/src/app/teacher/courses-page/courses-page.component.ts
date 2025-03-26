import { ChangeDetectionStrategy, Component } from '@angular/core';
import { TuiTable } from '@taiga-ui/addon-table';

@Component({
  selector: 'app-courses-page',
  imports: [TuiTable],
  templateUrl: './courses-page.component.html',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class CoursesPageComponent {}

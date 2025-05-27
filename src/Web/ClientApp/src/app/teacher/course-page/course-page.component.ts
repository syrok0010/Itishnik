import {
  ChangeDetectionStrategy,
  Component,
  computed,
  effect,
  inject,
  OnInit,
} from '@angular/core';
import { CoursesFacadeService } from '../../courses-facade.service';
import {
  ActivatedRoute,
  RouterLink,
  RouterLinkActive,
  RouterOutlet,
} from '@angular/router';
import {
  TuiAutoColorPipe,
  TuiButton,
  TuiInitialsPipe,
  TuiTextfield,
  TuiTitle,
} from '@taiga-ui/core';
import { TuiAvatar, TuiTabs, TuiTextarea } from '@taiga-ui/kit';
import { FormControl, ReactiveFormsModule } from '@angular/forms';
import { toSignal } from '@angular/core/rxjs-interop';
import { TuiSortChange } from '@taiga-ui/addon-table';
import { TaskListResponse } from '../../web-api-client';

@Component({
  selector: 'app-course-page',
  imports: [
    TuiTextfield,
    TuiTextarea,
    TuiButton,
    ReactiveFormsModule,
    TuiTabs,
    RouterLink,
    RouterOutlet,
    RouterLinkActive,
    TuiAutoColorPipe,
    TuiInitialsPipe,
    TuiAvatar,
    TuiTitle,
  ],
  templateUrl: './course-page.component.html',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export default class CoursePageComponent implements OnInit {
  coursesFacade = inject(CoursesFacadeService);
  route = inject(ActivatedRoute);
  currentCourse = toSignal(this.coursesFacade.currentCourse$);

  description = computed(() => this.currentCourse()?.description);
  descriptionControl = new FormControl<string | null>('');

  constructor() {
    effect(() => {
      if (!this.currentCourse()) return;
      this.descriptionControl.setValue(this.currentCourse().description, {
        emitEvent: false,
      });
    });
  }

  ngOnInit(): void {
    this.coursesFacade.setCurrentCourseId(this.route.snapshot.params['id']);
  }

  async saveDescription() {
    await this.coursesFacade.updateDescription(
      this.currentCourse().id,
      this.descriptionControl.value,
    );
    this.descriptionControl.markAsPristine();
  }
}

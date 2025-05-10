import {
  ChangeDetectionStrategy,
  Component,
  computed,
  effect,
  inject,
  OnInit,
  Signal,
} from '@angular/core';
import { CoursesFacadeService } from '../../courses-facade.service';
import {
  ActivatedRoute,
  RouterLink,
  RouterLinkActive,
  RouterOutlet,
} from '@angular/router';
import { TuiButton, TuiTextfield } from '@taiga-ui/core';
import { TuiTabs, TuiTextarea } from '@taiga-ui/kit';
import {
  AbstractControl,
  FormControl,
  ReactiveFormsModule,
  ValidationErrors,
  ValidatorFn,
} from '@angular/forms';
import { toSignal } from '@angular/core/rxjs-interop';

export function textDifferentFromLatest(
  latestTextSignal: Signal<string>,
): ValidatorFn {
  return (control: AbstractControl): ValidationErrors | null => {
    const newText = control.value as string | null;
    const latestText = latestTextSignal();

    if (
      latestText === null ||
      latestText === undefined ||
      newText === null ||
      newText === undefined ||
      newText.trim() === latestText.trim()
    ) {
      return { textNotChanged: true }; //сохранено
    }

    return null; //сохранить
  };
}

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
  ],
  templateUrl: './course-page.component.html',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export default class CoursePageComponent implements OnInit {
  coursesFacade = inject(CoursesFacadeService);
  route = inject(ActivatedRoute);
  currentCourse = toSignal(this.coursesFacade.currentCourse$);

  description = computed(() => this.currentCourse()?.description);
  descriptionControl = new FormControl<string | null>('', [
    textDifferentFromLatest(this.description),
  ]);

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

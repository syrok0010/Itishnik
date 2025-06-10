import { ChangeDetectionStrategy, Component, inject } from '@angular/core';
import {
  FormControl,
  FormsModule,
  ReactiveFormsModule,
  Validators,
} from '@angular/forms';
import { TuiInputModule } from '@taiga-ui/legacy';
import { TuiButton, TuiDialogContext, TuiError } from '@taiga-ui/core';
import { injectContext } from '@taiga-ui/polymorpheus';
import { CoursesFacadeService } from '../teacher/courses-facade.service';
import { TuiFieldErrorPipe } from '@taiga-ui/kit';
import { AsyncPipe } from '@angular/common';

@Component({
  selector: 'app-create-course-dialog',
  imports: [
    ReactiveFormsModule,
    TuiInputModule,
    TuiButton,
    FormsModule,
    TuiError,
    TuiFieldErrorPipe,
    AsyncPipe,
  ],
  template: `
    <form class="flex flex-col items-end gap-y-4 pt-4" (ngSubmit)="create()">
      <div class="w-full">
        <tui-input [formControl]="courseName" class="w-full">
          Название курса (неизменяемое)
        </tui-input>
        <tui-error
          [formControl]="courseName"
          [error]="[] | tuiFieldError | async"
        />
      </div>
      <div class="flex flex-row gap-x-4">
        <button appearance="outline" tuiButton type="button" (click)="cancel()">
          Отменить
        </button>
        <button
          appearance="primary"
          tuiButton
          type="submit"
          [disabled]="courseName.invalid"
        >
          Создать
        </button>
      </div>
    </form>
  `,
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export default class CreateCourseDialogComponent {
  courseName: FormControl = new FormControl('', Validators.required);
  public readonly context = injectContext<TuiDialogContext>();
  courseFacade = inject(CoursesFacadeService);

  cancel() {
    this.context.completeWith();
  }

  async create() {
    await this.courseFacade.createCourse(this.courseName.value);
    this.context.completeWith();
  }
}

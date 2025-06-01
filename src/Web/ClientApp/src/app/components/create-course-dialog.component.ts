import { ChangeDetectionStrategy, Component, inject } from '@angular/core';
import {
  FormControl,
  FormsModule,
  ReactiveFormsModule,
  Validators,
} from '@angular/forms';
import { TuiInputModule } from '@taiga-ui/legacy';
import { TuiButton, TuiDialogContext } from '@taiga-ui/core';
import { injectContext } from '@taiga-ui/polymorpheus';
import { CoursesFacadeService } from '../teacher/courses-facade.service';

@Component({
  selector: 'app-create-course-dialog',
  imports: [ReactiveFormsModule, TuiInputModule, TuiButton, FormsModule],
  template: `
    <form class="flex flex-col items-end gap-y-4 pt-4" (ngSubmit)="create()">
      <tui-input [formControl]="courseName" class="w-full">
        Название курса
      </tui-input>
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
export class CreateCourseDialogComponent {
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

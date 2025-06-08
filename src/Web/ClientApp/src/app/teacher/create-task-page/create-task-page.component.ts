import {
  ChangeDetectionStrategy,
  Component,
  inject,
  Injector,
} from '@angular/core';
import {
  FormControl,
  FormGroup,
  ReactiveFormsModule,
  Validators,
} from '@angular/forms';
import { TuiInputModule, TuiTextfieldControllerModule } from '@taiga-ui/legacy';
import {
  TUI_EDITOR_DEFAULT_EXTENSIONS,
  TUI_EDITOR_EXTENSIONS,
  TuiEditor,
} from '@taiga-ui/editor';
import { TuiButton, TuiError } from '@taiga-ui/core';
import {
  TuiCheckbox,
  TuiFieldErrorPipe,
  tuiValidationErrorsProvider,
} from '@taiga-ui/kit';
import { TasksFacadeService } from '../tasks-facade.service';
import { Router } from '@angular/router';
import { AsyncPipe } from '@angular/common';

@Component({
  selector: 'app-create-task-page',
  imports: [
    TuiInputModule,
    ReactiveFormsModule,
    TuiEditor,
    TuiCheckbox,
    TuiButton,
    TuiTextfieldControllerModule,
    TuiError,
    TuiFieldErrorPipe,
    AsyncPipe,
  ],
  templateUrl: './create-task-page.component.html',
  changeDetection: ChangeDetectionStrategy.OnPush,
  providers: [
    {
      provide: TUI_EDITOR_EXTENSIONS,
      deps: [Injector],
      useFactory: (injector: Injector) => [
        ...TUI_EDITOR_DEFAULT_EXTENSIONS,
        import('@taiga-ui/editor').then(({ setup }) => setup({ injector })),
      ],
    },
    tuiValidationErrorsProvider({
      required: 'Поле не может быть пустым',
    }),
  ],
})
export default class CreateTaskPageComponent {
  private readonly taskFacade = inject(TasksFacadeService);
  private readonly router = inject(Router);

  form = new FormGroup({
    name: new FormControl('', [Validators.required]),
    text: new FormControl('', [Validators.required]),
    solutionText: new FormControl('', [Validators.required]),
    isPublic: new FormControl(false),
  });

  async createTask() {
    const id = await this.taskFacade.createTask(
      this.form.value.name,
      this.form.value.text,
      this.form.value.solutionText,
      this.form.value.isPublic,
      null,
    );
    await this.router.navigate(['/tasks', id]);
  }
}

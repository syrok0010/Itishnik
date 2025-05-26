import {
  ChangeDetectionStrategy,
  Component,
  inject,
  Injector,
} from '@angular/core';
import { Location } from '@angular/common';
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
import { TuiButton } from '@taiga-ui/core';
import { TuiCheckbox } from '@taiga-ui/kit';
import { TasksFacadeService } from '../../tasks-facade.service';

@Component({
  selector: 'app-create-task-page',
  imports: [
    TuiInputModule,
    ReactiveFormsModule,
    TuiEditor,
    TuiCheckbox,
    TuiButton,
    TuiTextfieldControllerModule,
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
  ],
})
export default class CreateTaskPageComponent {
  private readonly taskFacade = inject(TasksFacadeService);
  private readonly location = inject(Location);

  form = new FormGroup({
    name: new FormControl('', [Validators.required]),
    text: new FormControl('', [Validators.required]),
    solutionText: new FormControl('', [Validators.required]),
    isPublic: new FormControl(true),
  });

  async createTask() {
    await this.taskFacade.createTask(
      this.form.value.name,
      this.form.value.text,
      this.form.value.solutionText,
      this.form.value.isPublic,
      null,
    );
    this.location.back();
  }
}

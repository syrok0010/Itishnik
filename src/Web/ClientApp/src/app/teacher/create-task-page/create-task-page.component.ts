import {
  ChangeDetectionStrategy,
  Component,
  inject,
  Injector,
  viewChild,
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
import {
  TuiAlertService,
  TuiButton,
  tuiDialog,
  TuiError,
} from '@taiga-ui/core';
import {
  TuiCheckbox,
  TuiFieldErrorPipe,
  tuiValidationErrorsProvider,
} from '@taiga-ui/kit';
import { TasksFacadeService } from '../tasks-facade.service';
import { Router } from '@angular/router';
import { AsyncPipe } from '@angular/common';
import GenerateTaskDialogComponent from '../../components/generate-task-dialog.component';
import { firstValueFrom } from 'rxjs';
import { GlobalLoadingService } from '../../global-loading.service';

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
        import('@taiga-ui/editor').then(({ TuiMarkdown }) =>
          TuiMarkdown.configure({
            html: true,
            linkify: true,
            transformPastedText: true,
          }),
        ),
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
  private readonly alerts = inject(TuiAlertService);
  private readonly globalLoadingService = inject(GlobalLoadingService);
  private readonly generateTaskDialog = tuiDialog(GenerateTaskDialogComponent, {
    dismissible: true,
    closeable: true,
  });
  private readonly textEditorRef = viewChild<TuiEditor>('textEditor');
  private readonly solutionEditorRef = viewChild<TuiEditor>('solutionEditor');

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

  async showGenerateDialog() {
    const command = await firstValueFrom(this.generateTaskDialog(undefined));

    try {
      this.globalLoadingService.setManualLoading(true, 'Происходит магия...');
      const generated = await this.taskFacade.generate(command);

      this.form.reset({
        name: '',
        text: '',
        solutionText: '',
        isPublic: false,
      });
      this.textEditorRef()
        .editorService.getOriginTiptapEditor()
        .chain()
        .focus()
        .insertContent(generated.text)
        .run();
      this.solutionEditorRef()
        .editorService.getOriginTiptapEditor()
        .chain()
        .focus()
        .insertContent(generated.solution)
        .run();

      this.form.controls.name.setValue(generated.name);
    } catch {
      this.alerts
        .open(
          `
            <p>Ой, что-то пошло не так...</p>
            <p>Повторите попытку еще раз</p>
        `,
          { autoClose: 3000, appearance: 'negative' },
        )
        .subscribe();
    } finally {
      this.globalLoadingService.setManualLoading(false);
    }
  }
}

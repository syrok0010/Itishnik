import {
  ChangeDetectionStrategy,
  Component,
  computed,
  effect,
  inject,
  Injector,
  Signal,
  signal,
  TemplateRef,
  viewChild,
} from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { TasksFacadeService } from '../tasks-facade.service';
import {
  TuiAutoColorPipe,
  TuiButton,
  TuiDialogService,
  TuiError,
  TuiInitialsPipe,
  TuiTitle,
} from '@taiga-ui/core';
import { toSignal } from '@angular/core/rxjs-interop';
import {
  TUI_CONFIRM,
  TuiAvatar,
  TuiBadge,
  TuiChip,
  TuiConfirmData,
  TuiFieldErrorPipe,
  TuiStatus,
  tuiValidationErrorsProvider,
} from '@taiga-ui/kit';
import { TuiAccordion } from '@taiga-ui/experimental';
import { TuiMultiSelectModule } from '@taiga-ui/legacy';
import {
  AbstractControl,
  FormControl,
  FormGroup,
  ReactiveFormsModule,
  ValidationErrors,
  ValidatorFn,
  Validators,
} from '@angular/forms';
import TagMultiselectInputComponent from '../../components/tag-multiselect-input.component';
import { AsyncPipe } from '@angular/common';
import {
  TUI_EDITOR_DEFAULT_EXTENSIONS,
  TUI_EDITOR_EXTENSIONS,
  TuiEditor,
  TuiEditorSocket,
} from '@taiga-ui/editor';
import { firstValueFrom } from 'rxjs';
import { UsersFacadeService } from '../../users-facade.service';

export function textDifferentFromLatest(
  latestTextSignal: Signal<string | undefined>,
): ValidatorFn {
  return (control: AbstractControl): ValidationErrors | null => {
    const newText = control.value as string | null;
    const latestText = latestTextSignal();

    if (
      latestText === undefined ||
      newText === null ||
      newText.trim() !== latestText.trim()
    ) {
      return null;
    }

    return { textNotChanged: true };
  };
}

@Component({
  selector: 'app-task-page',
  imports: [
    TuiButton,
    TuiAutoColorPipe,
    TuiAvatar,
    TuiInitialsPipe,
    TuiTitle,
    TuiChip,
    TuiBadge,
    TuiStatus,
    TuiMultiSelectModule,
    ReactiveFormsModule,
    TagMultiselectInputComponent,
    TuiError,
    TuiAccordion,
    TuiFieldErrorPipe,
    AsyncPipe,
    TuiEditorSocket,
    TuiEditor,
  ],
  templateUrl: './task-page.component.html',
  changeDetection: ChangeDetectionStrategy.OnPush,
  providers: [
    tuiValidationErrorsProvider({
      required: 'Условие задачи не может быть пустым',
      textNotChanged:
        'Новый текст задачи должен отличаться от предыдущей версии',
    }),
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
export default class TaskPageComponent {
  editTagsDialogTemplate = viewChild<TemplateRef<any>>('editTagsDialog');
  createVersionDialogTemplate = viewChild<TemplateRef<any>>(
    'createVersionDialog',
  );

  private readonly router = inject(Router);
  private readonly route = inject(ActivatedRoute);
  private readonly taskFacade = inject(TasksFacadeService);
  private readonly userFacade = inject(UsersFacadeService);
  private readonly dialogs = inject(TuiDialogService);
  id: string = this.route.snapshot.paramMap.get('id');

  authInfo = toSignal(this.userFacade.authInfo$);
  tagControl = new FormControl<string[]>([]);
  solutionControl = new FormControl('');

  currentTaskChain = toSignal(this.taskFacade.currentTaskChain$);
  editingSolution = signal(false);

  currentVersion = computed(() =>
    !this.currentTaskChain()
      ? null
      : this.currentTaskChain().find((t) => t.id === this.id),
  );

  isAuthor = computed(
    () => this.currentVersion().teacherId === this.authInfo().userId,
  );

  latestVersion = computed(() => {
    const chain = this.currentTaskChain();
    return !chain || chain.length === 0 ? null : chain[0];
  });

  private latestVersionText = computed(() => this.latestVersion()?.text);

  newVersionForm = new FormGroup({
    text: new FormControl<string>('', [
      Validators.required,
      textDifferentFromLatest(this.latestVersionText),
    ]),
    solutionText: new FormControl<string>('', Validators.required),
  });

  private initializeTagControl = effect(() => {
    if (!this.currentVersion()) return;
    this.tagControl.setValue(
      this.currentVersion().tags.map((t) => t.id),
      { emitEvent: false },
    );
    this.tagControl.markAsPristine();
  });

  private initializeNewVersionForm = effect(() => {
    if (!this.latestVersion()) return;
    this.solutionControl.setValue(this.latestVersion().rightSolutionText);
    this.newVersionForm.setValue(
      {
        text: this.latestVersion().text,
        solutionText: this.latestVersion().rightSolutionText,
      },
      { emitEvent: false },
    );
    this.newVersionForm.markAsPristine();
  });

  protected showEditTagsDialog(): void {
    this.dialogs
      .open(this.editTagsDialogTemplate(), { label: 'Редактировать теги' })
      .subscribe();
  }

  async updateTaskTags() {
    await this.taskFacade.updateTags(
      this.currentVersion().id,
      this.tagControl.value,
    );
  }

  async saveNewVersion() {
    const latest = this.latestVersion();

    if (this.newVersionForm.invalid) {
      this.newVersionForm.markAllAsTouched();
      return;
    }

    const id = await this.taskFacade.createTask(
      latest.name,
      this.newVersionForm.value.text,
      this.newVersionForm.value.solutionText,
      latest.isPublic,
      latest.id,
    );
    await this.router.navigate(['/tasks', id]);
  }

  async publish() {
    const data: TuiConfirmData = {
      content:
        'Задача станет доступна для просмотра и использования остальным преподавателям. Вы не сможете отменить это действие.',
      yes: 'Опубликовать',
      no: 'Отменить',
    };

    const shouldPublishTask = await firstValueFrom(
      this.dialogs.open<boolean>(TUI_CONFIRM, {
        label: 'Опубликовать задачу?',
        size: 'm',
        data,
      }),
    );

    if (!shouldPublishTask) return;

    await this.taskFacade.publishTask(this.currentVersion().id);
  }

  async saveSolution() {
    this.editingSolution.set(false);
    await this.taskFacade.editReferenceSolution(
      this.latestVersion().id,
      this.solutionControl.value,
    );
  }

  async openCreatingVersionDialog() {
    const save = await firstValueFrom(
      this.dialogs.open<boolean>(this.createVersionDialogTemplate(), {
        size: 'auto',
        label: 'Новая версия',
      }),
    );
    if (save) await this.saveNewVersion();
  }
}

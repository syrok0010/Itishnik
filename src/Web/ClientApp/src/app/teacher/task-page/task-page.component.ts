import {
  ChangeDetectionStrategy,
  Component,
  computed,
  effect,
  inject,
  signal,
  TemplateRef,
  viewChild,
} from '@angular/core';
import { ActivatedRoute, RouterLink } from '@angular/router';
import { TasksFacadeService } from '../../tasks-facade.service';
import {
  TuiAutoColorPipe,
  TuiButton,
  TuiDialogService,
  TuiError,
  TuiInitialsPipe,
  TuiTextfield,
  TuiTitle,
} from '@taiga-ui/core';
import { toSignal } from '@angular/core/rxjs-interop';
import {
  TuiAvatar,
  TuiBadge,
  TuiChip,
  TuiFieldErrorPipe,
  TuiStatus,
  TuiTextarea,
  tuiValidationErrorsProvider,
} from '@taiga-ui/kit';
import { TuiAccordion } from '@taiga-ui/experimental';
import { TuiMultiSelectModule } from '@taiga-ui/legacy';
import {
  FormControl,
  FormGroup,
  ReactiveFormsModule,
  Validators,
} from '@angular/forms';
import TagMultiselectInputComponent from '../../components/tag-multiselect-input.component';
import { AsyncPipe } from '@angular/common';

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
    TuiTextarea,
    TuiTextfield,
    TuiFieldErrorPipe,
    AsyncPipe,
    RouterLink,
  ],
  templateUrl: './task-page.component.html',
  changeDetection: ChangeDetectionStrategy.OnPush,
  providers: [
    tuiValidationErrorsProvider({
      required: 'Условие задачи не может быть пустым',
    }),
  ],
})
export default class TaskPageComponent {
  editTagsDialogTemplate = viewChild<TemplateRef<any>>('editTagsDialog');
  private readonly route = inject(ActivatedRoute);
  private readonly taskFacade = inject(TasksFacadeService);
  private readonly dialogs = inject(TuiDialogService);
  id: string = this.route.snapshot.paramMap.get('id');

  tagControl = new FormControl<string[]>([]);

  newVersionForm = new FormGroup({
    text: new FormControl<string>('', [Validators.required]),
    solutionId: new FormControl<string | null>(null),
  });

  currentTaskChain = toSignal(this.taskFacade.currentTaskChain$);
  creatingNewVersion = signal(false);

  currentVersion = computed(() =>
    !this.currentTaskChain()
      ? null
      : this.currentTaskChain().find((t) => t.id === this.id),
  );

  latestVersion = computed(() => {
    const chain = this.currentTaskChain();
    return !chain || chain.length === 0 ? null : chain[0];
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
    this.newVersionForm.setValue(
      {
        text: this.latestVersion().text,
        solutionId: null,
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

    if (
      !this.newVersionForm.controls.text.dirty ||
      this.newVersionForm.invalid
    ) {
      return;
    }

    await this.taskFacade.createTask(
      latest.name,
      this.newVersionForm.value.text,
      latest.isPublic,
      latest.id,
    );
    this.newVersionForm.reset();
  }

  publish() {
    throw new Error('Method not implemented.');
  }
}

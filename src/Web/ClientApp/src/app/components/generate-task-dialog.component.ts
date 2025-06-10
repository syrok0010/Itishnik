import { ChangeDetectionStrategy, Component, inject } from '@angular/core';
import {
  TuiAutoColorPipe,
  TuiButton,
  TuiDialogContext,
  TuiError,
  TuiTextfield,
} from '@taiga-ui/core';
import { CommonModule } from '@angular/common';
import { injectContext } from '@taiga-ui/polymorpheus';
import { GenerateTaskCommand } from '../web-api-client';
import { TasksFacadeService } from '../teacher/tasks-facade.service';
import {
  FormControl,
  FormGroup,
  ReactiveFormsModule,
  Validators,
} from '@angular/forms';
import {
  TuiChevron,
  TuiChip,
  TuiComboBox,
  TuiFieldErrorPipe,
  TuiFilterByInputPipe,
  TuiSelect,
  tuiValidationErrorsProvider,
} from '@taiga-ui/kit';
import { TuiMultiSelectModule } from '@taiga-ui/legacy';
import { TagsFacadeService } from '../teacher/tags-facade.service';
import { toSignal } from '@angular/core/rxjs-interop';
import { map } from 'rxjs/operators';

@Component({
  selector: 'app-generate-task-dialog',
  standalone: true,
  providers: [
    tuiValidationErrorsProvider({
      required: 'Заполните поле',
    }),
  ],
  imports: [
    CommonModule,
    TuiButton,
    ReactiveFormsModule,
    TuiMultiSelectModule,
    TuiTextfield,
    TuiChevron,
    TuiChip,
    TuiAutoColorPipe,
    TuiComboBox,
    TuiFilterByInputPipe,
    TuiSelect,
    TuiError,
    TuiFieldErrorPipe,
  ],
  template: `
    <form class="flex flex-col gap-4" [formGroup]="form">
      <h2 class="flex items-center gap-2 text-xl font-semibold">
        <span>🪄</span>
        Настройки генерации
      </h2>
      <tui-textfield tuiChevron>
        <label tuiLabel>Тема</label>
        <input tuiComboBox formControlName="topic" />
        <tui-data-list-wrapper
          *tuiTextfieldDropdown
          new
          [items]="allTags() | tuiFilterByInput"
          [itemContent]="tagContent"
        />
        <tui-error
          formControlName="topic"
          [error]="[] | tuiFieldError | async"
        />
      </tui-textfield>

      <tui-textfield tuiChevron>
        <label tuiLabel>Сложность</label>
        <input tuiSelect formControlName="difficulty" />
        <tui-data-list-wrapper
          *tuiTextfieldDropdown
          new
          [items]="difficulties"
        />
        <tui-error
          formControlName="difficulty"
          [error]="[] | tuiFieldError | async"
        />
      </tui-textfield>

      <tui-textfield tuiChevron>
        <label tuiLabel>Тип</label>
        <input tuiComboBox [strict]="false" formControlName="type" />
        <tui-data-list-wrapper *tuiTextfieldDropdown new [items]="types" />
        <tui-error
          formControlName="type"
          [error]="[] | tuiFieldError | async"
        />
      </tui-textfield>

      <tui-textfield>
        <label tuiLabel>Тематика</label>
        <input
          tuiTextfield
          placeholder="Вселенная Гарри Поттера"
          formControlName="theme"
        />
      </tui-textfield>

      <tui-textfield>
        <label tuiLabel>Суть задания</label>
        <input tuiTextfield formControlName="idea" />
      </tui-textfield>

      <p class="font-base text-center italic">
        ИИ <span class="font-bold">заменит</span> название, текст задания и
        решение.<br />
        Восстановить текущие значения будет
        <span class="font-bold">невозможно</span>.
      </p>

      <div class="flex justify-end gap-4">
        <button
          tuiButton
          size="l"
          appearance="flat"
          (click)="context.completeWith(null)"
        >
          Отмена
        </button>
        <button
          class="gemini-generate-button"
          (click)="generate()"
          [disabled]="form.invalid"
        >
          <span class="icon">✨</span>
          <span class="text">Сгенерировать</span>
        </button>
      </div>
    </form>

    <ng-template #tagContent let-tag>
      <tui-chip
        size="m"
        appearance="custom"
        [style.background-color]="tag | tuiAutoColor"
      >
        {{ tag }}
      </tui-chip>
    </ng-template>
  `,
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export default class GenerateTaskDialogComponent {
  private readonly taskFacade = inject(TasksFacadeService);
  private readonly tagFacade = inject(TagsFacadeService);

  readonly difficulties = ['Легкая', 'Средняя', 'Сложная'] as const;
  readonly types = [
    'Поиск ошибки',
    'Анализ решения',
    'Результат выполнения',
    'Рефакторинг',
  ] as const;

  allTags = toSignal(
    this.tagFacade.allTags$.pipe(map((tags) => tags.map((t) => t.text))),
  );
  public readonly context =
    injectContext<TuiDialogContext<GenerateTaskCommand | null>>();

  form = new FormGroup({
    topic: new FormControl<string>(null, Validators.required),
    difficulty: new FormControl<string>('', Validators.required),
    type: new FormControl<string>('', Validators.required),
    theme: new FormControl(''),
    idea: new FormControl(''),
  });

  generate(): void {
    this.context.completeWith(
      new GenerateTaskCommand({
        idea: this.form.value.idea,
        theme: this.form.value.theme,
        topic: this.form.value.topic,
        difficulty: this.form.value.difficulty,
        taskType: this.form.value.type,
      }),
    );
  }
}

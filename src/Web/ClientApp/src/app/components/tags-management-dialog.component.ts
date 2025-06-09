import {
  ChangeDetectionStrategy,
  Component,
  computed,
  inject,
} from '@angular/core';
import { TagsFacadeService } from '../teacher/tags-facade.service';
import {
  TuiAutoColorPipe,
  TuiButton,
  TuiError,
  TuiTextfield,
} from '@taiga-ui/core';
import {
  TuiChip,
  TuiFieldErrorPipe,
  tuiValidationErrorsProvider,
} from '@taiga-ui/kit';
import {
  AbstractControl,
  FormControl,
  FormsModule,
  ReactiveFormsModule,
  ValidationErrors,
  ValidatorFn,
  Validators,
} from '@angular/forms';
import { toSignal } from '@angular/core/rxjs-interop';
import { AsyncPipe } from '@angular/common';
import { TuiInputModule, TuiTextfieldControllerModule } from '@taiga-ui/legacy';

@Component({
  selector: 'app-tags-management-dialog',
  imports: [
    TuiAutoColorPipe,
    TuiChip,
    TuiTextfield,
    ReactiveFormsModule,
    TuiButton,
    FormsModule,
    TuiError,
    TuiFieldErrorPipe,
    AsyncPipe,
    TuiInputModule,
    TuiTextfieldControllerModule,
  ],
  providers: [
    tuiValidationErrorsProvider({
      required: 'Заполните поле',
      tagExists: 'Такой тэг уже существует',
    }),
  ],
  template: `
    <div class="mt-6 flex flex-col gap-4">
      <tui-input
        [formControl]="search"
        tuiTextfieldIcon="@tui.search"
        tuiTextfieldSize="m"
      >
        Поиск
      </tui-input>
      <div class="flex grow flex-wrap gap-2">
        @for (tag of filteredTasks(); track tag.id) {
          <tui-chip
            size="m"
            appearance="custom"
            [style.background-color]="tag.text | tuiAutoColor"
          >
            {{ tag.text }}
          </tui-chip>
        } @empty {
          <span class="w-full text-center text-xl italic text-gray-600">
            {{
              allTags().length === 0 ? 'Теги не созданы' : 'Теги не найдены'
            }}</span
          >
        }
      </div>

      <form class="mt-4 flex w-full gap-4" (ngSubmit)="addTag()">
        <tui-textfield class="grow">
          <label tuiLabel>Текст тега</label>
          <input tuiTextfield [formControl]="tagControl" />
          <tui-error
            [formControl]="tagControl"
            [error]="[] | tuiFieldError | async"
          />
        </tui-textfield>
        <button tuiButton type="submit" [disabled]="tagControl.invalid">
          Создать
        </button>
      </form>
    </div>
  `,
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export default class TagsManagementDialogComponent {
  private readonly tagsFacade = inject(TagsFacadeService);
  allTags = toSignal(this.tagsFacade.allTags$);

  tagControl = new FormControl('', [
    Validators.required,
    this.existingTagValidator(),
  ]);
  search = new FormControl('');
  searchText = toSignal(this.search.valueChanges);

  filteredTasks = computed(() => {
    const text = this.searchText();
    const allTags = this.allTags();

    return !text
      ? allTags
      : allTags.filter((t) =>
          t.text.toLowerCase().includes(text.toLowerCase()),
        );
  });

  async addTag() {
    await this.tagsFacade.createTag(this.tagControl.value);
    this.tagControl.reset();
    this.tagControl.markAsPristine();
  }

  existingTagValidator(): ValidatorFn {
    return (control: AbstractControl): ValidationErrors | null =>
      this.allTags()
        .map((t) => t.text.toLowerCase())
        .includes(control.value.toLowerCase())
        ? { tagExists: true }
        : null;
  }
}

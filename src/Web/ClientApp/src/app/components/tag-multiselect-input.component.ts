import { ChangeDetectionStrategy, Component, inject } from '@angular/core';
import { TuiDataListComponent, TuiOption } from '@taiga-ui/core';
import {
  TuiMultiSelectModule,
  TuiTextfieldControllerModule,
} from '@taiga-ui/legacy';
import {
  ControlValueAccessor,
  FormControl,
  NG_VALUE_ACCESSOR,
  ReactiveFormsModule,
} from '@angular/forms';
import { toSignal } from '@angular/core/rxjs-interop';
import { TagsFacadeService } from '../tags-facade.service';
import { UntilDestroy, untilDestroyed } from '@ngneat/until-destroy';

@UntilDestroy()
@Component({
  selector: 'app-tag-multiselect-input',
  providers: [
    {
      provide: NG_VALUE_ACCESSOR,
      useExisting: TagMultiselectInputComponent,
      multi: true,
    },
  ],
  imports: [
    TuiDataListComponent,
    TuiMultiSelectModule,
    TuiOption,
    ReactiveFormsModule,
    TuiTextfieldControllerModule,
  ],
  template: `
    <tui-multi-select
      [tuiTextfieldLabelOutside]="true"
      [autoColor]="true"
      [formControl]="tagControl"
      [stringify]="stringifyTag"
    >
      Теги задачи
      <tui-data-list *tuiDataList tuiMultiSelectGroup>
        @for (tag of allTags(); track tag.id) {
          <button tuiOption type="button" [value]="tag.id">
            {{ tag.text }}
          </button>
        }
      </tui-data-list>
    </tui-multi-select>
  `,
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export default class TagMultiselectInputComponent
  implements ControlValueAccessor
{
  private readonly tagFacade = inject(TagsFacadeService);
  tagControl = new FormControl<string[]>([]);

  allTags = toSignal(this.tagFacade.allTags$);

  public onTouched: () => void = () => {};

  writeValue(val: string[]): void {
    val && this.tagControl.patchValue(val, { emitEvent: false });
  }

  registerOnChange(fn: any): void {
    this.tagControl.valueChanges
      .pipe(untilDestroyed(this))
      .subscribe(() => fn(this.tagControl.getRawValue()));
  }

  registerOnTouched(fn: any): void {
    this.onTouched = fn;
  }

  setDisabledState?(isDisabled: boolean): void {
    isDisabled ? this.tagControl.disable() : this.tagControl.enable();
  }

  stringifyTag = (id: string) =>
    this.allTags().find((t) => t.id === id)?.text ?? 'Тег не найден';
}

import { ChangeDetectionStrategy, Component, inject } from '@angular/core';
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
import { TagsFacadeService } from '../teacher/tags-facade.service';
import { UntilDestroy, untilDestroyed } from '@ngneat/until-destroy';
import { tuiItemsHandlersProvider } from '@taiga-ui/kit';
import { TuiContext } from '@taiga-ui/cdk';
import { Tag } from '../web-api-client';

@UntilDestroy()
@Component({
  selector: 'app-tag-multiselect-input',
  providers: [
    {
      provide: NG_VALUE_ACCESSOR,
      useExisting: TagMultiselectInputComponent,
      multi: true,
    },
    tuiItemsHandlersProvider({
      identityMatcher: (item1: Tag, item2: Tag) => item1.id === item2.id,
      stringify: (item: Tag | TuiContext<Tag>) =>
        '$implicit' in item ? item.$implicit.text : item.text,
    }),
  ],
  imports: [
    TuiMultiSelectModule,
    ReactiveFormsModule,
    TuiTextfieldControllerModule,
  ],
  template: `
    <tui-multi-select
      [tuiTextfieldLabelOutside]="true"
      [autoColor]="true"
      [formControl]="tagControl"
    >
      Теги задачи
      <tui-data-list-wrapper
        *tuiDataList
        [items]="allTags() | tuiHideSelected"
      />
    </tui-multi-select>
  `,
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export default class TagMultiselectInputComponent
  implements ControlValueAccessor
{
  private readonly tagFacade = inject(TagsFacadeService);
  tagControl = new FormControl<Tag[]>([]);

  allTags = toSignal(this.tagFacade.allTags$);

  public onTouched: () => void = () => {};

  writeValue(val: string[]): void {
    val &&
      this.tagControl.patchValue(
        this.allTags().filter((t) => val.includes(t.id)),
        { emitEvent: false },
      );
  }

  registerOnChange(fn: any): void {
    this.tagControl.valueChanges
      .pipe(untilDestroyed(this))
      .subscribe(() => fn(this.tagControl.getRawValue().map((t) => t.id)));
  }

  registerOnTouched(fn: any): void {
    this.onTouched = fn;
  }

  setDisabledState?(isDisabled: boolean): void {
    isDisabled ? this.tagControl.disable() : this.tagControl.enable();
  }
}

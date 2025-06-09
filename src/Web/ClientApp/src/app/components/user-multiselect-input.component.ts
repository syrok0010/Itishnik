import {
  ChangeDetectionStrategy,
  Component,
  inject,
  input,
  OnInit,
  Signal,
} from '@angular/core';
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
import { UntilDestroy, untilDestroyed } from '@ngneat/until-destroy';
import { tuiItemsHandlersProvider } from '@taiga-ui/kit';
import { TuiContext } from '@taiga-ui/cdk';
import { UserDto } from '../web-api-client';
import { getFullName, Role, UsersFacadeService } from '../users-facade.service';

@UntilDestroy()
@Component({
  selector: 'app-user-multiselect-input',
  providers: [
    {
      provide: NG_VALUE_ACCESSOR,
      useExisting: UserMultiselectInputComponent,
      multi: true,
    },
    tuiItemsHandlersProvider({
      identityMatcher: (item1: UserDto, item2: UserDto) =>
        item1.id === item2.id,
      stringify: (item: UserDto | TuiContext<UserDto>) =>
        getFullName('$implicit' in item ? item.$implicit : item),
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
      [tuiTextfieldSize]=""
      [formControl]="usersControl"
    >
      {{ placeholder() }}
      <tui-data-list-wrapper
        *tuiDataList
        [items]="allUsers() | tuiHideSelected"
      />
    </tui-multi-select>
  `,

  changeDetection: ChangeDetectionStrategy.OnPush,
})
export default class UserMultiselectInputComponent
  implements ControlValueAccessor, OnInit
{
  roles = input.required<Role[]>();
  placeholder = input.required<string>();
  private readonly usersFacade = inject(UsersFacadeService);
  usersControl = new FormControl<UserDto[]>([]);

  allUsers: Signal<UserDto[]> = toSignal(this.usersFacade.selectedUsers$);

  ngOnInit() {
    this.usersFacade.selectRoles(this.roles());
  }

  public onTouched: () => void = () => {};

  writeValue(val: string[]): void {
    val &&
      this.allUsers() &&
      this.usersControl.patchValue(
        this.allUsers().filter((t) => val.includes(t.id)),
        { emitEvent: false },
      );
  }

  registerOnChange(fn: any): void {
    this.usersControl.valueChanges
      .pipe(untilDestroyed(this))
      .subscribe(() => fn(this.usersControl.getRawValue().map((t) => t.id)));
  }

  registerOnTouched(fn: any): void {
    this.onTouched = fn;
  }

  setDisabledState?(isDisabled: boolean): void {
    isDisabled ? this.usersControl.disable() : this.usersControl.enable();
  }
}

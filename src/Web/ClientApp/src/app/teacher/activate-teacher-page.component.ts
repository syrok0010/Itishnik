import {
  ChangeDetectionStrategy,
  Component,
  inject,
  OnInit,
} from '@angular/core';
import {
  FormControl,
  FormGroup,
  ReactiveFormsModule,
  Validators,
} from '@angular/forms';
import { TuiInputModule, TuiInputYearModule } from '@taiga-ui/legacy';
import { TuiButton, TuiError, TuiTextfield } from '@taiga-ui/core';
import { TuiFieldErrorPipe } from '@taiga-ui/kit';
import { UsersFacadeService } from '../users-facade.service';
import { Router } from '@angular/router';
import { ActivateTeacherCommand } from '../web-api-client';
import { firstValueFrom } from 'rxjs';
import { AsyncPipe } from '@angular/common';

@Component({
  selector: 'app-activate-student-page',
  imports: [
    ReactiveFormsModule,
    TuiInputModule,
    TuiTextfield,
    TuiInputYearModule,
    TuiButton,
    TuiError,
    TuiFieldErrorPipe,
    AsyncPipe,
  ],
  template: `
    <form
      [formGroup]="form"
      (ngSubmit)="save()"
      class="flex w-full flex-col gap-4 rounded-lg bg-white p-4 sm:w-2/3 md:w-1/2"
    >
      <h1 class="text-2xl font-bold">Заполните данные</h1>

      <tui-textfield
        ><label tuiLabel>Фамилия</label>
        <input tuiTextfield formControlName="surname" />
        <tui-error
          formControlName="surname"
          [error]="[] | tuiFieldError | async"
        />
      </tui-textfield>
      <tui-textfield
        ><label tuiLabel>Имя</label>
        <input tuiTextfield formControlName="name" />
        <tui-error
          formControlName="name"
          [error]="[] | tuiFieldError | async"
        />
      </tui-textfield>
      <tui-textfield
        ><label tuiLabel>Отчество</label>
        <input tuiTextfield formControlName="patronymic" />
        <tui-error
          formControlName="patronymic"
          [error]="[] | tuiFieldError | async"
        />
      </tui-textfield>

      <button tuiButton type="submit" size="m" [disabled]="form.invalid">
        Сохранить
      </button>
    </form>
  `,
  styles: `
    :host {
      @apply items-center justify-center;
    }
  `,
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export default class ActivateTeacherPageComponent implements OnInit {
  private readonly usersFacade = inject(UsersFacadeService);
  private readonly router = inject(Router);

  form = new FormGroup({
    name: new FormControl('', [Validators.required]),
    surname: new FormControl('', [Validators.required]),
    patronymic: new FormControl(''),
  });

  async ngOnInit() {
    const userInfo = await firstValueFrom(this.usersFacade.currentUser$);
    if (!userInfo.surname.toLowerCase().includes('не установлено'))
      await this.router.navigate(['/']);
  }

  async save() {
    await this.usersFacade.activateTeacher(
      new ActivateTeacherCommand({
        ...this.form.value,
      }),
    );
    await this.router.navigate(['/']);
  }
}

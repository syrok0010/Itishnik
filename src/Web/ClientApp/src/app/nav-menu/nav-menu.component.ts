import { Component, computed, inject, signal } from '@angular/core';
import { RouterLink, RouterLinkActive } from '@angular/router';
import {
  TuiAlertService,
  TuiAutoColorPipe,
  TuiButton,
  TuiError,
  TuiIcon,
  TuiInitialsPipe,
  TuiPopup,
  TuiTextfield,
  TuiTitle,
} from '@taiga-ui/core';
import { UsersFacadeService } from '../users-facade.service';
import { map } from 'rxjs/operators';
import { AsyncPipe, NgOptimizedImage } from '@angular/common';
import { FullNamePipe, HasFio } from '../components/full-name-pipe.pipe';
import {
  TuiAvatar,
  TuiDrawer,
  TuiFieldErrorPipe,
  TuiPassword,
  tuiValidationErrorsProvider,
} from '@taiga-ui/kit';
import { toSignal } from '@angular/core/rxjs-interop';
import {
  AbstractControl,
  FormControl,
  FormGroup,
  FormsModule,
  ReactiveFormsModule,
  ValidationErrors,
  ValidatorFn,
  Validators,
} from '@angular/forms';
import { UsersClient } from '../web-api-client';
import { firstValueFrom } from 'rxjs';

@Component({
  selector: 'app-nav-menu',
  templateUrl: './nav-menu.component.html',
  standalone: true,
  imports: [
    RouterLink,
    RouterLinkActive,
    TuiIcon,
    AsyncPipe,
    NgOptimizedImage,
    FullNamePipe,
    TuiDrawer,
    TuiInitialsPipe,
    TuiAutoColorPipe,
    TuiAvatar,
    TuiTitle,
    TuiPopup,
    TuiButton,
    FormsModule,
    TuiTextfield,
    ReactiveFormsModule,
    TuiPassword,
    TuiError,
    TuiFieldErrorPipe,
  ],
  providers: [tuiValidationErrorsProvider({ noMatch: 'Пароли не совпадают' })],
})
export class NavMenuComponent {
  readonly teacherLinks: [string, string][] = [
    ['Курсы', '/courses'],
    ['Задания', '/tasks'],
  ] as const;

  readonly studentLinks: [string, string][] = [
    ['Курсы', '/courses'],
    ['Оценки', '/grades'],
  ] as const;

  readonly adminLinks: [string, string][] = [
    ...this.teacherLinks,
    ['Преподаватели', '/teachers'],
  ] as const;

  private readonly usersFacade = inject(UsersFacadeService);
  private readonly usersClient = inject(UsersClient);
  private readonly alerts = inject(TuiAlertService);

  links$ = this.usersFacade.authInfo$.pipe(
    map((authInfo) =>
      authInfo.roles.includes('Administrator')
        ? this.adminLinks
        : authInfo.roles.includes('Teacher')
          ? this.teacherLinks
          : this.studentLinks,
    ),
  );

  shortenedFullName$ = this.usersFacade.currentUser$.pipe(
    map((user) => {
      if (!user) return '';

      const result = `${user.surname} ${user.name[0].toUpperCase()}.`;
      return !!user.patronymic
        ? result + user.patronymic[0].toUpperCase() + '.'
        : result;
    }),
  );

  user = toSignal(this.usersFacade.currentUser$);
  role = toSignal(this.usersFacade.authInfo$.pipe(map((i) => i.roles[0])));
  fio = computed(() => this.user() as HasFio);
  open = signal(false);

  passwordForm = new FormGroup(
    {
      oldPassword: new FormControl('', Validators.required),
      newPassword: new FormControl('', [
        Validators.required,
        Validators.minLength(8),
      ]),
      confirmPassword: new FormControl('', Validators.required),
    },
    this.confirmPasswordValidator(),
  );

  async changePassword() {
    await firstValueFrom(
      this.usersClient.changePassword(
        this.passwordForm.value.oldPassword,
        this.passwordForm.value.newPassword,
      ),
    );
    this.alerts
      .open('Пароль изменен', { appearance: 'positive', autoClose: 3000 })
      .subscribe();
    this.passwordForm.reset();
  }

  confirmPasswordValidator(): ValidatorFn {
    return (form: AbstractControl): ValidationErrors | null => {
      const password = form.get('newPassword');
      const confirmPassword = form.get('confirmPassword');
      if (password.value === confirmPassword.value) {
        confirmPassword.setErrors(null);
      } else {
        confirmPassword.setErrors({ noMatch: true });
      }
      return null;
    };
  }

  logout() {
    window.location.href = '/Identity/Account/Logout';
  }
}

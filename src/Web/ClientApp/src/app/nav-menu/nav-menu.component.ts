import { Component, computed, inject, signal } from '@angular/core';
import { RouterLink, RouterLinkActive } from '@angular/router';
import {
  TuiAutoColorPipe,
  TuiButton,
  TuiIcon,
  TuiInitialsPipe,
  TuiPopup,
  TuiTitle,
} from '@taiga-ui/core';
import { UsersFacadeService } from '../users-facade.service';
import { map } from 'rxjs/operators';
import { AsyncPipe, NgOptimizedImage } from '@angular/common';
import { FullNamePipe, HasFio } from '../components/full-name-pipe.pipe';
import { TuiAvatar, TuiDrawer } from '@taiga-ui/kit';
import { toSignal } from '@angular/core/rxjs-interop';
import { FormsModule } from '@angular/forms';

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
  ],
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

  usersFacade = inject(UsersFacadeService);
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
}

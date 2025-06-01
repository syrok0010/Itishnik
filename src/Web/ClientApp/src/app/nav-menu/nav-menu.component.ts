import { Component, computed, inject } from '@angular/core';
import { RouterLink, RouterLinkActive } from '@angular/router';
import { TuiIcon } from '@taiga-ui/core';
import { UsersFacadeService } from '../users-facade.service';
import { map } from 'rxjs/operators';
import { AsyncPipe } from '@angular/common';
import { toSignal } from '@angular/core/rxjs-interop';

@Component({
  selector: 'app-nav-menu',
  templateUrl: './nav-menu.component.html',
  standalone: true,
  imports: [RouterLink, RouterLinkActive, TuiIcon, AsyncPipe],
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

  usersFacade = inject(UsersFacadeService);
  roles = toSignal(
    this.usersFacade.authInfo$.pipe(map((authInfo) => authInfo.roles)),
  );
  links = computed(() =>
    this.roles().includes('Student') ? this.studentLinks : this.teacherLinks,
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
}

import { Component, inject } from '@angular/core';
import { RouterLink, RouterLinkActive } from '@angular/router';
import { TuiIcon } from '@taiga-ui/core';
import { UsersFacadeService } from '../users-facade.service';
import { map } from 'rxjs/operators';
import { AsyncPipe } from '@angular/common';

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
  links$ = this.usersFacade.authInfo$.pipe(
    map((authInfo) =>
      authInfo.roles.includes('Student')
        ? this.studentLinks
        : this.teacherLinks,
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
}

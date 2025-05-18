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
  readonly links: [string, string][] = [
    ['Курсы', '/courses'],
    ['Задания', '/tasks'],
    ['Студенты', '/students'],
  ] as const;

  usersFacade = inject(UsersFacadeService);

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

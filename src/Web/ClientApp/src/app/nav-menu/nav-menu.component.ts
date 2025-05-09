import { Component, computed, inject } from '@angular/core';
import { RouterLink, RouterLinkActive } from '@angular/router';
import { TuiIcon } from '@taiga-ui/core';
import { AuthClient } from '../web-api-client';
import { toSignal } from '@angular/core/rxjs-interop';

@Component({
  selector: 'app-nav-menu',
  templateUrl: './nav-menu.component.html',
  standalone: true,
  imports: [RouterLink, RouterLinkActive, TuiIcon],
})
export class NavMenuComponent {
  readonly links: [string, string][] = [
    ['Курсы', '/courses'],
    ['Задания', '/tasks'],
    ['Студенты', '/students'],
  ] as const;

  authClient = inject(AuthClient);
  userInfo = toSignal(this.authClient.userInfo());
  shortenedFullName = computed(() => {
    const user = this.userInfo();
    let result = `${user.surname} ${user.name[0].toUpperCase()}.`;
    if (!!user.patronymic) result += user.patronymic[0].toUpperCase() + '.';
    return result;
  });
}

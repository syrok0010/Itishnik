import { Component } from '@angular/core';
import { RouterLink, RouterLinkActive } from '@angular/router';
import { TuiIcon } from '@taiga-ui/core';

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
}

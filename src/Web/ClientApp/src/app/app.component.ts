import { Component } from '@angular/core';
import { NavMenuComponent } from './nav-menu/nav-menu.component';
import { RouterOutlet } from '@angular/router';
import { TuiRoot } from '@taiga-ui/core';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  standalone: true,
  imports: [NavMenuComponent, RouterOutlet, TuiRoot],
})
export class AppComponent {
  title = 'app';
}

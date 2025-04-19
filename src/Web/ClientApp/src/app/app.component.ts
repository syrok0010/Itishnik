import { Component, inject } from '@angular/core';
import { NavMenuComponent } from './nav-menu/nav-menu.component';
import { RouterOutlet } from '@angular/router';
import { TuiLoader, TuiRoot } from '@taiga-ui/core';
import { GlobalLoadingService } from './global-loading.service';
import { AsyncPipe } from '@angular/common';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  standalone: true,
  imports: [NavMenuComponent, RouterOutlet, TuiRoot, TuiLoader, AsyncPipe],
})
export class AppComponent {
  title = 'app';
  loadingService = inject(GlobalLoadingService);
  showLoader$ = this.loadingService.showLoader$;
}

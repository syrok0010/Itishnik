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
import { TuiButton, TuiNumberFormat, TuiTextfield } from '@taiga-ui/core';
import { TuiInputNumber } from '@taiga-ui/kit';
import { UsersFacadeService } from '../users-facade.service';
import { Router } from '@angular/router';
import { ActivateStudentCommand } from '../web-api-client';
import { toSignal } from '@angular/core/rxjs-interop';

@Component({
  selector: 'app-activate-student-page',
  imports: [
    ReactiveFormsModule,
    TuiInputModule,
    TuiTextfield,
    TuiInputNumber,
    TuiInputYearModule,
    TuiButton,
    TuiNumberFormat,
  ],
  templateUrl: './activate-student-page.component.html',
  styles: `
    :host {
      @apply items-center justify-center;
    }
  `,
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export default class ActivateStudentPageComponent implements OnInit {
  private readonly usersFacade = inject(UsersFacadeService);
  private readonly router = inject(Router);

  private readonly currentUser = toSignal(this.usersFacade.currentUser$);

  form = new FormGroup({
    name: new FormControl('', [Validators.required]),
    surname: new FormControl('', [Validators.required]),
    patronymic: new FormControl(''),
    educationalProgram: new FormControl('', [Validators.required]),
    groupNumber: new FormControl<number>(null, [Validators.required]),
    educationStartYear: new FormControl<number>(null, [Validators.required]),
  });

  async ngOnInit() {
    const userInfo = this.currentUser();
    if (
      userInfo.groupNumber === undefined ||
      userInfo.groupNumber === null ||
      userInfo.groupNumber !== 100
    ) {
      await this.router.navigate(['/']);
    }
  }

  async save() {
    await this.usersFacade.activateUser(
      new ActivateStudentCommand({
        ...this.form.value,
        studentId: this.currentUser().id,
      }),
    );
    await this.router.navigate(['/']);
  }
}

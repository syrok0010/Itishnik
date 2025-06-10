import {
  ChangeDetectionStrategy,
  Component,
  computed,
  input,
  output,
} from '@angular/core';
import {
  FormArray,
  FormControl,
  FormsModule,
  ReactiveFormsModule,
  Validators,
} from '@angular/forms';
import { TuiInputModule, TuiTextfieldControllerModule } from '@taiga-ui/legacy';
import {
  TuiAutoColorPipe,
  TuiButton,
  TuiIcon,
  TuiInitialsPipe,
  TuiTextfield,
  TuiTitle,
} from '@taiga-ui/core';
import { startWith } from 'rxjs';
import { TuiAvatar } from '@taiga-ui/kit';
import { toSignal } from '@angular/core/rxjs-interop';

export interface User {
  id: string;
  email: string;
  fullName: string;
}

@Component({
  selector: 'app-users',
  imports: [
    ReactiveFormsModule,
    TuiTextfieldControllerModule,
    TuiInputModule,
    TuiButton,
    TuiTextfield,
    FormsModule,
    TuiAutoColorPipe,
    TuiAvatar,
    TuiInitialsPipe,
    TuiTitle,
    TuiIcon,
  ],
  templateUrl: './users.component.html',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export default class UsersComponent {
  emailsArray = new FormArray<FormControl<string>>([this.getEmailControl()]);
  search = new FormControl('');
  readonly existingUsers = input<User[]>();
  readonly headers = input.required<{ invite: string; list: string }>();

  readonly addUsers = output<string[]>();

  private readonly searchSignal = toSignal(
    this.search.valueChanges.pipe(startWith('')),
    { initialValue: '' },
  );
  readonly filteredStudents = computed(() => {
    const students = this.existingUsers();
    const searchTerm = this.searchSignal().toLowerCase();

    if (!searchTerm) {
      return students;
    }

    return students.filter(
      (s) =>
        s.fullName.toLowerCase().includes(searchTerm) ||
        s.email.toLowerCase().includes(searchTerm),
    );
  });
  getEmailControl() {
    return new FormControl('', [Validators.required, Validators.email]);
  }

  addControl() {
    this.emailsArray.push(this.getEmailControl());

    setTimeout(() => {
      window.scrollTo({
        top: document.documentElement.scrollHeight,
        behavior: 'smooth',
      });
    }, 0);
  }

  async invite() {
    this.addUsers.emit(this.emailsArray.value);
    this.emailsArray.reset();
  }
}

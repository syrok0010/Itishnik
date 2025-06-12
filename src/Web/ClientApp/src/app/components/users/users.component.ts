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
  TuiHint,
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

  group: string | null;
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
    TuiHint,
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
  readonly filteredUsers = computed(() => {
    let students = this.existingUsers();
    const searchTerm = this.searchSignal().toLowerCase();

    if (searchTerm) {
      students = students.filter(
        (s) =>
          s.fullName.toLowerCase().includes(searchTerm) ||
          s.email.toLowerCase().includes(searchTerm),
      );
    }

    return [
      ...students
        .filter((s) => !s.fullName.includes('Не установлено'))
        .sort((a, b) => a.fullName.localeCompare(b.fullName)),
      ...students
        .filter((s) => s.fullName.includes('Не установлено'))
        .sort((a, b) => a.email.localeCompare(b.email)),
    ];
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

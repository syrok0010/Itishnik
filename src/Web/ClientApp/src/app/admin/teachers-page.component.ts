import {
  ChangeDetectionStrategy,
  Component,
  inject,
  OnInit,
} from '@angular/core';
import UsersComponent, { User } from '../components/users/users.component';
import { UsersFacadeService } from '../users-facade.service';
import { FullNamePipe } from '../components/full-name-pipe.pipe';
import { map } from 'rxjs/operators';
import { AsyncPipe } from '@angular/common';

@Component({
  selector: 'app-teachers-page',
  imports: [UsersComponent, AsyncPipe],
  template: `
    <div class="flex h-full w-full flex-col gap-4 rounded-lg bg-white p-4">
      <app-users
        [existingUsers]="teachers$ | async"
        (addUsers)="inviteTeachers($event)"
        [headers]="{
          invite: 'Пригласить преподавателей',
          list: 'Список преподавателей',
        }"
      />
    </div>
  `,
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export default class TeachersPageComponent implements OnInit {
  private readonly usersFacade = inject(UsersFacadeService);
  private readonly fullNamePipe = new FullNamePipe();
  teachers$ = this.usersFacade.selectedUsers$.pipe(
    map((users) =>
      users.map<User>(
        (u) =>
          ({
            id: u.id,
            email: u.email,
            fullName: this.fullNamePipe.transform({
              surname: u.surname,
              name: u.name,
              patronymic: u.patronymic,
            }),
          }) as User,
      ),
    ),
  );

  ngOnInit() {
    this.usersFacade.selectRoles(['Teacher']);
  }

  async inviteTeachers(emails: string[]) {
    await this.usersFacade.inviteTeachers(emails);
  }
}

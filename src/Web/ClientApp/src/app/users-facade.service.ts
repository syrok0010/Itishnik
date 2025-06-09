import { inject, Injectable } from '@angular/core';
import {
  ActivateStudentCommand,
  AuthState,
  InviteTeachersCommand,
  UserDto,
  UsersClient,
} from './web-api-client';
import { filter, shareReplay, switchMap } from 'rxjs/operators';
import {
  BehaviorSubject,
  firstValueFrom,
  Observable,
  startWith,
  Subject,
} from 'rxjs';

export type Role = 'Administrator' | 'Teacher' | 'Student';

export function getFullName(user: UserDto) {
  const result = user.surname + ' ' + user.name[0].toUpperCase() + '.';
  return user.patronymic
    ? result + ' ' + user.patronymic[0].toUpperCase() + '.'
    : result;
}

@Injectable({
  providedIn: 'root',
})
export class UsersFacadeService {
  private readonly usersClient = inject(UsersClient);
  private readonly refetchSubject = new Subject<void>();

  authInfo$: Observable<AuthState> = this.refetchSubject.pipe(
    startWith(undefined),
    switchMap(() => this.usersClient.authInfo()),
    shareReplay(1),
  );

  currentUser$ = this.authInfo$.pipe(
    switchMap((_) => this.usersClient.userInfo()),
    shareReplay(1),
  );

  selectedRoles = new BehaviorSubject<Role[]>([]);
  selectedUsers$ = this.selectedRoles.pipe(
    filter((roles) => roles.length > 0),
    switchMap((roles) => this.usersClient.getUsers(roles)),
    shareReplay(1),
  );

  selectRoles(roles: Role[]) {
    this.selectedRoles.next(roles);
  }

  async activateUser(activationInfo: ActivateStudentCommand) {
    await firstValueFrom(this.usersClient.activateStudent(activationInfo));
    this.refetchSubject.next();
  }

  async inviteTeachers(emails: string[]) {
    await firstValueFrom(
      this.usersClient.inviteTeachers(new InviteTeachersCommand({ emails })),
    );
    this.selectedRoles.next(this.selectedRoles.value);
  }
}

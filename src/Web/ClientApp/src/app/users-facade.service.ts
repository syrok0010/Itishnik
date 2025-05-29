import { inject, Injectable } from '@angular/core';
import {
  ActivateStudentCommand,
  AuthState,
  UserDto,
  UsersClient,
} from './web-api-client';
import { shareReplay, switchMap } from 'rxjs/operators';
import { firstValueFrom, Observable, startWith, Subject } from 'rxjs';

export type Role = 'Teacher' | 'Student';

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

  selectedRoles = new Subject<Role[]>();
  selectedUsers = this.selectedRoles.pipe(
    switchMap((roles) => this.usersClient.getUsers(roles)),
  );

  selectRoles(roles: Role[]) {
    this.selectedRoles.next(roles);
  }

  async activateUser(activationInfo: ActivateStudentCommand) {
    await firstValueFrom(this.usersClient.activateStudent(activationInfo));
    this.refetchSubject.next();
  }
}

import { inject, Injectable } from '@angular/core';
import { AuthState, UserDto, UsersClient } from './web-api-client';
import { shareReplay, switchMap, tap } from 'rxjs/operators';
import { Observable, Subject, timer } from 'rxjs';

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

  authInfo$: Observable<AuthState> = this.usersClient
    .authInfo()
    .pipe(shareReplay(1));

  selectedRoles = new Subject<Role[]>();
  selectedUsers = this.selectedRoles.pipe(
    switchMap((roles) => this.usersClient.getUsers(roles)),
  );

  selectRoles(roles: Role[]) {
    this.selectedRoles.next(roles);
  }
}

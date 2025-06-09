import { ChangeDetectionStrategy, Component, inject } from '@angular/core';
import UsersComponent, {
  User,
} from '../../components/course-students/users.component';
import { firstValueFrom } from 'rxjs';
import { CoursesFacadeService } from '../courses-facade.service';
import { AsyncPipe } from '@angular/common';
import { map } from 'rxjs/operators';

@Component({
  selector: 'app-students-tab',
  imports: [UsersComponent, AsyncPipe],
  template: `
    <app-users
      [existingUsers]="students$ | async"
      (addUsers)="inviteStudents($event)"
      [headers]="{
        invite: 'Пригласить студентов',
        list: 'Студенты на курсе',
      }"
    />
  `,
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export default class StudentsTabComponent {
  protected readonly coursesFacade = inject(CoursesFacadeService);
  students$ = this.coursesFacade.currentCourseStudents$.pipe(
    map((s) => s as User[]),
  );

  async inviteStudents(emails: string[]) {
    const course = await firstValueFrom(this.coursesFacade.currentCourse$);
    await this.coursesFacade.inviteStudents(course.id, emails);
  }
}

import { ChangeDetectionStrategy, Component, inject } from '@angular/core';
import { AsyncPipe } from '@angular/common';
import StudentCourseCardComponent from '../components/student-course-card/student-course-card.component';
import { StudentCoursesFacadeService } from './student-courses-facade.service';
import { RouterLink } from '@angular/router';

@Component({
  selector: 'app-student-courses-page',
  imports: [AsyncPipe, StudentCourseCardComponent, RouterLink],
  template: `
    <div class="my-4 grid grid-cols-3 gap-x-12 gap-y-8">
      @for (course of courses$ | async; track course.id) {
        <app-student-course-card
          [course]="course"
          class="hover:cursor-pointer"
          [routerLink]="[course.id]"
        />
      }
    </div>
  `,
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export default class StudentCoursesPageComponent {
  private readonly studentFacade = inject(StudentCoursesFacadeService);
  courses$ = this.studentFacade.coursesList$;
}

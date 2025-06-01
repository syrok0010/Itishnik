import { ChangeDetectionStrategy, Component, inject } from '@angular/core';
import { StudentsClient } from '../../web-api-client';
import { map } from 'rxjs';
import { AsyncPipe } from '@angular/common';
import StudentCourseCardComponent from '../../components/student-course-card/student-course-card.component';

@Component({
  selector: 'app-student-courses-page',
  imports: [AsyncPipe, StudentCourseCardComponent],
  templateUrl: './student-courses-page.component.html',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export default class StudentCoursesPageComponent {
  private readonly studentFacade = inject(StudentsClient);
  courses = this.studentFacade.getCourses(1, 25).pipe(map((c) => c.items));
}

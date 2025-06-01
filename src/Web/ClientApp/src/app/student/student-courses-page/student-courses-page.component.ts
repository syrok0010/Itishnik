import { ChangeDetectionStrategy, Component, inject } from '@angular/core';
import { AsyncPipe } from '@angular/common';
import StudentCourseCardComponent from '../../components/student-course-card/student-course-card.component';
import { StudentCoursesFacadeService } from '../student-courses-facade.service';

@Component({
  selector: 'app-student-courses-page',
  imports: [AsyncPipe, StudentCourseCardComponent],
  templateUrl: './student-courses-page.component.html',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export default class StudentCoursesPageComponent {
  private readonly studentFacade = inject(StudentCoursesFacadeService);
  courses$ = this.studentFacade.coursesList$;
}

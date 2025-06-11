import { ChangeDetectionStrategy, Component, inject } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { TuiBadge } from '@taiga-ui/kit';
import { toSignal } from '@angular/core/rxjs-interop';
import { StudentCoursesFacadeService } from '../student-courses-facade.service';
import { GradedTaskBlockResponse } from '../../web-api-client';
import { DecimalPipe } from '@angular/common';

@Component({
  selector: 'app-student-grades-page',
  imports: [FormsModule, TuiBadge, DecimalPipe],
  templateUrl: './student-grades-page.component.html',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export default class StudentGradesPageComponent {
  private readonly studentsFacade = inject(StudentCoursesFacadeService);
  coursesAndGrades = toSignal(this.studentsFacade.studentAllGrades$);

  average(array: GradedTaskBlockResponse[]) {
    const finalArray = array?.filter((e) => !!e.grade) ?? [];
    return finalArray.length === 0
      ? 0
      : finalArray.reduce((acc, t) => acc + t.grade, 0) / finalArray.length;
  }
}

import { ChangeDetectionStrategy, Component, inject } from '@angular/core';
import { StudentCoursesFacadeService } from '../student-courses-facade.service';
import { FormsModule } from '@angular/forms';
import { AsyncPipe } from '@angular/common';
import { TuiEditorSocket } from '@taiga-ui/editor';
import { TuiAccordion } from '@taiga-ui/experimental';
import { TuiIcon } from '@taiga-ui/core';
import StudentTaskBlocksAccordionItemComponent from '../../components/student-task-blocks-accordion-item/student-task-blocks-accordion-item.component';
import { map } from 'rxjs/operators';

@Component({
  selector: 'app-student-course-page',
  imports: [
    FormsModule,
    AsyncPipe,
    TuiEditorSocket,
    TuiAccordion,
    TuiIcon,
    StudentTaskBlocksAccordionItemComponent,
  ],
  templateUrl: './student-course-page.component.html',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export default class StudentCoursePageComponent {
  private readonly coursesFacade = inject(StudentCoursesFacadeService);
  protected readonly currentCourse$ = this.coursesFacade.currentCourse$;
  protected readonly sortedTaskBlocks$ = this.currentCourse$.pipe(
    map((course) =>
      course.taskBlocks.sort(
        (a, b) => a.startTime.getTime() - b.startTime.getTime(),
      ),
    ),
  );

  inFuture(date: Date): boolean {
    return Date.now() < date.getTime();
  }
}

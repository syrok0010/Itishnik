import { ChangeDetectionStrategy, Component, inject } from '@angular/core';
import { StudentCoursesFacadeService } from '../student-courses-facade.service';
import { FormsModule } from '@angular/forms';
import { AsyncPipe } from '@angular/common';
import { TuiEditorSocket } from '@taiga-ui/editor';
import { TuiAccordion } from '@taiga-ui/experimental';
import { TuiIcon } from '@taiga-ui/core';
import StudentTaskBlocksAccordionItemComponent from '../../components/student-task-blocks-accordion-item/student-task-blocks-accordion-item.component';

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
}

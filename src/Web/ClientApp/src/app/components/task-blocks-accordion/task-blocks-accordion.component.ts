import { ChangeDetectionStrategy, Component, inject } from '@angular/core';
import { TuiAccordion } from '@taiga-ui/experimental';
import { CoursesFacadeService } from '../../courses-facade.service';
import { map } from 'rxjs/operators';
import {
  TuiInputDateTimeModule,
  TuiInputModule,
  TuiInputTimeModule,
} from '@taiga-ui/legacy';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { AsyncPipe } from '@angular/common';
import TaskBlocksAccordionItemComponent from '../task-blocks-accordion-item/task-blocks-accordion-item.component';

@Component({
  selector: 'app-task-blocks-accordion',
  imports: [
    TuiAccordion,
    TuiInputModule,
    FormsModule,
    ReactiveFormsModule,
    TuiInputDateTimeModule,
    TuiInputTimeModule,
    TaskBlocksAccordionItemComponent,
    AsyncPipe,
  ],
  templateUrl: './task-blocks-accordion.component.html',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export default class TaskBlocksAccordionComponent {
  private readonly coursesFacade = inject(CoursesFacadeService);

  taskBlocks$ = this.coursesFacade.currentCourse$.pipe(
    map((course) => course.taskBlocks),
  );
}

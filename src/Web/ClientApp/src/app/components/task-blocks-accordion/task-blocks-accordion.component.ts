import {
  ChangeDetectionStrategy,
  Component,
  computed,
  inject,
} from '@angular/core';
import { TuiAccordion } from '@taiga-ui/experimental';
import { CoursesFacadeService } from '../../courses-facade.service';
import { map } from 'rxjs/operators';
import {
  TuiInputDateTimeModule,
  TuiInputModule,
  TuiInputTimeModule,
} from '@taiga-ui/legacy';
import {
  FormControl,
  FormGroup,
  FormsModule,
  ReactiveFormsModule,
} from '@angular/forms';
import { TuiTextfield } from '@taiga-ui/core';
import { toSignal } from '@angular/core/rxjs-interop';
import { TuiDay, TuiTime } from '@taiga-ui/cdk';
import { TuiTextarea } from '@taiga-ui/kit';

@Component({
  selector: 'app-task-blocks-accordion',
  imports: [
    TuiAccordion,
    TuiInputModule,
    FormsModule,
    ReactiveFormsModule,
    TuiTextfield,
    TuiTextarea,
    TuiInputDateTimeModule,
    TuiInputTimeModule,
  ],
  templateUrl: './task-blocks-accordion.component.html',
  styles: ``,
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export default class TaskBlocksAccordionComponent {
  coursesFacade = inject(CoursesFacadeService);
  taskBlocks = toSignal(
    this.coursesFacade.currentCourse$.pipe(map((course) => course.taskBlocks)),
  );
  taskBlockForms = computed(() =>
    this.taskBlocks().map(
      (taskBlock) =>
        new FormGroup({
          name: new FormControl<string>(taskBlock.name),
          description: new FormControl<string | null>(taskBlock.description),
          startTime: new FormControl<[TuiDay, TuiTime]>([
            TuiDay.fromLocalNativeDate(taskBlock.startTime),
            TuiTime.fromLocalNativeDate(taskBlock.startTime),
          ]),
          endTime: new FormControl<[TuiDay, TuiTime]>([
            TuiDay.fromLocalNativeDate(taskBlock.endTime),
            TuiTime.fromLocalNativeDate(taskBlock.endTime),
          ]),
          timeAllowed: new FormControl<TuiTime>(
            TuiTime.fromString(taskBlock.timeAllowed),
          ),
        }),
    ),
  );
  currentDay = TuiDay.currentLocal();
}

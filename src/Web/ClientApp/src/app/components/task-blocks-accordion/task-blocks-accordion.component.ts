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
  Validators,
} from '@angular/forms';
import { TuiButton, TuiIcon, TuiTextfield } from '@taiga-ui/core';
import { toSignal } from '@angular/core/rxjs-interop';
import { TuiDay, TuiTime } from '@taiga-ui/cdk';
import { TuiTextarea } from '@taiga-ui/kit';
import { RouterLink } from '@angular/router';

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
    TuiButton,
    RouterLink,
    TuiIcon,
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
    (this.taskBlocks() ?? []).map(
      (taskBlock) =>
        new FormGroup({
          name: new FormControl<string>(taskBlock.name, [Validators.required]),
          description: new FormControl<string | null>(taskBlock.description),
          startTime: new FormControl<[TuiDay, TuiTime]>(
            [
              TuiDay.fromLocalNativeDate(taskBlock.startTime),
              TuiTime.fromLocalNativeDate(taskBlock.startTime),
            ],
            Validators.required,
          ),
          endTime: new FormControl<[TuiDay, TuiTime]>(
            [
              TuiDay.fromLocalNativeDate(taskBlock.endTime),
              TuiTime.fromLocalNativeDate(taskBlock.endTime),
            ],
            Validators.required,
          ),
          timeAllowed: new FormControl<TuiTime>(
            TuiTime.fromString(taskBlock.timeAllowed),
            Validators.required,
          ),
        }),
    ),
  );
  currentDay = TuiDay.currentLocal();

  async saveTaskBlock(taskBlockIndex: number) {
    const form = this.taskBlockForms()[taskBlockIndex];
    const taskBlock = this.taskBlocks()[taskBlockIndex];
    let startTime = form.value.startTime[0].toLocalNativeDate().getTime();
    if (!!form.value.startTime[1])
      startTime += form.value.startTime[1].toAbsoluteMilliseconds();
    let endTime = form.value.endTime[0].toLocalNativeDate().getTime();
    if (!!form.value.endTime[1])
      endTime += form.value.endTime[1].toAbsoluteMilliseconds();
    await this.coursesFacade.updateTaskBlock(
      taskBlock.courseId,
      taskBlock.id,
      form.value.name,
      form.value.description,
      new Date(startTime),
      new Date(endTime),
      form.value.timeAllowed.toString('HH:MM:SS'),
    );
  }

  async addTasksToBlock(taskBlockId: string, courseId: string) {}
}

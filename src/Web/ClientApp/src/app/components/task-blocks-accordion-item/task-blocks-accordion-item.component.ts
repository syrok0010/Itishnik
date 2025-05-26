import {
  ChangeDetectionStrategy,
  Component,
  computed,
  inject,
  input,
  signal,
} from '@angular/core';
import {
  AbstractControl,
  FormControl,
  FormGroup,
  ReactiveFormsModule,
  ValidationErrors,
  ValidatorFn,
  Validators,
} from '@angular/forms';
import {
  TuiInputDateTimeModule,
  TuiInputModule,
  TuiInputTimeModule,
} from '@taiga-ui/legacy';
import {
  TuiButton,
  tuiDialog,
  TuiError,
  TuiIcon,
  TuiScrollbar,
  TuiTextfield,
} from '@taiga-ui/core';
import {
  TuiBadge,
  TuiCarousel,
  TuiFieldErrorPipe,
  TuiInputNumber,
  TuiPagination,
  TuiStatus,
  TuiTextarea,
  tuiValidationErrorsProvider,
} from '@taiga-ui/kit';
import { TuiTable } from '@taiga-ui/addon-table';
import { RouterLink } from '@angular/router';
import { TaskBlockResponse } from '../../web-api-client';
import { TuiDay, TuiTime } from '@taiga-ui/cdk';
import { CoursesFacadeService } from '../../courses-facade.service';
import { firstValueFrom } from 'rxjs';
import SelectTasksDialogComponent from '../select-tasks-dialog.component';
import { AsyncPipe } from '@angular/common';

@Component({
  selector: 'app-task-blocks-accordion-item',
  imports: [
    ReactiveFormsModule,
    TuiInputModule,
    TuiTextfield,
    TuiTextarea,
    TuiInputDateTimeModule,
    TuiInputTimeModule,
    TuiButton,
    TuiTable,
    TuiIcon,
    RouterLink,
    TuiInputNumber,
    TuiError,
    TuiFieldErrorPipe,
    AsyncPipe,
    TuiCarousel,
    TuiPagination,
    TuiScrollbar,
    TuiBadge,
    TuiStatus,
  ],
  templateUrl: './task-blocks-accordion-item.component.html',
  changeDetection: ChangeDetectionStrategy.OnPush,
  providers: [
    tuiValidationErrorsProvider({
      required: 'Заполните поле',
      beforeStart: 'Дата конца должна быть позднее начала',
    }),
  ],
})
export default class TaskBlocksAccordionItemComponent {
  private readonly coursesFacade = inject(CoursesFacadeService);
  private readonly dialog = tuiDialog(SelectTasksDialogComponent, {
    dismissible: true,
    size: 'auto',
  });

  timeValidator(): ValidatorFn {
    return (control: AbstractControl): ValidationErrors | null => {
      const start = control.get('startTime');
      const end = control.get('endTime');
      if (!start.value || !end.value) return null;
      const [startDay, startTime]: [TuiDay, TuiTime] = start.value;
      const [endDay, endTime]: [TuiDay, TuiTime] = end.value;

      if (!startTime || !endTime) return null;

      if (
        startDay.dayAfter(endDay) ||
        (startDay.daySame(endDay) &&
          startTime.toAbsoluteMilliseconds() -
            endTime.toAbsoluteMilliseconds() >=
            0)
      ) {
        end.setErrors({ beforeStart: true });
        end.markAsTouched();
      }
      return null;
    };
  }

  taskBlockComments = signal<string[]>([]);
  index = 0;

  taskBlock = input.required<TaskBlockResponse>();
  formGroup = computed(
    () =>
      new FormGroup(
        {
          name: new FormControl<string>(this.taskBlock().name, [
            Validators.required,
          ]),
          description: new FormControl<string | null>(
            this.taskBlock().description,
          ),
          startTime: new FormControl<[TuiDay, TuiTime]>(
            [
              TuiDay.fromLocalNativeDate(this.taskBlock().startTime),
              TuiTime.fromLocalNativeDate(this.taskBlock().startTime),
            ],
            Validators.required,
          ),
          endTime: new FormControl<[TuiDay, TuiTime]>(
            [
              TuiDay.fromLocalNativeDate(this.taskBlock().endTime),
              TuiTime.fromLocalNativeDate(this.taskBlock().endTime),
            ],
            Validators.required,
          ),
          timeAllowed: new FormControl<TuiTime>(
            TuiTime.fromString(this.taskBlock().timeAllowed),
            Validators.required,
          ),
        },
        [this.timeValidator()],
      ),
  );
  weightControls = computed(() =>
    this.taskBlock().weights.map((w) => new FormControl(w)),
  );
  taskBlockTableData = computed(() =>
    this.taskBlock().tasks.map((t, i) => ({
      task: t,
      weight: this.taskBlock().weights[i],
    })),
  );

  tableColumns = ['number', 'task', 'weight'] as readonly string[];

  currentDay = TuiDay.currentLocal();

  async saveTaskBlock() {
    const form = this.formGroup();
    const taskBlock = this.taskBlock();
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

  async addTasksToBlock() {
    const taskIds = await firstValueFrom(
      this.dialog(this.taskBlock().tasks.map((t) => t.id)),
    );
    await this.coursesFacade.addTasksToTaskBlock(
      this.taskBlock().courseId,
      this.taskBlock().id,
      taskIds,
    );
  }

  async removeTaskFromBlock(taskId: string) {
    await this.coursesFacade.removeTasksFromTaskBlock(
      this.taskBlock().courseId,
      this.taskBlock().id,
      [taskId],
    );
  }

  async saveWeights() {
    const weights = this.weightControls().map((fc) => fc.value);
    await this.coursesFacade.updateTaskBlockWeights(
      this.taskBlock().courseId,
      this.taskBlock().id,
      weights,
    );
  }
}

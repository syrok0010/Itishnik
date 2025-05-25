import {
  ChangeDetectionStrategy,
  Component,
  computed,
  inject,
  TemplateRef,
  viewChild,
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
import {
  TuiButton,
  TuiDialogService,
  TuiIcon,
  TuiTextfield,
} from '@taiga-ui/core';
import { toSignal } from '@angular/core/rxjs-interop';
import { TuiDay, TuiTime } from '@taiga-ui/cdk';
import { TuiInputNumber, TuiTextarea } from '@taiga-ui/kit';
import { RouterLink } from '@angular/router';
import TasksTableComponent from '../tasks-table/tasks-table.component';
import { SafeSubscriber } from 'rxjs/internal/Subscriber';
import { firstValueFrom } from 'rxjs';
import { TaskListDto } from 'src/app/web-api-client';

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
    TasksTableComponent,
    TuiInputNumber,
  ],
  templateUrl: './task-blocks-accordion.component.html',
  styles: ``,
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export default class TaskBlocksAccordionComponent {
  private readonly coursesFacade = inject(CoursesFacadeService);
  private readonly dialogs = inject(TuiDialogService);

  addTasksDialogTemplate = viewChild<TemplateRef<any>>('addTasksDialog');
  taskTableTemplate = viewChild<TasksTableComponent>('taskTable');

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

  weightControls = computed(() =>
    this.taskBlocks().map((tb) => tb.weights.map((w) => new FormControl(w))),
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

  async addTasksToBlock(
    taskBlockId: string,
    courseId: string,
    existingTasks: TaskListDto[],
  ) {
    const taskIds = await firstValueFrom(
      this.dialogs.open<string[]>(this.addTasksDialogTemplate(), {
        size: 'auto',
        data: {
          taskIds: existingTasks.map((t) => t.id),
        },
      }),
    );
    await this.coursesFacade.addTasksToTaskBlock(
      courseId,
      taskBlockId,
      taskIds,
    );
  }

  addTasks(context: SafeSubscriber<string[]>) {
    const table = this.taskTableTemplate();
    const selected = table
      .selectedArray()
      .value.map((e, i) => [e, i])
      .filter(([e]) => e)
      .map(([, i]) => table.filteredTasks()[i as number].id);
    context.next(selected);
    context.complete();
  }

  async removeTaskFromBlock(
    courseId: string,
    taskBlockId: string,
    taskId: string,
  ) {
    await this.coursesFacade.removeTasksFromTaskBlock(courseId, taskBlockId, [
      taskId,
    ]);
  }
}

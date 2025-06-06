import {
  ChangeDetectionStrategy,
  Component,
  computed,
  inject,
  input,
  Signal,
} from '@angular/core';
import { GradedTaskBlockDto, SolutionDto } from '../../web-api-client';
import { DatePipe } from '@angular/common';
import {
  TuiAlertService,
  TuiButton,
  tuiDialog,
  TuiDialogService,
  TuiIcon,
} from '@taiga-ui/core';
import { TUI_CONFIRM, TuiConfirmData } from '@taiga-ui/kit';
import { firstValueFrom } from 'rxjs';
import { StudentCoursesFacadeService } from '../../student/student-courses-facade.service';
import { CountdownTimerComponent } from '../countdown-timer.component';
import { TuiTime } from '@taiga-ui/cdk';
import TaskSolutionDialogComponent from '../task-solution-dialog.component';

type TaskBlockStatus = 'BeforeStart' | 'CanStart' | 'Solving' | 'Finished';
const DEFAULT_SOLUTION_TEXT = 'Здесь будет текст вашего решения';

@Component({
  selector: 'app-student-task-blocks-accordion-item',
  imports: [DatePipe, TuiIcon, TuiButton, CountdownTimerComponent],
  templateUrl: './student-task-blocks-accordion-item.component.html',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export default class StudentTaskBlocksAccordionItemComponent {
  private readonly courseFacade = inject(StudentCoursesFacadeService);
  private readonly dialogs = inject(TuiDialogService);
  private readonly alerts = inject(TuiAlertService);
  private readonly solutionDialog = tuiDialog(TaskSolutionDialogComponent, {
    size: 'auto',
    label: 'Решение задания',
  });

  taskBlock = input.required<GradedTaskBlockDto>();
  courseId = input.required<string>();

  status: Signal<TaskBlockStatus> = computed(() => {
    const block = this.taskBlock();
    const blockStarted = Date.now() - block.startTime.getTime() > 0;
    const blockFinished = Date.now() - this.studentEndTime().getTime() > 0;
    const userStarted = !!block.studentStartTime;

    if (!blockStarted) return 'BeforeStart';
    if (!userStarted) return 'CanStart';
    if (!blockFinished) return 'Solving';
    return 'Finished';
  });

  timeAllowed = computed(() => {
    const timeStr = this.taskBlock().timeAllowed;
    const hours = parseInt(timeStr.substring(0, 2), 10);
    const minutes = parseInt(timeStr.substring(3, 5), 10);
    return `${hours} ч. ${minutes} мин.`;
  });

  studentEndTime = computed(() => {
    const block = this.taskBlock();
    const blockEndMs = block.endTime.getTime();
    const studentStartMs = block.studentStartTime
      ? block.studentStartTime.getTime()
      : Date.now();
    const timeAllowedMs = TuiTime.fromString(
      block.timeAllowed,
    ).toAbsoluteMilliseconds();
    const studentCalculatedEndMs = studentStartMs + timeAllowedMs;
    return new Date(Math.min(blockEndMs, studentCalculatedEndMs));
  });

  async confirmStart() {
    const data: TuiConfirmData = {
      content: `Вы не сможете прервать решение. У вас будет ровно ${this.timeAllowed()} на решение всех задач.`,
      yes: 'Начать',
      no: 'Отменить',
    };

    const shouldStartSolution = await firstValueFrom(
      this.dialogs.open<boolean>(TUI_CONFIRM, {
        label: 'Начать решение?',
        size: 's',
        data,
      }),
    );

    if (!shouldStartSolution) {
      return;
    }

    await this.courseFacade.startSolution(this.courseId(), this.taskBlock().id);
  }

  startedTaskSolution(solution: SolutionDto): boolean {
    return !!solution && solution.text !== DEFAULT_SOLUTION_TEXT;
  }

  getTaskStatusIcon(solution: SolutionDto): string {
    if (this.status() === 'Solving') {
      return this.startedTaskSolution(solution)
        ? '@tui.circle-check-big'
        : '@tui.pen';
    }
    return '@tui.circle';
  }

  getTaskStatusIconColor(solution: SolutionDto): string {
    if (this.status() === 'Solving') {
      return this.startedTaskSolution(solution) ? 'green' : 'blue';
    }
    return 'gray';
  }

  getTaskActionText(solution: SolutionDto): string {
    if (this.status() === 'Solving') {
      return this.startedTaskSolution(solution)
        ? 'Редактировать решение'
        : 'Решить';
    }
    return 'Посмотреть решение';
  }

  openTaskDialog(solution: SolutionDto): void {
    this.solutionDialog({
      solution,
      taskBlockId: this.taskBlock().id,
      courseId: this.courseId(),
      isEditable: this.status() === 'Solving',
    }).subscribe();
  }

  onTimerExpired(): void {
    this.alerts
      .open(
        'Время на выполнение работы истекло. Сохранение решений больше недоступно.',
        {
          label: 'Время вышло!',
          appearance: 'warning',
          autoClose: 10000,
        },
      )
      .subscribe();
  }
}

import {
  ChangeDetectionStrategy,
  Component,
  computed,
  inject,
  input,
  Signal,
} from '@angular/core';
import { GradedTaskBlockDto } from '../../web-api-client';
import { DatePipe } from '@angular/common';
import { TuiButton, TuiDialogService, TuiIcon } from '@taiga-ui/core';
import { TUI_CONFIRM, TuiConfirmData } from '@taiga-ui/kit';
import { firstValueFrom } from 'rxjs';
import { StudentCoursesFacadeService } from '../../student/student-courses-facade.service';

type TaskBlockStatus = 'BeforeStart' | 'CanStart' | 'Solving' | 'Finished';

@Component({
  selector: 'app-student-task-blocks-accordion-item',
  imports: [DatePipe, TuiIcon, TuiButton],
  templateUrl: './student-task-blocks-accordion-item.component.html',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export default class StudentTaskBlocksAccordionItemComponent {
  private readonly courseFacade = inject(StudentCoursesFacadeService);
  private readonly dialogs = inject(TuiDialogService);
  taskBlock = input.required<GradedTaskBlockDto>();
  courseId = input.required<string>();

  status: Signal<TaskBlockStatus> = computed(() => {
    const blockStarted = Date.now() - this.taskBlock().startTime.getTime() > 0;
    const blockFinished = Date.now() - this.taskBlock().endTime.getTime() > 0;
    const userStarted = !!this.taskBlock().studentStartTime;

    if (!blockStarted) return 'BeforeStart';
    if (!userStarted) return 'CanStart';
    if (!blockFinished) return 'Solving';
    return 'Finished';
  });

  timeAllowed = computed(() => {
    const hours = parseInt(this.taskBlock().timeAllowed.substring(0, 2), 10);
    const minutes = parseInt(this.taskBlock().timeAllowed.substring(3, 4), 10);
    return `${hours} ч. ${minutes} мин.`;
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
}

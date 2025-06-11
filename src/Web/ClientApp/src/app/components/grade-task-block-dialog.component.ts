import {
  ChangeDetectionStrategy,
  Component,
  computed,
  effect,
  inject,
  signal,
} from '@angular/core';
import { injectContext } from '@taiga-ui/polymorpheus';
import {
  TuiButton,
  TuiDialogContext,
  TuiLoader,
  TuiTextfield,
} from '@taiga-ui/core';
import { TuiEditorSocket } from '@taiga-ui/editor';
import { CoursesFacadeService } from '../teacher/courses-facade.service';
import { map } from 'rxjs/operators';
import { toSignal } from '@angular/core/rxjs-interop';
import { TuiInputNumber } from '@taiga-ui/kit';
import {
  CoursesClient,
  GetAiVerdictCommand,
  TaskBlockResponse,
} from '../web-api-client';
import { FormControl, ReactiveFormsModule } from '@angular/forms';
import { firstValueFrom } from 'rxjs';
import { UntilDestroy, untilDestroyed } from '@ngneat/until-destroy';

export interface GradeTaskBlockDialogInput {
  gradedTaskBlockId: string;
  taskBlock: TaskBlockResponse;
}
const DEFAULT_SOLUTION_TEXT = 'Здесь будет текст вашего решения';

@UntilDestroy()
@Component({
  selector: 'app-grade-task-block-dialog',
  imports: [
    TuiEditorSocket,
    TuiButton,
    TuiLoader,
    TuiTextfield,
    TuiInputNumber,
    ReactiveFormsModule,
  ],
  template: ` <div class="flex min-h-[70dvh] w-[90dvw] flex-col justify-center">
    @let taskBlock = gradedTaskBlock();
    @let solution = currentSolution();
    @if (!!taskBlock) {
      <div class="flex flex-grow gap-6 overflow-auto py-4">
        <div class="flex w-1/2 flex-col">
          <h3 class="mb-3 text-xl font-semibold">
            Задание "{{ solution.task.name }}"
          </h3>
          <tui-editor-socket
            class="flex-grow rounded-xl border-2 p-4"
            [content]="solution.task.text"
          />
        </div>
        <div class="flex w-1/2 flex-col">
          <h3 class="mb-3 text-xl font-semibold">Решение студента</h3>
          <tui-editor-socket
            class="flex-grow rounded-xl border-2 p-4"
            [content]="
              solution.text === DEFAULT_SOLUTION_TEXT
                ? 'Студент не решил задачу'
                : solution.text
            "
          />
        </div>
      </div>
      <div class="flex items-center justify-between gap-4 border-gray-200">
        <button
          tuiButton
          type="button"
          (click)="previousSolution()"
          [disabled]="!hasPrevious()"
        >
          Предыдущая
        </button>
        <div class="flex gap-4">
          <tui-textfield>
            <label tuiLabel class="pr-20">Оценка за задачу</label>
            <input
              tuiInputNumber
              [min]="0"
              [max]="solution.weight"
              [formControl]="gradeControl"
            />
          </tui-textfield>

          <tui-loader [showLoader]="generatingVerdict()" size="m">
            <button
              class="gemini-generate-button h-full"
              (click)="getAiVerdict()"
            >
              <span class="icon">✨</span>
            </button>
          </tui-loader>
        </div>
        <button
          tuiButton
          type="button"
          (click)="nextSolution()"
          [disabled]="!hasNext()"
        >
          Следующая
        </button>
      </div>
    } @else {
      <tui-loader size="xxl" class="h-full w-full" />
    }
  </div>`,
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export default class GradeTaskBlockDialogComponent {
  public readonly context =
    injectContext<TuiDialogContext<void, GradeTaskBlockDialogInput>>();
  private readonly coursesClient = inject(CoursesClient);

  private readonly courseFacade = inject(CoursesFacadeService);
  readonly generatingVerdict = signal(false);
  readonly gradedTaskBlock = toSignal(
    this.courseFacade.studentTaskBlocks$.pipe(
      map((blocks) =>
        blocks.find((gtb) => gtb.id == this.context.data.gradedTaskBlockId),
      ),
    ),
  );
  readonly currentIndex = signal(0);

  readonly solutions = computed(() => this.gradedTaskBlock()?.solutions ?? []);
  readonly currentSolution = computed(
    () => this.solutions()[this.currentIndex()],
  );

  readonly hasPrevious = computed(() => this.currentIndex() > 0);
  readonly hasNext = computed(
    () => this.currentIndex() < this.solutions().length - 1,
  );

  gradeControl = new FormControl(0);

  constructor() {
    effect(() => {
      const solutionsCount = this.solutions().length;
      if (this.currentIndex() >= solutionsCount && solutionsCount > 0)
        this.currentIndex.set(0);
    });
    effect(() =>
      this.gradeControl.setValue(this.currentSolution().grade, {
        emitEvent: false,
      }),
    );
    this.gradeControl.valueChanges
      .pipe(untilDestroyed(this))
      .subscribe((g) =>
        this.courseFacade.setSolutionGrade(
          this.gradedTaskBlock().id,
          this.context.data.taskBlock.id,
          this.currentSolution().task.id,
          this.currentSolution().id,
          this.currentSolution().grade,
          g,
        ),
      );
  }

  public previousSolution(): void {
    this.currentIndex.update((i) => i - 1);
  }

  public nextSolution(): void {
    this.currentIndex.update((i) => i + 1);
  }

  async getAiVerdict() {
    try {
      this.generatingVerdict.set(true);
      const response = await firstValueFrom(
        this.coursesClient.getAiVerdict(
          this.context.data.taskBlock.courseId,
          this.context.data.taskBlock.id,
          new GetAiVerdictCommand({
            courseId: this.context.data.taskBlock.courseId,
            taskBlockId: this.context.data.taskBlock.id,
            solutionId: this.currentSolution().id,
          }),
        ),
      );

      this.gradeControl.setValue(response.score, { emitEvent: true });
    } finally {
      this.generatingVerdict.set(false);
    }
  }

  protected readonly DEFAULT_SOLUTION_TEXT = DEFAULT_SOLUTION_TEXT;
}

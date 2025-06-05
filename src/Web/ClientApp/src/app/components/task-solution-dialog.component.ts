import {
  ChangeDetectionStrategy,
  Component,
  inject,
  Injector,
} from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormControl, ReactiveFormsModule } from '@angular/forms';
import { TuiButton, TuiDialogContext } from '@taiga-ui/core';
import {
  TUI_EDITOR_DEFAULT_EXTENSIONS,
  TUI_EDITOR_EXTENSIONS,
  TuiEditor,
  TuiEditorSocket,
} from '@taiga-ui/editor';
import { SolutionDto } from '../web-api-client';
import { injectContext } from '@taiga-ui/polymorpheus';
import { StudentCoursesFacadeService } from '../student/student-courses-facade.service';

export interface TaskSolutionDialogData {
  solution: SolutionDto;
  isEditable: boolean;
  courseId: string;
  taskBlockId: string;
}

@Component({
  selector: 'app-task-solution-dialog',
  standalone: true,
  imports: [
    CommonModule,
    ReactiveFormsModule,
    TuiEditorSocket,
    TuiEditor,
    TuiButton,
  ],
  template: `
    <div class="flex min-h-[70dvh] min-w-[70dvw] flex-col">
      <div class="flex flex-grow gap-6 overflow-auto p-4">
        <div class="flex w-1/2 flex-col">
          <h3 class="mb-3 text-xl font-semibold text-gray-800">Задание</h3>
          <tui-editor-socket
            class="flex-grow"
            [content]="context.data.solution.task.text"
          />
        </div>
        <div class="flex w-1/2 flex-col">
          <h3 class="mb-3 text-xl font-semibold text-gray-800">Ваше решение</h3>
          @if (context.data.isEditable) {
            <tui-editor class="flex-grow" [formControl]="solutionControl" />
          } @else {
            <tui-editor-socket
              class="flex-grow"
              [content]="context.data.solution.text"
            />
          }
        </div>
      </div>
      <div class="flex justify-end gap-4 border-t border-gray-200 p-4">
        <button
          tuiButton
          type="button"
          [appearance]="context.data.isEditable ? 'flat' : 'primary'"
          (click)="closeDialog()"
        >
          Закрыть
        </button>
        @if (context.data.isEditable) {
          <button
            tuiButton
            type="button"
            appearance="primary"
            [disabled]="!solutionControl.dirty || solutionControl.invalid"
            (click)="save()"
          >
            Сохранить
          </button>
        }
      </div>
    </div>
  `,
  changeDetection: ChangeDetectionStrategy.OnPush,
  providers: [
    {
      provide: TUI_EDITOR_EXTENSIONS,
      deps: [Injector],
      useFactory: (injector: Injector) => [
        ...TUI_EDITOR_DEFAULT_EXTENSIONS,
        import('@taiga-ui/editor').then(({ setup }) => setup({ injector })),
      ],
    },
  ],
})
export default class TaskSolutionDialogComponent {
  public readonly context =
    injectContext<TuiDialogContext<void, TaskSolutionDialogData>>();
  private readonly courseFacade = inject(StudentCoursesFacadeService);

  public readonly solutionControl: FormControl<string> = new FormControl(
    this.context.data.solution.text,
  );

  async save() {
    await this.courseFacade.saveSolution(
      this.context.data.courseId,
      this.context.data.taskBlockId,
      this.context.data.solution.task.id,
      this.solutionControl.value,
    );
    this.context.completeWith();
  }

  closeDialog(): void {
    this.context.completeWith();
  }
}

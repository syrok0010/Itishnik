import { ChangeDetectionStrategy, Component, inject } from '@angular/core';
import { TuiAccordion } from '@taiga-ui/experimental';
import { CoursesFacadeService } from '../../teacher/courses-facade.service';
import { map } from 'rxjs/operators';
import {
  TuiInputDateTimeModule,
  TuiInputModule,
  TuiInputTimeModule,
  TuiTextfieldControllerModule,
} from '@taiga-ui/legacy';
import {
  FormControl,
  FormsModule,
  ReactiveFormsModule,
  Validators,
} from '@angular/forms';
import { AsyncPipe } from '@angular/common';
import TaskBlocksAccordionItemComponent from '../task-blocks-accordion-item/task-blocks-accordion-item.component';
import { TuiButton } from '@taiga-ui/core';
import { firstValueFrom } from 'rxjs';

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
    TuiButton,
    TuiTextfieldControllerModule,
  ],
  templateUrl: './task-blocks-accordion.component.html',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export default class TaskBlocksAccordionComponent {
  private readonly coursesFacade = inject(CoursesFacadeService);

  taskBlocks$ = this.coursesFacade.currentCourse$.pipe(
    map((course) =>
      course.taskBlocks.sort(
        (a, b) => b.startTime.getTime() - a.startTime.getTime(),
      ),
    ),
  );

  publicTaskBlocks$ = this.taskBlocks$.pipe(
    map((taskBlocks) => taskBlocks.filter((tb) => tb.isPublic)),
  );

  notPublicTaskBlocks$ = this.taskBlocks$.pipe(
    map((taskBlocks) => taskBlocks.filter((tb) => !tb.isPublic)),
  );

  nameControl = new FormControl<string>('', [Validators.required]);

  async createTaskBlock() {
    const course = await firstValueFrom(this.coursesFacade.currentCourse$);
    await this.coursesFacade.createTaskBlock(course.id, this.nameControl.value);
    this.nameControl.reset();
  }
}

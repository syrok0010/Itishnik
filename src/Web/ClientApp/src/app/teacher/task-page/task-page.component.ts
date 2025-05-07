import {
  ChangeDetectionStrategy,
  Component,
  computed,
  effect,
  inject,
  TemplateRef,
  viewChild,
} from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { TasksFacadeService } from '../../tasks-facade.service';
import {
  TuiAutoColorPipe,
  TuiButton,
  TuiDialogService,
  TuiInitialsPipe,
  TuiTitle,
} from '@taiga-ui/core';
import { toSignal } from '@angular/core/rxjs-interop';
import { TuiAvatar, TuiBadge, TuiChip, TuiStatus } from '@taiga-ui/kit';
import {
  TuiMultiSelectModule,
  TuiTextfieldControllerModule,
} from '@taiga-ui/legacy';
import { FormControl, ReactiveFormsModule } from '@angular/forms';
import TagMultiselectInputComponent from '../../components/tag-multiselect-input.component';

@Component({
  selector: 'app-task-page',
  imports: [
    TuiButton,
    TuiAutoColorPipe,
    TuiAvatar,
    TuiInitialsPipe,
    TuiTitle,
    TuiChip,
    TuiBadge,
    TuiStatus,
    TuiMultiSelectModule,
    ReactiveFormsModule,
    TuiTextfieldControllerModule,
    TagMultiselectInputComponent,
  ],
  templateUrl: './task-page.component.html',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export default class TaskPageComponent {
  editTagsDialogTemplate = viewChild<TemplateRef<any>>('editTagsDialog');
  private readonly route = inject(ActivatedRoute);
  private readonly taskFacade = inject(TasksFacadeService);
  private readonly dialogs = inject(TuiDialogService);
  id: string = this.route.snapshot.paramMap.get('id');

  tagControl = new FormControl<string[]>([]);

  currentTaskChain = toSignal(this.taskFacade.currentTaskChain$);
  currentVersion = computed(() =>
    !this.currentTaskChain()
      ? null
      : this.currentTaskChain().find((t) => t.id === this.id),
  );

  protected showEditTagsDialog(): void {
    this.dialogs
      .open(this.editTagsDialogTemplate(), { label: 'Редактировать теги' })
      .subscribe();
  }

  constructor() {
    effect(() => {
      if (!this.currentVersion()) return;
      this.tagControl.setValue(this.currentVersion().tags.map((t) => t.id));
    });
  }

  async updateTaskTags() {
    await this.taskFacade.updateTags(
      this.currentVersion().id,
      this.tagControl.value,
    );
  }

  publish() {
    throw new Error('Method not implemented.');
  }
}

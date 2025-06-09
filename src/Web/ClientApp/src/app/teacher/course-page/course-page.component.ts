import {
  ChangeDetectionStrategy,
  Component,
  computed,
  effect,
  inject,
  Injector,
  OnInit,
  signal,
} from '@angular/core';
import { CoursesFacadeService } from '../courses-facade.service';
import {
  ActivatedRoute,
  RouterLink,
  RouterLinkActive,
  RouterOutlet,
} from '@angular/router';
import {
  TuiAutoColorPipe,
  TuiButton,
  TuiInitialsPipe,
  TuiTextfield,
  TuiTitle,
} from '@taiga-ui/core';
import {
  TuiAvatar,
  TuiChevron,
  TuiComboBox,
  TuiDataListWrapperComponent,
  TuiFilterByInputPipe,
  TuiTabs,
} from '@taiga-ui/kit';
import { FormControl, ReactiveFormsModule } from '@angular/forms';
import { toSignal } from '@angular/core/rxjs-interop';
import { UsersFacadeService } from '../../users-facade.service';
import { map } from 'rxjs/operators';
import { AsyncPipe } from '@angular/common';
import { UserDto } from '../../web-api-client';
import { TuiStringMatcher } from '@taiga-ui/cdk';
import { TuiMultiSelectModule } from '@taiga-ui/legacy';
import { FullNamePipe } from '../../components/full-name-pipe.pipe';
import {
  TUI_EDITOR_DEFAULT_EXTENSIONS,
  TUI_EDITOR_EXTENSIONS,
  TuiEditor,
} from '@taiga-ui/editor';

@Component({
  selector: 'app-course-page',
  imports: [
    TuiTextfield,
    TuiButton,
    ReactiveFormsModule,
    TuiTabs,
    RouterLink,
    RouterOutlet,
    RouterLinkActive,
    TuiAutoColorPipe,
    TuiInitialsPipe,
    TuiAvatar,
    TuiTitle,
    AsyncPipe,
    TuiChevron,
    TuiDataListWrapperComponent,
    TuiMultiSelectModule,
    TuiFilterByInputPipe,
    TuiComboBox,
    FullNamePipe,
    TuiEditor,
  ],
  templateUrl: './course-page.component.html',
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
export default class CoursePageComponent implements OnInit {
  coursesFacade = inject(CoursesFacadeService);
  route = inject(ActivatedRoute);
  usersFacade = inject(UsersFacadeService);

  currentCourse = toSignal(this.coursesFacade.currentCourse$);
  allTeachers = toSignal(this.usersFacade.selectedUsers$);

  description = computed(() => this.currentCourse()?.description);
  descriptionControl = new FormControl<string | null>('');

  teacherControl = new FormControl<UserDto | null>(null);
  editTeacherMode = signal(false);
  isAdmin$ = this.usersFacade.authInfo$.pipe(
    map((info) => info.roles.includes('Administrator')),
  );

  constructor() {
    effect(() => {
      if (!this.currentCourse()) return;
      this.descriptionControl.setValue(this.currentCourse().description, {
        emitEvent: false,
      });
    });
  }

  ngOnInit(): void {
    this.usersFacade.selectRoles(['Teacher']);
  }

  async saveDescription() {
    await this.coursesFacade.updateDescription(
      this.currentCourse().id,
      this.descriptionControl.value,
    );
    this.descriptionControl.markAsPristine();
  }

  matcher: TuiStringMatcher<UserDto> = (user, search): boolean => {
    if (
      `${user.surname.toLowerCase()} ${user.name.toLowerCase()} ${user.patronymic.toLowerCase()}`.includes(
        search.toLowerCase(),
      )
    )
      return true;
    return user.email.toLowerCase().includes(search.toLowerCase());
  };

  async saveSelectedAuthor() {
    await this.coursesFacade.updateCourseTeacher(
      this.currentCourse().id,
      this.teacherControl.value.id,
    );
    this.editTeacherMode.set(false);
    this.teacherControl.reset();
    this.teacherControl.markAsPristine();
  }
}

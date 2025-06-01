import { ChangeDetectionStrategy, Component, inject } from '@angular/core';
import {
  FormArray,
  FormControl,
  FormsModule,
  ReactiveFormsModule,
  Validators,
} from '@angular/forms';
import { TuiInputModule, TuiTextfieldControllerModule } from '@taiga-ui/legacy';
import {
  TuiAutoColorPipe,
  TuiButton,
  TuiIcon,
  TuiInitialsPipe,
  TuiTextfield,
  TuiTitle,
} from '@taiga-ui/core';
import { CoursesFacadeService } from '../../teacher/courses-facade.service';
import { combineLatest, firstValueFrom, startWith } from 'rxjs';
import { map } from 'rxjs/operators';
import { AsyncPipe } from '@angular/common';
import { TuiAvatar } from '@taiga-ui/kit';

@Component({
  selector: 'app-course-students',
  imports: [
    ReactiveFormsModule,
    TuiTextfieldControllerModule,
    TuiInputModule,
    TuiButton,
    TuiTextfield,
    AsyncPipe,
    FormsModule,
    TuiAutoColorPipe,
    TuiAvatar,
    TuiInitialsPipe,
    TuiTitle,
    TuiIcon,
  ],
  templateUrl: './course-students.component.html',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export default class CourseStudentsComponent {
  courseFacade = inject(CoursesFacadeService);
  emailsArray = new FormArray<FormControl<string>>([this.getEmailControl()]);
  search = new FormControl('');

  existingStudents$ = this.courseFacade.currentCoursesStudents$;
  filteredStudents$ = combineLatest([
    this.existingStudents$,
    this.search.valueChanges.pipe(startWith('')),
  ]).pipe(
    map(([students, search]) =>
      students.filter(
        (s) =>
          s.fullName.toLowerCase().includes(search.toLowerCase()) ||
          s.email.toLowerCase().includes(search.toLowerCase()),
      ),
    ),
  );

  getEmailControl() {
    return new FormControl('', [Validators.required, Validators.email]);
  }

  addControl() {
    this.emailsArray.push(this.getEmailControl());

    setTimeout(() => {
      window.scrollTo({
        top: document.documentElement.scrollHeight,
        behavior: 'smooth',
      });
    }, 0);
  }

  async inviteStudents() {
    const course = await firstValueFrom(this.courseFacade.currentCourse$);
    await this.courseFacade.inviteStudents(course.id, this.emailsArray.value);
    this.emailsArray.reset();
  }
}

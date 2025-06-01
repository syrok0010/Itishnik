import { inject, Injectable } from '@angular/core';
import { GradedCourseResponse, StudentsClient } from '../web-api-client';
import { UntilDestroy, untilDestroyed } from '@ngneat/until-destroy';
import { map } from 'rxjs/operators';
import { BehaviorSubject } from 'rxjs';
import { TuiAlertService } from '@taiga-ui/core';

export interface StudentCoursesState {
  coursesList: GradedCourseResponse[];
}

let _state: StudentCoursesState = {
  coursesList: [],
};

@UntilDestroy()
@Injectable({
  providedIn: 'root',
})
export class StudentCoursesFacadeService {
  studentClient = inject(StudentsClient);
  private readonly alerts = inject(TuiAlertService);
  private _store: BehaviorSubject<StudentCoursesState> = new BehaviorSubject(
    _state,
  );

  coursesList$ = this._store.pipe(map((state) => state.coursesList));

  constructor() {
    this.studentClient
      .getCourses(1, 30)
      .pipe(untilDestroyed(this))
      .subscribe((x) =>
        this._store.next(
          (_state = {
            ..._state,
            coursesList: x.items,
          }),
        ),
      );
  }
}

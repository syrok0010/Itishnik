import { inject, Injectable } from '@angular/core';
import {
  GradedCourseResponse,
  StudentCourseResponse,
  StudentsClient,
} from '../web-api-client';
import { UntilDestroy, untilDestroyed } from '@ngneat/until-destroy';
import { map } from 'rxjs/operators';
import { BehaviorSubject, firstValueFrom } from 'rxjs';
import { TuiAlertService } from '@taiga-ui/core';

export interface StudentCoursesState {
  coursesList: GradedCourseResponse[];
  currentCourse: StudentCourseResponse | null;
}

let _state: StudentCoursesState = {
  coursesList: [],
  currentCourse: null,
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
  currentCourse$ = this._store.pipe(map((state) => state.currentCourse));

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

  async setCurrentCourseId(id: string) {
    const currentCourse = await firstValueFrom(
      this.studentClient.getCourse(id),
    );
    this._store.next((_state = { ..._state, currentCourse }));
  }

  async startSolution(id: string) {}
}

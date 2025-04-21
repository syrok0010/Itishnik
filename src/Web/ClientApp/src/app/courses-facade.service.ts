import { inject, Injectable } from '@angular/core';
import {
  CourseListResponse,
  CoursesClient,
  CreateCourseCommand,
} from './web-api-client';
import { BehaviorSubject, firstValueFrom, switchMap } from 'rxjs';
import { distinctUntilChanged, map, shareReplay } from 'rxjs/operators';
import { UntilDestroy, untilDestroyed } from '@ngneat/until-destroy';

export interface CoursesState {
  coursesList: CourseListResponse[];
  currentCourseId: string | null;
}

let _state: CoursesState = {
  coursesList: [],
  currentCourseId: null,
};

@UntilDestroy()
@Injectable({
  providedIn: 'root',
})
export class CoursesFacadeService {
  coursesClient = inject(CoursesClient);

  private _store: BehaviorSubject<CoursesState> = new BehaviorSubject(_state);

  coursesList$ = this._store.pipe(map((state) => state.coursesList));
  currentCourse$ = this._store.pipe(
    map((state) => state.currentCourseId),
    distinctUntilChanged(),
    switchMap((courseId) => this.coursesClient.getCourseById(courseId)),
    shareReplay(1),
  );

  constructor() {
    this.coursesClient
      .getCoursesList(1, 25)
      .pipe(untilDestroyed(this))
      .subscribe((x) =>
        this._store.next(
          (_state = {
            ..._state,
            coursesList: [..._state.coursesList, ...x.items],
          }),
        ),
      );
  }

  setCurrentCourseId(courseId: string) {
    this._store.next((_state = { ..._state, currentCourseId: courseId }));
  }

  async createCourse(name: string): Promise<void> {
    const response = await firstValueFrom(
      this.coursesClient.createCourse(
        new CreateCourseCommand({ name: name, description: '' }),
      ),
    );
    this._store.next(
      (_state = {
        ..._state,
        coursesList: [
          ..._state.coursesList,
          new CourseListResponse({
            id: response.id,
            name: response.name,
            description: response.description,
            studentsCount: 0,
          }),
        ],
      }),
    );
  }
}

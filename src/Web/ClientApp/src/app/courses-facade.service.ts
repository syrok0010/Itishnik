import { inject, Injectable } from '@angular/core';
import {
  CourseListResponse,
  CoursesClient,
  CreateCourseCommand,
} from './web-api-client';
import { BehaviorSubject, firstValueFrom } from 'rxjs';
import { map } from 'rxjs/operators';
import { UntilDestroy, untilDestroyed } from '@ngneat/until-destroy';

export interface CoursesState {
  coursesList: CourseListResponse[];
}

let _state: CoursesState = {
  coursesList: [],
};

@UntilDestroy()
@Injectable({
  providedIn: 'root',
})
export class CoursesFacadeService {
  coursesClient = inject(CoursesClient);

  private _store: BehaviorSubject<CoursesState> = new BehaviorSubject(_state);

  coursesList$ = this._store.pipe(map((state) => state.coursesList));

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

  async createCourse(name: string): Promise<void> {
    const response = await firstValueFrom(
      this.coursesClient.createCourseRequest(
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

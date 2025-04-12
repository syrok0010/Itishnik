import { Injectable } from '@angular/core';
import { CourseListResponse } from './web-api-client';
import { BehaviorSubject } from 'rxjs';
import { map } from 'rxjs/operators';

export interface CoursesState {
  coursesList: CourseListResponse[];
}

const _state: CoursesState = {
  coursesList: [
    new CourseListResponse({
      id: '',
      name: 'Some course name',
      studentsCount: 15,
      description: 'Very long description',
    }),
  ],
};

@Injectable({
  providedIn: 'root',
})
export class CoursesFacadeService {
  private _store: BehaviorSubject<CoursesState> = new BehaviorSubject(_state);

  coursesList$ = this._store.pipe(map((state) => state.coursesList));

  createCourse(name: string): void {}
}

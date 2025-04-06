import { Injectable } from '@angular/core';
import { Course, Student } from './web-api-client';
import { BehaviorSubject } from 'rxjs';
import { map } from 'rxjs/operators';

export interface CoursesState {
  coursesList: Course[];
}

const _state: CoursesState = {
  coursesList: [
    new Course({
      name: 'Course 1',
      description: 'Course 1',
      taskBlocks: [],
      students: [
        new Student({
          educationStartYear: 2021,
          educationalProgram: 'ПМИ',
          groupNumber: 2,
        }),
        new Student({
          educationStartYear: 2020,
          educationalProgram: 'БИ',
          groupNumber: 1,
        }),
        new Student({
          educationStartYear: 2022,
          educationalProgram: 'ПИ',
          groupNumber: 3,
        }),
        new Student({
          educationStartYear: 2023,
          educationalProgram: 'ПИ',
          groupNumber: 2,
        }),
        new Student({
          educationStartYear: 2020,
          educationalProgram: 'ПИ',
          groupNumber: 2,
        }),
        new Student({
          educationStartYear: 2022,
          educationalProgram: 'ПИ',
          groupNumber: 2,
        }),
      ],
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

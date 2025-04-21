import {
  ChangeDetectionStrategy,
  Component,
  inject,
  OnInit,
} from '@angular/core';
import { CoursesFacadeService } from '../../courses-facade.service';
import { AsyncPipe } from '@angular/common';
import { ActivatedRoute } from '@angular/router';

@Component({
  selector: 'app-course-page',
  imports: [AsyncPipe],
  templateUrl: './course-page.component.html',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export default class CoursePageComponent implements OnInit {
  coursesFacade = inject(CoursesFacadeService);
  route = inject(ActivatedRoute);
  currentCourse$ = this.coursesFacade.currentCourse$;

  ngOnInit(): void {
    this.coursesFacade.setCurrentCourseId(this.route.snapshot.params['id']);
  }
}

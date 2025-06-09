import { inject, Injectable } from '@angular/core';
import { CreateTagCommand, Tag, TasksClient } from '../web-api-client';
import { UntilDestroy, untilDestroyed } from '@ngneat/until-destroy';
import { BehaviorSubject, firstValueFrom } from 'rxjs';
import { map } from 'rxjs/operators';

export interface TagsState {
  tags: Tag[];
}

let _state: TagsState = {
  tags: [],
};

@UntilDestroy()
@Injectable({
  providedIn: 'root',
})
export class TagsFacadeService {
  private readonly tasksClient = inject(TasksClient);

  private _store: BehaviorSubject<TagsState> = new BehaviorSubject(_state);

  allTags$ = this._store.pipe(map((state) => state.tags));

  constructor() {
    this.tasksClient
      .getTagList()
      .pipe(untilDestroyed(this))
      .subscribe((tags) => this._store.next((_state = { ..._state, tags })));
  }

  async createTag(tag: string) {
    const newTag = await firstValueFrom(
      this.tasksClient.createTag(new CreateTagCommand({ text: tag })),
    );

    this._store.next((_state = { ..._state, tags: [..._state.tags, newTag] }));
  }
}

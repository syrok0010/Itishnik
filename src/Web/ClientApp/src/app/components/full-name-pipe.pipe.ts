import { Pipe, PipeTransform } from '@angular/core';

interface HasFio {
  surname: string;
  name: string;
  patronymic: string;
}

@Pipe({
  name: 'fullName',
})
export class FullNamePipe implements PipeTransform {
  transform(person: HasFio | null | undefined): string {
    return !person
      ? ''
      : `${person.surname} ${person.name} ${person.patronymic || ''}`.trim();
  }
}

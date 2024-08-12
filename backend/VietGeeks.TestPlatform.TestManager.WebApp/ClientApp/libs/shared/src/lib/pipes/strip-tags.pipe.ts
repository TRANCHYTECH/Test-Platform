import { Pipe, PipeTransform } from '@angular/core';
import { stripHtml } from 'string-strip-html';

@Pipe({
  name: 'stripTags'
})
export class StripTagsPipe implements PipeTransform {

  transform(value: string): string {
    if (value === undefined || value === '') {
      return '';
    }

    return stripHtml(value).result;
  }
}

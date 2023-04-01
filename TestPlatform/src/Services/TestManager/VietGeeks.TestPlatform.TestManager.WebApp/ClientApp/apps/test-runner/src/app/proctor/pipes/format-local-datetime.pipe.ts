import { Pipe, PipeTransform } from '@angular/core';

@Pipe({
  name: 'formatLocalDateTime'
})
export class FormatLocalDateTimePipe implements PipeTransform {

  transform(date: Date | undefined): string {
    return date?.toLocaleString() ?? '';
  }
}

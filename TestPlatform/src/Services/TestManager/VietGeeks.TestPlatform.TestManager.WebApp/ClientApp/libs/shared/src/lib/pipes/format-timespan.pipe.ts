import { Pipe, PipeTransform } from '@angular/core';
import { padStart } from 'lodash-es';
import { TimeSpanV1 } from '../models/timespan.model';

@Pipe({
  name: 'formatTimeSpan',
  standalone: true
})
export class FormatTimespanPipe implements PipeTransform {

  transform(time: TimeSpanV1 | undefined): string {
    if (!time) {
      return '';
    }

    const tryAddLeadingZero = (value?: number) => padStart((value ?? 0).toString(), 2, '0');

    return `${tryAddLeadingZero(time.hours)}:${tryAddLeadingZero(time.minutes)}:${tryAddLeadingZero(time.seconds)}`;
  }
}

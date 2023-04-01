import { Pipe, PipeTransform } from '@angular/core';
import { TimeSpan } from '../../api/models';
import padStart from 'lodash-es/padStart';

@Pipe({
  name: 'formatTimeSpan'
})
export class FormatTimespanPipe implements PipeTransform {

  transform(time: TimeSpan | undefined): string {
    if (!time) {
      return '';
    }

    const tryAddLeadingZero = (value?: number) => padStart((value ?? 0).toString(), 2, '0');

    return `${tryAddLeadingZero(time.hours)}:${tryAddLeadingZero(time.minutes)}:${tryAddLeadingZero(time.seconds)}`;
  }
}

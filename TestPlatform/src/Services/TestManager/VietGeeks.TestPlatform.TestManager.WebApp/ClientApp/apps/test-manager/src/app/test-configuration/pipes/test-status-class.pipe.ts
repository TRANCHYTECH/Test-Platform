import { Pipe, PipeTransform } from '@angular/core';
import { TestStatus } from '../state/test.model';


@Pipe({
  name: 'testStatusClass'
})
export class TestStatusClassPipe implements PipeTransform {
  transform(status: TestStatus | null, prefix?: string) {
    let patternColor = '';
    prefix = prefix === undefined ? '' : prefix + '-';
    switch (status || TestStatus.Draft) {
      case TestStatus.Draft:
        patternColor = 'info';
        break;
      case TestStatus.Activated:
        patternColor = 'primary';
        break;
      case TestStatus.Scheduled:
        patternColor = 'secondary';
        break;
      case TestStatus.Ended:
        patternColor = 'success';
        break;
    }

    return `${prefix}${patternColor}`;
  }
}

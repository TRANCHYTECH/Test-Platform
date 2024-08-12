import { Pipe, PipeTransform } from '@angular/core';
import { TestStatus } from '../models/test.model';

@Pipe({
  name: 'testStatus'
})
export class TestStatusPipe implements PipeTransform {

  transform(status: number | null, suffix: string = 'testStatuses.') {
    return suffix + TestStatus[status || TestStatus.Draft];
  }
}



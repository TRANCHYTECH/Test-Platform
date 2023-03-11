import { Pipe, PipeTransform } from '@angular/core';
import { TestStatus } from '../state/test.model';

@Pipe({
  name: 'testStatus'
})
export class TestStatusPipe implements PipeTransform {

  transform(status: TestStatus | null): unknown {
    return TestStatus[status || TestStatus.Draft];
  }
}

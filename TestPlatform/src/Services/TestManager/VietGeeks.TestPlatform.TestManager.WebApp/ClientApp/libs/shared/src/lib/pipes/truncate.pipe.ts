import { Pipe, PipeTransform } from '@angular/core';
import { truncate } from "string-truncator";

@Pipe({
  name: 'truncate'
})
export class TruncatePipe implements PipeTransform {

  transform(value: string): string {
    if (value === undefined || value === '') {
      return '';
    }

    const truncateResult = truncate(value, {maxLines: 1, maxLen: 70});
    
    return `${truncateResult.result}${truncateResult.addEllipsis ? '...': ''}`
  }
}

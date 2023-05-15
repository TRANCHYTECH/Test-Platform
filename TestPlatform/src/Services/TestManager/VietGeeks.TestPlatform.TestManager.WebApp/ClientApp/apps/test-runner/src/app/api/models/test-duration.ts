
import { TestDurationMethodType } from './test-duration-method-type';
import { TimeSpan } from './time-span';
export interface TestDuration {
  duration?: TimeSpan;
  method?: TestDurationMethodType;
  totalDuration: TimeSpan;
}

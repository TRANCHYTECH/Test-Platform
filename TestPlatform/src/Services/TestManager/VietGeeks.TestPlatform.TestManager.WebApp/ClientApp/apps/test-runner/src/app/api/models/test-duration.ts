/* tslint:disable */
/* eslint-disable */
import { TestDurationMethod } from '../../state/test-session.model';
import { TimeSpan } from './time-span';
export interface TestDuration {
  duration?: TimeSpan;
  method?: TestDurationMethod;
}

import { TestStatus } from '../../_state/test-support.model';

export interface TestOverview {
    id: string;
    name: string;
    description: string;
    createdOn: string;
    status: TestStatus;
    category: string;
  }
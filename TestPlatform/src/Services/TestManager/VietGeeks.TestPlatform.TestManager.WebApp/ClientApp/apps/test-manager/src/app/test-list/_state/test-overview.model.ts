import { TestStatus } from "@viet-geeks/test-manager/state";

export interface TestOverview {
    id: string;
    name: string;
    description: string;
    createdOn: string;
    status: TestStatus;
    category: string;
  }
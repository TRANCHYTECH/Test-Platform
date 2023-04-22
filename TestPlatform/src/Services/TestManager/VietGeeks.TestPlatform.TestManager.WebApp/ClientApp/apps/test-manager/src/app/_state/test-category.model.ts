export interface TestCategory {
  id: string;
  name: string;
}

export const TestCategoryUncategorizedId = "000000000000000000000001";

export function createTestCategory(params: Partial<TestCategory>) {
  return params as TestCategory;
}

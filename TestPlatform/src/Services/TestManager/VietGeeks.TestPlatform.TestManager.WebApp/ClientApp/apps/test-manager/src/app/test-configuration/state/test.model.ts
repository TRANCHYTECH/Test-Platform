
export interface Test {
  id: string;
  basicSetting: TestBasicSetting;
}

export interface TestBasicSetting {
  name: string;
  category: string;
  description: string;
}

export function createTest(params: Partial<Test>) {
  return {
    id: params.id,
    basicSetting: params.basicSetting
  } as Test;
}

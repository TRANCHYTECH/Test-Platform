
export interface Test {
  id: string;
  basicSettings: BasicSettings;
  createdOn: Date;
}

export interface BasicSettings {
  name: string;
  category: string;
  description: string;
}


export function createTest(params: Partial<Test>) {
  return params as Test;
}

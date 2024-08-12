export interface QuestionCategory {
  id: string;
  name: string;
}

export const QuestionCategoryGenericId = "000000000000000000000001";

export function createQuestionCategory(params: Partial<QuestionCategory>) {
  return params as QuestionCategory;
}

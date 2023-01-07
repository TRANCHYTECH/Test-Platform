const question = {
  id: 1,
  questionNo: 1,
  questionDefinition: "What is the current officially recognized elevation of Mount Everest?",
  categoryId: "Geography",
  categoryName: "Geography",
  categoryColor: "Geography",
  answerType: 1,
  answerTypeName: "Single choice",
  answers: [
  {
    answerDescription: "8488 m",
    answerPoint: 2,
    isCorrect: true
  },
  {
    answerDescription: "8466 m",
    answerPoint: 2,
  },
  {
    answerDescription: "8844 m",
    answerPoint: 2,
  }],
  scoreSettings: {
    correctPoint: 1,
    incorrectPoint: 1,
    isPartialAnswersEnabled: false,
    maxPoints: 1,
    maxWords: 2
  },
  isMandatory: true,
  createdDate: '2022-12-20T00:00:00',
  lastModifiedDate: '2022-12-20T00:00:00'
};

const answerTypes = [
  {
    id: 1,
    text: 'Single choice'
  },
  {
    id: 2,
    text: 'Multiple choices'
  },
  {
    id: 3,
    text: 'Descriptive'
  },
  {
    id: 4,
    text: 'True/False'
  },
  {
    id: 5,
    text: 'Short Answer'
  }
];

export { question as QuestionData,answerTypes as AnswerTypes };

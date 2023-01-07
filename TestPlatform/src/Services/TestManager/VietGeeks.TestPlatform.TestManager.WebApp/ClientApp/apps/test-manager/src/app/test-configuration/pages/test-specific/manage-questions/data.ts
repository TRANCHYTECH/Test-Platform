const questions = [
  {
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
  },
  {
    id: 2,
    questionNo: 2,
    questionDefinition: "Select all tributaries of the Hudson River:",
    categoryId: "River",
    categoryName: "River",
    categoryColor: "River",
    answerType: 2,
    answerTypeName: "Multiple choices",
    answers: [
    {
      answerDescription: "Croton River",
      answerPoint: 2,
      isCorrect: true,
    },
    {
      answerDescription: "Maid River",
      answerPoint: 2,
    },
    {
      answerDescription: "Kinderhook Creek",
      answerPoint: 2,
      isCorrect: true,
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
  }
];

export { questions };

export const TestStartConfig = {
  name: 'Example: History test',
  totalQuestions: 6,
  totalMinutes: 60,
  instruction: `Hello! <br/>
    This test consists of 6 questions. You have 60 minutes to solve it. <br/>
    Make sure you have enough time and then start the test.`,
  honestRespondentTitle: 'Focus on your test only!',
  honestRespondentContent: `The test is secured with Honest Respondent Technology. <br/>
  Don't click outside the test tab area. Every browser tab movement is recorded. <br/>
  We recommend disabling background programs, chats and system notifications before the test, as they can trigger a test block.`,
  respondentIdentifyFields: [
    {
      fieldId: 'firstName',
      fieldLabel: 'First name',
      isRequired: false
    },
    {
      fieldId: 'lastName',
      fieldLabel: 'Last name',
      isRequired: true
    }
  ]
};

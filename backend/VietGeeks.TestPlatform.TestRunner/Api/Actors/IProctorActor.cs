using Dapr.Actors;
using VietGeeks.TestPlatform.TestRunner.Contract;
using VietGeeks.TestPlatform.TestRunner.Contract.ProctorExamActor;

namespace VietGeeks.TestPlatform.TestRunner.Api.Actors;

public interface IProctorActor : IActor
{
    Task<string> ProvideExamineeInfo(ProvideExamineeInfoInput input);

    Task<StartExamOutput> StartExam(StartExamInput input);

    Task<SubmitAnswerOutput> SubmitAnswer(SubmitAnswerInput input);

    Task<FinishExamOutput> FinishExam();
    Task<ExamStatus> GetExamStatus();
    Task<ActivateQuestionOutput> ActivateQuestionFromCurrent(ActivateNextQuestionInput input);
    Task<ActivateQuestionOutput> ActivateQuestionByIndex(int nextQuestionIndex);
}
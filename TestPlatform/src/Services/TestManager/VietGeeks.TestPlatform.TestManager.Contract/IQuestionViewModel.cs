using System.Collections.Generic;

namespace VietGeeks.TestPlatform.TestManager.Contract
{
    public interface IQuestionViewModel
    {
        IEnumerable<AnswerViewModel> Answers { get; set; }
        AnswerType AnswerType { get; set; }
        string CategoryId { get; set; }
        string Description { get; set; }
        int QuestionNo { get; set; }
        ScoreSettingsViewModel ScoreSettings { get; set; }
    }
}
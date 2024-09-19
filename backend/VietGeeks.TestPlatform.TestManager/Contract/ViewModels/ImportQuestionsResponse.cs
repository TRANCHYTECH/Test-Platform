namespace VietGeeks.TestPlatform.TestManager.Contract.ViewModels;

public class QuestionResponse
{
    public required IList<Question> Questions { get; set; }
}

public class Question
{
    public required string Description { get; set; }
    public required IList<Answer> Answers { get; set; }
}

public class Answer
{
    public required string Text { get; set; }
    public bool Correct { get; set; }
}

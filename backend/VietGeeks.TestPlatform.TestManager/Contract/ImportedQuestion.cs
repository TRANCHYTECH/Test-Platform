namespace VietGeeks.TestPlatform.TestManager.Contract;

public class ImportedAnswer
{
    public required string Text { get; init; }
    public bool IsCorrect { get; init; }
}

public class ImportedQuestion
{
    public required string Text { get; set; }
    public required IList<ImportedAnswer> Answers { get; set; }
}
using MongoDB.Entities;

namespace VietGeeks.TestPlatform.TestManager.Data.Models;

[Collection("Category")]
public class QuestionCategory : CategoryBase
{
    public string TestId { get; set; } = default!;

    public static QuestionCategory Generic()
    {
        return new QuestionCategory
        {
            ID = UncategorizedId,
            Name = nameof(Generic),
            DisplayOrder = -1
        };
    }
}
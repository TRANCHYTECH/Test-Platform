using MongoDB.Entities;

namespace VietGeeks.TestPlatform.TestManager.Core.Models;

[Collection("Category")]
public class TestCategory : CategoryBase
{
    public static TestCategory Uncategorized()
    {
        return new()
        {
            ID = CategoryBase.UncategorizedId,
            Name = nameof(Uncategorized),
            DisplayOrder = -1,
            IsSystem = true
        };
    }
}

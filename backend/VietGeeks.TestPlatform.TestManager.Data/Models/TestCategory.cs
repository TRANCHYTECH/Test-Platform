using MongoDB.Entities;

namespace VietGeeks.TestPlatform.TestManager.Data.Models
{
    [Collection("Category")]
    public class TestCategory : CategoryBase
    {
        public bool IsSystem { get; set; }

        public static TestCategory Uncategorized()
        {
            return new TestCategory
            {
                ID = UncategorizedId,
                Name = nameof(Uncategorized),
                DisplayOrder = -1,
                IsSystem = true
            };
        }
    }
}
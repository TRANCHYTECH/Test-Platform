using MongoDB.Entities;

namespace VietGeeks.TestPlatform.TestManager.Core.Models
{
    [Collection("Tests")]
    public class MyTest : EntityBase
    {
        public string TestName { get; set; } = default!;

        public string CategoryId { get; set; } = default!;

        public string LanguageId { get; set; } = default!;

        public string Description { get; set; } = default!;
    }
}


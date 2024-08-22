using MongoDB.Bson.Serialization.Attributes;

namespace VietGeeks.TestPlatform.TestManager.Data.Models;

[BsonDiscriminator("Category", RootClass = true)]
[BsonKnownTypes(typeof(TestCategory), typeof(QuestionCategory))]
public abstract class CategoryBase : EntityBase
{
    public static readonly string UncategorizedId = "000000000000000000000001";
    public string Name { get; set; } = default!;

    public int DisplayOrder { get; set; }
}
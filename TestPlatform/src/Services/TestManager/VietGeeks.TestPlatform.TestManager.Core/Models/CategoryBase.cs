using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Entities;

namespace VietGeeks.TestPlatform.TestManager.Core.Models;

[BsonDiscriminator("Category", RootClass = true)]
[BsonKnownTypes(typeof(TestCategory), typeof(QuestionCategory))]
public abstract class CategoryBase : EntityBase
{
    public string Name { get; set; } = default!;
}
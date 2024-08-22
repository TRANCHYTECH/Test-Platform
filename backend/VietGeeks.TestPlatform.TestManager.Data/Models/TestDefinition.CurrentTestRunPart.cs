using MongoDB.Bson.Serialization.Attributes;

namespace VietGeeks.TestPlatform.TestManager.Data.Models;

public class CurrentTestRunPart
{
    public string Id { get; set; } = default!;

    [BsonDateTimeOptions(Kind = DateTimeKind.Utc)]
    public DateTime ActivatedOrScheduledAtUtc { get; set; }
}
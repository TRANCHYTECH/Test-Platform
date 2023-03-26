using MongoDB.Bson.Serialization.Attributes;
using System;
namespace VietGeeks.TestPlatform.TestManager.Core.Models
{
    public class CurrentTestRunPart
    {
        public string Id { get; set; } = default!;

        [BsonDateTimeOptions(Kind = DateTimeKind.Utc)]
        public DateTime ActivatedOrScheduledAtUtc { get; set; }
    }
}


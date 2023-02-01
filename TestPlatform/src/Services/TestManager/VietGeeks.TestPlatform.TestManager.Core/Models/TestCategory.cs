using MongoDB.Entities;

namespace VietGeeks.TestPlatform.TestManager.Core.Models;

[Collection("TestCategory")]
public class TestCategory : EntityBase
{
    public string Name { get; set; } = default!;
}


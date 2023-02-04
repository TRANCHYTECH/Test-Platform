﻿using MongoDB.Entities;

namespace VietGeeks.TestPlatform.TestManager.Core.Models;

[Collection("Category")]
public class TestCategory : EntityBase
{
    public string Name { get; set; } = default!;
}

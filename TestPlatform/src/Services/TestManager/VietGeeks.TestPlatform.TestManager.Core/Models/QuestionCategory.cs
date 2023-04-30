using System;
using MongoDB.Entities;

namespace VietGeeks.TestPlatform.TestManager.Core.Models;

[Collection("Category")]
public class QuestionCategory : CategoryBase
{
    public string TestId { get; set; } = default!;

    public static QuestionCategory Generic()
    {
        return new()
        {
            ID = CategoryBase.UncategorizedId,
            Name = nameof(Generic),
            DisplayOrder = -1
        };
    }
}


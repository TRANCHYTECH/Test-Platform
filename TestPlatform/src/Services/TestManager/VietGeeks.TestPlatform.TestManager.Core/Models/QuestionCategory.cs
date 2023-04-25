using System;
using MongoDB.Entities;

namespace VietGeeks.TestPlatform.TestManager.Core.Models;

[Collection("Category")]
public class QuestionCategory : CategoryBase
{
    public static QuestionCategory Generic()
    {
        return new()
        {
            ID = CategoryBase.UncategorizedId,
            Name = nameof(Generic),
            DisplayOrder = -1,
            IsSystem = true
        };
    }
}


﻿using System;
using MongoDB.Entities;

namespace VietGeeks.TestPlatform.TestManager.Core.Models;

[Collection("Category")]
public class QuestionCategory : EntityBase
{
    public string Name { get; set; } = default!;
}

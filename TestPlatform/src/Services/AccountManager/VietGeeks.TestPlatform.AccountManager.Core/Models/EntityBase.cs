﻿using System;
using MongoDB.Entities;

namespace VietGeeks.TestPlatform.AccountManager.Core.Models
{
    public class EntityBase : Entity, ICreatedOn, IModifiedOn
    {
        public ModifiedBy ModifiedBy { get; set; } = default!;

        public DateTime CreatedOn { get; set; } = DateTime.UtcNow;

        public DateTime ModifiedOn { get; set; }
    }
}

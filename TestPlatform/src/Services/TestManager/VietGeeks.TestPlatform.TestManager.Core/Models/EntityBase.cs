using System;
using MongoDB.Entities;

namespace VietGeeks.TestPlatform.TestManager.Core.Models;

public class EntityBase: Entity, ICreatedOn, IModifiedOn
{
    public ModifiedBy ModifiedBy { get; set; } = default!;

    public DateTime CreatedOn { get; set; }

    public DateTime ModifiedOn { get; set; }
}


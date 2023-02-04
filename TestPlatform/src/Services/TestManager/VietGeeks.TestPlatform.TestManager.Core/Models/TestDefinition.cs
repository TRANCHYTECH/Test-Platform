using System.Collections.Generic;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson.Serialization.Serializers;
using MongoDB.Entities;

namespace VietGeeks.TestPlatform.TestManager.Core.Models;

[Collection("TestDefinition")]
public class TestDefinition: EntityBase
{
    public TestBasicSettingsPart BasicSettings { get; set; } = default!;

    public TimeSettingsPart? TimeSettings { get; set; }

  //  [BsonSerializer(typeof(ImpliedImplementationInterfaceSerializer<ITestSetGenerator, RandomByCategoriesGenerator>))]

    public TestSetSettingsPart? TestSetSettings { get; set; }

    public GradingSettingsPart? GradingSettings { get; set; }

    public string InstructionMessage { get; set; } = default!;

    public bool IsSkipQuestionsEnabled { get; set; } = default!;

    public bool IsActivated { get; set; }

    public bool IsEnabled { get; set; }
}


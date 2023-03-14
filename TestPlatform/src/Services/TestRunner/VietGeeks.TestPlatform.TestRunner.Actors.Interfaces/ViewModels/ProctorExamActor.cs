using System;
using System.Collections.Generic;

namespace VietGeeks.TestPlatform.TestRunner.Actors.Interfaces.ViewModels
{

    public class StartExamInput
    {
        public string ExamId { get; set; } = default!;

        public string TestDefinitionId { get; set; } = default!;

        public Dictionary<string, string[]> Questions { get; set; } = default!;
    }
}


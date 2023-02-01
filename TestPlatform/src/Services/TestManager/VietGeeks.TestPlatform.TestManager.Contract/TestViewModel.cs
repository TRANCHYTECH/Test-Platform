using System;

namespace VietGeeks.TestPlatform.TestManager.Contract;

public class NewTestDefinitionViewModel
{
    public CreateOrUpdateTestBasicSettingsViewModel BasicSettings { get; set; } = default!;
}

public class UpdateTestDefinitionViewModel
{
    public CreateOrUpdateTestBasicSettingsViewModel? BasicSettings { get; set; }
}

public class CreateOrUpdateTestBasicSettingsViewModel
{
    public string Name { get; set; }

    public string Category { get; set; }

    public string? Language { get; set; }

    public string Description { get; set; }
}

public class TestBasicSettingsViewModel
{
    public string Name { get; set; }

    public string Category { get; set; }

    public string? Language { get; set; }

    public string Description { get; set; }
}

public class TestDefinitionViewModel
{
    public string Id { get; set; } = default!;

    public TestBasicSettingsViewModel BasicSettings { get; set; } = default!;

    public DateTime CreatedOn { get; set; }

    public DateTime ModifiedOn { get; set; }
}

using System;

namespace VietGeeks.TestPlatform.TestManager.Contract;

public class TestCategoryViewModel
{
    public string Id { get; set; } = default!;

    public string Name { get; set; } = default!;

    public DateTime CreatedOn { get; set; }

    public DateTime ModifiedOn { get; set; }
}

public class NewTestCategoryViewModel
{
    public string Name { get; set; } = default!;
}
using System;

namespace VietGeeks.TestPlatform.TestManager.Contract;

public class QuestionCategoryViewModel
{
    public string Id { get; set; }

    public string Name { get; set; }

    public DateTime CreatedOn { get; set; }

    public DateTime ModifiedOn { get; set; }

    public int DisplayOrder { get; set; }
}

public class NewQuestionCategoryViewModel
{
    public string Name { get; set; } = default!;
}
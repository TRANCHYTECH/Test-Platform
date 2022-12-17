using System;

namespace VietGeeks.TestPlatform.TestManager.Contract
{
    public class NewTestViewModel
    {
        public string TestName { get; set; }

        public string CategoryId { get; set; }

        public string LanguageId { get; set; }

        public string Description { get; set; }
    }

    public class UpdateTestViewModel
    {

    }

    public class TestViewModel
    {
        public string Id { get; set; }

        public string TestName { get; set; }

        public string CategoryId { get; set; }

        public string LanguageId { get; set; }

        public string Description { get; set; }

        public DateTime CreatedOn { get; set; }

        public DateTime ModifiedOn { get; set; }
    }
}
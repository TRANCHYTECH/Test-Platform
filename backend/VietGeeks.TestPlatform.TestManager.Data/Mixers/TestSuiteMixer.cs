using VietGeeks.TestPlatform.TestManager.Data.Models;

namespace VietGeeks.TestPlatform.TestManager.Data.Mixers
{
    public static class TestSuiteMixer
    {
        public static List<QuestionDefinition> GenerateTestSet(this TestDefinition testDefinition,
            List<QuestionDefinition> questions, string? accessCode)
        {
            // Check test access type.
            var testAccess = testDefinition.TestAccessSettings;
            if (testAccess.AccessType == TestAcessType.PublicLink)
            {
                // Use test sets config
                return GenerateTestSet(testDefinition.TestSetSettings, questions);
            }

            if (testAccess.AccessType == TestAcessType.PrivateAccessCode)
            {
                var settings = testAccess.Settings as PrivateAccessCodeType;
                if (settings == null)
                {
                    throw new Exception("Not Found Private Access Code Settings");
                }

                if (string.IsNullOrWhiteSpace(accessCode))
                {
                    throw new Exception("Access Code Is Not Valid");
                }

                var configForAccessCode = settings.Configs.SingleOrDefault(c => c.Code == accessCode);
                if (configForAccessCode == null)
                {
                    throw new Exception("Not Found Config for this access code");
                }

                if (!string.IsNullOrEmpty(configForAccessCode.SetId))
                {
                    //todo: get generated test set.
                    return questions;
                }

                return GenerateTestSet(testDefinition.TestSetSettings, questions);
            }

            if (testAccess.AccessType == TestAcessType.GroupPassword)
            {
                //todo: impl logic for this case.
                return GenerateTestSet(testDefinition.TestSetSettings, questions);
            }

            throw new NotSupportedException(testAccess.AccessType.ToString());
        }

        private static List<QuestionDefinition> GenerateTestSet(TestSetSettingsPart? testSets,
            List<QuestionDefinition> questions)
        {
            if (testSets == null)
            {
                throw new Exception("Not Found Test Set Settings");
            }

            return testSets.Generator.Generate(questions);
        }
    }
}
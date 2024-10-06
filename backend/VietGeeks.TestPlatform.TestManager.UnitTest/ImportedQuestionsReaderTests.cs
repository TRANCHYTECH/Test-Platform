using VietGeeks.TestPlatform.TestManager.Contract;
using VietGeeks.TestPlatform.TestManager.Infrastructure.Services;

namespace VietGeeks.TestPlatform.TestManager.UnitTest
{
    public class ImportedQuestionsReaderTests
    {
        [Fact]
        public void ReadQuestionsFromStream_ShouldReturnCorrectQuestionsAndAnswers()
        {
            // Arrange
            var expectedQuestions = new List<ImportedQuestion>
            {
                new ImportedQuestion
                {
                    Text = "1. What is the capital of France?",
                    Answers = new List<ImportedAnswer>
                    {
                        new ImportedAnswer { Text = "a. Berlin", IsCorrect = false },
                        new ImportedAnswer { Text = "b. Madrid", IsCorrect = false },
                        new ImportedAnswer { Text = "c. Paris", IsCorrect = true },
                        new ImportedAnswer { Text = "d. Rome", IsCorrect = false }
                    }
                },
                new ImportedQuestion
                {
                    Text = "2. What is 2 + 2?",
                    Answers = new List<ImportedAnswer>
                    {
                        new ImportedAnswer { Text = "a. 3", IsCorrect = false },
                        new ImportedAnswer { Text = "b. 4", IsCorrect = true },
                        new ImportedAnswer { Text = "c. 5", IsCorrect = false },
                        new ImportedAnswer { Text = "d. 6", IsCorrect = false }
                    }
                }
            };

            string filePath = Path.Combine("TestData", "import-questions-2.docx");
            using var fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read);

            // Act
            var result = WordDocumentReader.ReadQuestions(fileStream);

            // Assert
            Assert.Equal(expectedQuestions.Count, result.Count);
            for (int i = 0; i < expectedQuestions.Count; i++)
            {
                Assert.Equal(expectedQuestions[i].Text, result[i].Text);
                Assert.Equal(expectedQuestions[i].Answers.Count, result[i].Answers.Count);
                for (int j = 0; j < expectedQuestions[i].Answers.Count; j++)
                {
                    Assert.Equal(expectedQuestions[i].Answers[j].Text, result[i].Answers[j].Text);
                    Assert.Equal(expectedQuestions[i].Answers[j].IsCorrect, result[i].Answers[j].IsCorrect);
                }
            }
        }
    }
}
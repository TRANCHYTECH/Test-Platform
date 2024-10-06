
using OfficeIMO.Word;
using VietGeeks.TestPlatform.TestManager.Contract;

namespace VietGeeks.TestPlatform.TestManager.Infrastructure.Services;

public static class WordDocumentReader
{
    public static IList<ImportedQuestion> ReadQuestions(Stream stream)
    {
        var questions = new List<ImportedQuestion>();
        using var document = WordDocument.Load(stream, readOnly: true);
        ImportedQuestion? currentQuestion = null;
        foreach (var paragraph in document.Paragraphs)
        {
            var text = paragraph.Text.Trim();
            if (string.IsNullOrEmpty(text))
            {
                currentQuestion = null;
                continue;
            }

            if (currentQuestion == null)
            {
                // New question
                currentQuestion = new ImportedQuestion
                {
                    Text = text,
                    Answers = new List<ImportedAnswer>()
                };
                questions.Add(currentQuestion);
            }
            else
            {
                // Answer
                var isCorrect = paragraph.Bold;
                currentQuestion.Answers.Add(new ImportedAnswer
                {
                    Text = text,
                    IsCorrect = isCorrect
                });
            }
        }

        return questions;
    }
}

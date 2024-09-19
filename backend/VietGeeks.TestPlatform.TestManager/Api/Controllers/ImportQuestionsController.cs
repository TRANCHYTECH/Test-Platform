using Azure.AI.OpenAI;
using Json.Schema;
using Json.Schema.Generation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using OpenAI.Chat;
using System.Text.Json;
using VietGeeks.TestPlatform.TestManager.Contract.ViewModels;

namespace VietGeeks.TestPlatform.TestManager.Api.Controllers;

[ApiController]
[Route("Management/TestDefinition/{testId}/Question")]
[Authorize]
public class ImportQuestionsController(ILogger<ImportQuestionsController> logger) : ControllerBase
{
    private const string ParsingQuestionsSystemPrompt = "you are a text conversion expert. Help convert to json object list";
    private const string ParsingQuestionsUserPrompt = """
        Parse list of questions in this passage:
        ```
        {0}
        ```
        Each question begins with a number followed by a period. For example: 1. or 2.
        Questions are separated by a newline.
        Below each question are the answers, which begin with a letter and a period. For example: a., b. or c.
        It's possible to have multiple corrected answer. Each correct answer is marked with a pair of square brackets. For example: [a].
        """;

    [HttpPost(":ReadFile")]
    public async Task<IActionResult> ReadFile([FromServices] AzureOpenAIClient openAIClient, [FromServices] IConfiguration configuration, IFormFile file, CancellationToken cancellation)
    {
        try
        {
            //todo: max file size, max request token allowed
            const int maxSize = 1024 * 1024 * 1;
            await using var fileContent = file.OpenReadStream();
            if (fileContent.Length > maxSize)
            {
                return BadRequest();
            }

            using var reader = new StreamReader(fileContent);
            var textContent = await reader.ReadToEndAsync(cancellation);

            var responseSchemaBuilder = new JsonSchemaBuilder();
            responseSchemaBuilder.FromType<QuestionResponse>();
            responseSchemaBuilder.AdditionalProperties(false);
            var jsonSchema = JsonSerializer.Serialize(responseSchemaBuilder.Build());

            using var openAiRequestCancellationSource = CancellationTokenSource.CreateLinkedTokenSource(cancellation, new CancellationTokenSource(TimeSpan.FromSeconds(15)).Token);
            var completionResult = await openAIClient.GetChatClient(configuration.GetValue<string>("OpenAi:Deployment")).CompleteChatAsync(
                [
                    new SystemChatMessage(ParsingQuestionsSystemPrompt),
                new UserChatMessage(string.Format(ParsingQuestionsUserPrompt, textContent)),
            ],
                new()
                {
                    ResponseFormat = ChatResponseFormat.CreateJsonSchemaFormat("parsing_questions", BinaryData.FromString(jsonSchema), strictSchemaEnabled: false),
                },
                cancellationToken: openAiRequestCancellationSource.Token);

            var response = JsonSerializer.Deserialize<QuestionResponse>(completionResult.Value.Content[0]!.Text);

            return Ok(response?.Questions);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }
}

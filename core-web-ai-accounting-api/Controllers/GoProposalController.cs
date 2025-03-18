using Microsoft.AspNetCore.Mvc;
using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.SemanticKernel.Connectors.OpenAI;
using Microsoft.SemanticKernel;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json;

namespace core_web_ai_accounting_api.Controllers;

[ApiController]
[Route("[controller]")]
public class GoProposalController : ControllerBase
{
    private readonly Kernel _kernel;

    private readonly ILogger<GoProposalController> _logger;

    public GoProposalController(ILogger<GoProposalController> logger, Kernel kernel)
    {
        _logger = logger;
        _kernel = kernel ?? throw new ArgumentNullException(nameof(kernel));
    }

    [HttpGet]
    [Experimental("SKEXP0001")]
    public async Task<ActionResult> Test()
    {
        var chatCompletionService = _kernel.GetRequiredService<IChatCompletionService>();

        ChatHistory history = new ChatHistory();

        // Add system message
        history.Add(
            new ChatMessageContent
            {
                Role = AuthorRole.System,
                Content = System.IO.File.ReadAllText("Support/Context.txt")
            }
        );

        history.AddUserMessage(System.IO.File.ReadAllText("Support/UserMessage.txt"));

        // Enable planning
        OpenAIPromptExecutionSettings openAIPromptExecutionSettings = new()
        {
            FunctionChoiceBehavior = FunctionChoiceBehavior.Auto()
        };

        var response = await chatCompletionService.GetChatMessageContentAsync(
            history,
            executionSettings: openAIPromptExecutionSettings,
            kernel: _kernel
        );

        var options = new JsonSerializerOptions { WriteIndented = true };
        _logger.LogInformation(JsonSerializer.Serialize(history, options));
        return Ok(response);
    }
}

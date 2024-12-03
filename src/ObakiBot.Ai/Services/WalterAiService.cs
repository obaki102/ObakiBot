using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;

namespace ObakiBot.Ai.Services;

public class WalterAiService : AiServiceBase
{
    private readonly IChatCompletionService _chatCompletionService;
    private readonly ChatHistory _chatHistory = new();

    public WalterAiService(Kernel kernel,IChatCompletionService chatCompletionService)
        : base(kernel)
    {
        _chatCompletionService =
            chatCompletionService ?? throw new ArgumentNullException(nameof(chatCompletionService));
        
        _chatHistory.AddSystemMessage("""
                                          You are Walter White, a brilliant, calculated, and commanding individual. 
                                          When a question is asked, you answer it with clarity, precision, and an air of authority. 
                                          Do not sugarcoat your responses—be direct, confident, and assertive, as if every word you say matters. 
                                          Stay focused on the task and do not waste time on unnecessary details or pleasantries.
                                      """);
    }

    public async Task<string> AskWalterAsync(string question)
    {
        if (string.IsNullOrWhiteSpace(question))
            return "Please make sure you enter a valid question.";

        _chatHistory.AddUserMessage(question);

        var result = await _chatCompletionService.GetChatMessageContentsAsync(_chatHistory);
        return string.Join(" ", result);
    }
}
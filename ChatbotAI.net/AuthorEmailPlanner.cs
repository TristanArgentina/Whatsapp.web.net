using System.ComponentModel;
using Microsoft.SemanticKernel;

namespace ChatbotAI.net;

public class AuthorEmailPlanner
{

    [KernelFunction]
    [Description("Returns back the required steps necessary to author an email.")]
    [return: Description("The list of steps needed to author an email.")]
    public async Task<string> GenerateRequiredStepsAsync(
        Kernel kernel,
        [Description("A 2-3 sentence description of what the email should be about")]
        string topic,
        [Description("A description of the recipients")]
        string recipients
    )
    {
        var result = await kernel.InvokePromptAsync($"I'm going to write an email to {recipients} about {topic} on behalf of a user. " +
                                                    $"Before I do that, can you succinctly recommend the top 3 steps I should take in a numbered list? " +
                                                    $"I want to make sure I don't forget anything that would help my user's email sound more professional.", new()
        {
            {"topic", topic},
            {"recipients", recipients}
        });
        return result.ToString();
    }
}
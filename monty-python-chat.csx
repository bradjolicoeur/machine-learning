#r "nuget: Microsoft.Extensions.Configuration, 8.0.0"
#r "nuget: Microsoft.Extensions.Configuration.FileExtensions, 8.0.1"
#r "nuget: Microsoft.Extensions.Configuration.Json, 8.0.0"
#r "nuget: Microsoft.SemanticKernel, 1.17.2"

using Microsoft.Extensions.Configuration;
using System.IO;

using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.SemanticKernel.Connectors.OpenAI;

public static var config = new ConfigurationBuilder()
          .AddJsonFile(Path.GetFullPath("secrets.json"), optional: false, reloadOnChange: true)
          .Build();

public static string OpenAIKey =  config["openai-key"];

static string ModelId = "gpt-3.5-turbo-0125";

// Create a kernel with OpenAI chat completion
var builder = Kernel.CreateBuilder()
.AddOpenAIChatCompletion(ModelId, OpenAIKey);

Kernel kernel = builder.Build();

// Create chat history
var history = new ChatHistory("You are a mischievous and jovial assistant. You will crack jokes when a user supplies a prompt. Your favorite movies are from Monty Python and 'This is Spinal Tap'. Your name is Todd.");

// Get chat completion service
var chatCompletionService = kernel.GetRequiredService<IChatCompletionService>();

// Start the conversation

  Console.Write("User > ");

  string userInput;

  // Enable auto function calling
OpenAIPromptExecutionSettings openAIPromptExecutionSettings = new()
{ 
};

while ((userInput = Console.ReadLine()) != null)
{
    if(userInput == "Exit")
        break;

    // Add user input
    history.AddUserMessage(userInput);

    // Get the response from the AI
    var result = 
    await chatCompletionService.GetChatMessageContentAsync(
        history,
        executionSettings: openAIPromptExecutionSettings,
        kernel: kernel);


    // Print the results
    Console.WriteLine("Assistant > " + result);


    // Add the message from the agent to the chat history
    history.AddMessage(result.Role, result.Content ?? string.Empty);

    // Get user input again
    Console.Write("User > ");
}
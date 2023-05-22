namespace OpenAIConnector
{
    using Azure;
    using Azure.AI.OpenAI;
    public class ChatUnitTests
    {

        private ClientBuilder? clientBuilder;
        private Chat? chatCompletion;
        public ChatUnitTests(ClientBuilder clientBuilder)
        {
            this.clientBuilder = clientBuilder;
            this.chatCompletion = new(clientBuilder);
        }

        public async Task UnitTest1(string deploymentOrModelName)
        {
            if (chatCompletion is null) throw new System.Exception("Client not initialized");
            var chatCompletionsOptions = new ChatCompletionsOptions()
            {
                Messages =
                {
                    new ChatMessage(ChatRole.System, "You are a helpful assistant."),
                    new ChatMessage(ChatRole.User, "Does Azure OpenAI support customer managed keys?"),
                    new ChatMessage(ChatRole.Assistant, "Yes, customer managed keys are supported by Azure OpenAI."),
                    new ChatMessage(ChatRole.User, "Do other Azure Cognitive Services support this too?"),
                },
                MaxTokens = 100
            };

            StreamingChatCompletions streamingChatCompletions = await chatCompletion.GetChatCompletionsStreamingAsync(
                deploymentOrModelName,
                chatCompletionsOptions);

            await foreach (StreamingChatChoice choice in streamingChatCompletions.GetChoicesStreaming())
            {
                await foreach (ChatMessage message in choice.GetMessageStreaming())
                {
                    Console.Write(message.Content);
                }
                Console.WriteLine();
            }            
        }
    }
}
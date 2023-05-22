namespace OpenAIConnector
{
    using Azure;
    using Azure.AI.OpenAI;
    public class Chat
    {
        private ClientBuilder? clientBuilder;

        public Chat(ClientBuilder clientBuilder)
        {
            this.clientBuilder = clientBuilder;
        }

        public async Task<StreamingChatCompletions> GetChatCompletionsStreamingAsync(string deploymentOrModelName, ChatCompletionsOptions chatCompletionsOptions)
        {
            if (clientBuilder == null)
                throw new System.Exception("Client not initialized");
            Response<StreamingChatCompletions> streamingChatCompletionsResponse =  await clientBuilder.GetClient().GetChatCompletionsStreamingAsync(deploymentOrModelName, chatCompletionsOptions);
            return streamingChatCompletionsResponse.Value;
        }

        public StreamingChatCompletions GetChatCompletionsStreaming(string deploymentOrModelName, ChatCompletionsOptions chatCompletionsOptions)
        {
            if (clientBuilder == null)
                throw new System.Exception("Client not initialized");
            return clientBuilder.GetClient().GetChatCompletionsStreaming(deploymentOrModelName, chatCompletionsOptions).Value;
        }

    }
}
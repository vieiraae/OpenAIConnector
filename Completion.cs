namespace OpenAIConnector
{
    using Azure;
    using Azure.AI.OpenAI;
    public class Completion
    {
        private ClientBuilder? clientBuilder;

        public Completion(ClientBuilder clientBuilder)
        {
            this.clientBuilder = clientBuilder;
        }
        public async Task<Response<Completions>> GetCompletionsAsync(string deploymentOrModelName, string prompt)
        {
            if (clientBuilder == null)
                throw new System.Exception("Client not initialized");
            return await clientBuilder.GetClient().GetCompletionsAsync(deploymentOrModelName, prompt);
        }

        public Response<Completions> GetCompletions(string deploymentOrModelName, string prompt)
        {
            if (clientBuilder == null)
                throw new System.Exception("Client not initialized");
            return clientBuilder.GetClient().GetCompletions(deploymentOrModelName, prompt);
        }

        public async Task<Response<Completions>> GetCompletionsAsync(string deploymentOrModelName, CompletionsOptions completionsOptions)
        {
            if (clientBuilder == null)
                throw new System.Exception("Client not initialized");
            return await clientBuilder.GetClient().GetCompletionsAsync(deploymentOrModelName, completionsOptions);
        }

        public Response<Completions> GetCompletions(string deploymentOrModelName, CompletionsOptions completionsOptions)
        {
            if (clientBuilder == null)
                throw new System.Exception("Client not initialized");
            return clientBuilder.GetClient().GetCompletions(deploymentOrModelName, completionsOptions);
        }


    }
}

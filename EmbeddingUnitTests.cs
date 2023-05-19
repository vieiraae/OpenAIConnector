namespace OpenAIConnector
{
    using Azure;
    using Azure.AI.OpenAI;
    public class EmbeddingUnitTests
    {

        private ClientBuilder? clientBuilder;
        private Completion? completion;
        public EmbeddingUnitTests(ClientBuilder clientBuilder)
        {
            this.clientBuilder = clientBuilder;
            this.completion = new(clientBuilder);
        }

    }
}
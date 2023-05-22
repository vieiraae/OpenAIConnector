namespace OpenAIConnector
{
    using Azure;
    using Azure.AI.OpenAI;
    public class Embedding
    {
        private ClientBuilder? clientBuilder;

        public Embedding(ClientBuilder clientBuilder)
        {
            this.clientBuilder = clientBuilder;
        }
        

        public async Task<Response<Embeddings>> GetEmbeddingsAsync(string deploymentOrModelName, EmbeddingsOptions embeddingsOptions)
        {
            if (clientBuilder == null)
                throw new System.Exception("Client not initialized");
            return await clientBuilder.GetClient().GetEmbeddingsAsync(deploymentOrModelName, embeddingsOptions);
        }

        public Response<Embeddings> GetEmbeddings(string deploymentOrModelName, EmbeddingsOptions embeddingsOptions)
        {
            if (clientBuilder == null)
                throw new System.Exception("Client not initialized");
            return clientBuilder.GetClient().GetEmbeddings(deploymentOrModelName, embeddingsOptions);
        }

    }
}
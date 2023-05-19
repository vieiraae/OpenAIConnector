namespace OpenAIConnector 
{
    using Azure;
    using Azure.AI.OpenAI;
    using Azure.Identity;
    public class ClientBuilder
    {
        private OpenAIClient? client;

        // Azure OpenAI service
        public ClientBuilder(Uri endpoint, string key)
        {
            client = new(endpoint, new AzureKeyCredential(key));
        }

        // Azure OpenAI service with an Azure Active Directory Credential
        public ClientBuilder(Uri endpoint)
        {
            client = new(endpoint, new DefaultAzureCredential());
        }

        // openai.com service
        public ClientBuilder(string key)
        {
            client = new(key, new OpenAIClientOptions());
        }


        public OpenAIClient GetClient()
        {
            if (client == null)
                throw new System.Exception("Client not initialized");
            return client;
        }

    }

}
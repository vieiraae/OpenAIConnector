namespace OpenAIConnector
{
    public class Chat
    {
        private ClientBuilder? clientBuilder;

        public Chat(ClientBuilder clientBuilder)
        {
            this.clientBuilder = clientBuilder;
        }

        // TODO: Consider ChatML; session persistance

    }
}
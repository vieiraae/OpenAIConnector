namespace OpenAIConnector
{
    using Azure;
    using Azure.AI.OpenAI;
    public class CompletionUnitTests
    {

        private ClientBuilder? clientBuilder;
        private Completion? completion;
        public CompletionUnitTests(ClientBuilder clientBuilder)
        {
            this.clientBuilder = clientBuilder;
            this.completion = new(clientBuilder);
        }
        public void UnitTest1(string deploymentOrModelName)
        {
            if (completion is null) throw new System.Exception("Client not initialized");
            string prompt = "Where DaVinci was born?";
            Console.Write($"Input: {prompt}\n");
            Response<Completions> completionsResponse = completion.GetCompletions(deploymentOrModelName, prompt);
            Console.Write($"Chatbot: ");
            foreach (Choice choice in completionsResponse.Value.Choices) Console.Write(choice.Text);
        } 

        public void UnitTest2(string deploymentOrModelName)
        {
            if (completion is null) throw new System.Exception("Client not initialized");
            string textToSummarize = @"
                Two independent experiments reported their results this morning at CERN, Europe's high-energy physics laboratory near Geneva in Switzerland. Both show convincing evidence of a new boson particle weighing around 125 gigaelectronvolts, which so far fits predictions of the Higgs previously made by theoretical physicists.

                ""As a layman I would say: 'I think we have it'. Would you agree?"" Rolf-Dieter Heuer, CERN's director-general, asked the packed auditorium. The physicists assembled there burst into applause.
            :";

            string summarizationPrompt = @$"
                Summarize the following text.

                Text:
                """"""
                {textToSummarize}
                """"""

                Summary:
            ";

            Console.Write($"Input: {summarizationPrompt}");
            var completionsOptions = new CompletionsOptions()
            {
                Prompts = { summarizationPrompt },
            };

            Response<Completions> completionsResponse = completion.GetCompletions(deploymentOrModelName, completionsOptions);

            Console.Write($"Summarization: ");
            foreach (Choice choice in completionsResponse.Value.Choices) Console.Write(choice.Text);
        }

        // Summarize issue resolution from conversation
        public void UnitTest3(string deploymentOrModelName)
        {
            if (completion is null) throw new System.Exception("Client not initialized");
            Response<Completions> completionsResponse = completion.GetCompletions(
                deploymentOrModelName,
                new CompletionsOptions()
                {
                    Prompts = { "Generate a summary of the below conversation in the following format:\nCustomer problem:\nOutcome of the conversation:\nAction items for follow-up:\nCustomer budget:\nDeparture city:\nDestination city:\n\nConversation:\nUser: Hi there, I’m off between August 25 and September 11. I saved up 4000 for a nice trip. If I flew out from San Francisco, what are your suggestions for where I can go?\nAgent: For that budget you could travel to cities in the US, Mexico, Brazil, Italy or Japan. Any preferences?\nUser: Excellent, I’ve always wanted to see Japan. What kind of hotel can I expect?\nAgent: Great, let me check what I have. First, can I just confirm with you that this is a trip for one adult?\nUser: Yes it is\nAgent: Great, thank you, In that case I can offer you 15 days at HOTEL Sugoi, a 3 star hotel close to a Palace. You would be staying there between August 25th and September 7th. They offer free wifi and have an excellent guest rating of 8.49/10. The entire package costs 2024.25USD. Should I book this for you?\nUser: That sounds really good actually. Please book me at Sugoi.\nAgent: I can do that for you! Can I help you with anything else today?\nUser: No, thanks! Please just send me the itinerary to my email soon.\n\nSummary:" },
                    Temperature = (float)0.3,
                    MaxTokens = 350,
                    NucleusSamplingFactor = (float)1,
                    FrequencyPenalty = (float)0,
                    PresencePenalty = (float)0,
                });
            Console.Write($"Summary: ");
            foreach (Choice choice in completionsResponse.Value.Choices) Console.Write(choice.Text);                
        }
                    
        // Summarize key points from financial report (extractive)
        public void UnitTest4(string deploymentOrModelName)
        {
            if (completion is null) throw new System.Exception("Client not initialized");
            Response<Completions> completionsResponse = completion.GetCompletions(
                deploymentOrModelName,
                new CompletionsOptions()
                {
                    Prompts = { "Below is an extract from the annual financial report of a company. Extract key financial number (if present), key internal risk factors, and key external risk factors.\n\n# Start of Report\nRevenue increased $7.5 billion or 16%. Commercial products and cloud services revenue increased $4.0 billion or 13%. O365 Commercial revenue grew 22% driven by seat growth of 17% and higher revenue per user. Office Consumer products and cloud services revenue increased $474 million or 10% driven by Consumer subscription revenue, on a strong prior year comparable that benefited from transactional strength in Japan. Gross margin increased $6.5 billion or 18% driven by the change in estimated useful lives of our server and network equipment. \nOur competitors range in size from diversified global companies with significant research and development resources to small, specialized firms whose narrower product lines may let them be more effective in deploying technical, marketing, and financial resources. Barriers to entry in many of our businesses are low and many of the areas in which we compete evolve rapidly with changing and disruptive technologies, shifting user needs, and frequent introductions of new products and services. Our ability to remain competitive depends on our success in making innovative products, devices, and services that appeal to businesses and consumers.\n# End of Report" },
                    Temperature = (float)0.3,
                    MaxTokens = 350,
                    NucleusSamplingFactor = (float)1,
                    FrequencyPenalty = (float)0,
                    PresencePenalty = (float)0,
                    GenerationSampleCount = 1,
                });
            foreach (Choice choice in completionsResponse.Value.Choices) Console.Write(choice.Text);
        }

        // Summarize an article (abstractive)
        public void UnitTest5(string deploymentOrModelName)
        {
            if (completion is null) throw new System.Exception("Client not initialized");
            Response<Completions> completionsResponse = completion.GetCompletions(
                deploymentOrModelName,
                new CompletionsOptions()
                {
                    Prompts = { "Provide a summary of the text below that captures its main idea.\n\nAt Microsoft, we have been on a quest to advance AI beyond existing techniques, by taking a more holistic, human-centric approach to learning and understanding. As Chief Technology Officer of Azure AI Cognitive Services, I have been working with a team of amazing scientists and engineers to turn this quest into a reality. In my role, I enjoy a unique perspective in viewing the relationship among three attributes of human cognition: monolingual text (X), audio or visual sensory signals, (Y) and multilingual (Z). At the intersection of all three, there’s magic—what we call XYZ-code as illustrated in Figure 1—a joint representation to create more powerful AI that can speak, hear, see, and understand humans better. We believe XYZ-code will enable us to fulfill our long-term vision: cross-domain transfer learning, spanning modalities and languages. The goal is to have pre-trained models that can jointly learn representations to support a broad range of downstream AI tasks, much in the way humans do today. Over the past five years, we have achieved human performance on benchmarks in conversational speech recognition, machine translation, conversational question answering, machine reading comprehension, and image captioning. These five breakthroughs provided us with strong signals toward our more ambitious aspiration to produce a leap in AI capabilities, achieving multi-sensory and multilingual learning that is closer in line with how humans learn and understand. I believe the joint XYZ-code is a foundational component of this aspiration, if grounded with external knowledge sources in the downstream AI tasks." },
                    Temperature = (float)0.3,
                    MaxTokens = 250,
                    NucleusSamplingFactor = (float)1,
                    FrequencyPenalty = (float)0,
                    PresencePenalty = (float)0,
                    GenerationSampleCount = 1,
                });            
            foreach (Choice choice in completionsResponse.Value.Choices) Console.Write(choice.Text);
        }

        // Generate product name ideas
        public void UnitTest6(string deploymentOrModelName)
        {
            if (completion is null) throw new System.Exception("Client not initialized");
            Response<Completions> completionsResponse = completion.GetCompletions(
                deploymentOrModelName,
                new CompletionsOptions()
                {
                    Prompts = { "Generate product name ideas for a yet to be launched wearable health device that will allow users to monitor their health and wellness in real-time using AI and share their health metrics with their friends and family. The generated product name ideas should reflect the product's key features, have an international appeal, and evoke positive emotions.\n\nSeed words: fast, healthy, compact\n\nExample product names: \n1. WellnessVibe\n2. HealthFlux\n3. VitalTracker\n\nProduct names:\n1." },
                    Temperature = (float)0.8,
                    MaxTokens = 60,
                    NucleusSamplingFactor = (float)1,
                    FrequencyPenalty = (float)0,
                    PresencePenalty = (float)0,
                    GenerationSampleCount = 1,
                });            
            foreach (Choice choice in completionsResponse.Value.Choices) Console.Write(choice.Text);
        }

        // Generate an email
        public void UnitTest7(string deploymentOrModelName)
        {
            if (completion is null) throw new System.Exception("Client not initialized");            
            Response<Completions> completionsResponse = completion.GetCompletions(
                deploymentOrModelName: "text-davinci-003",
                new CompletionsOptions()
                {
                    Prompts = { "Write a product launch email for new AI-powered headphones that are priced at $79.99 and available at Best Buy, Target and Amazon.com. The target audience is tech-savvy music lovers and the tone is friendly and exciting.\n\n1. What should be the subject line of the email?  \n2. What should be the body of the email?\n \nSubject Line: Experience unparalleled sound with our new AI-powered headphones!\n\nDear Music Lover,\n\nAre you ready to experience the next level of audio? Then you have to try our new AI-powered headphones! These sleek and stylish headphones offer unparalleled sound with comfort and convenience.\n\nAt just $79.99, our AI-powered headphones give you the best music experience. With advanced sound processing, our headphones can understand your music preferences, making sure you always get the optimal sound quality. Plus, they’re compatible with most devices, so you can listen to your favorite tunes anywhere you go.\n\nSo don’t wait any longer - pick up your AI-powered headphones today at Best Buy, Target and Amazon.com! Enjoy superior sound quality and superior convenience with our new AI-powered headphones.\n\nGet ready to experience better music now!\n\nSincerely, \n[Company Name]" },
                    Temperature = (float)1,
                    MaxTokens = 350,
                    NucleusSamplingFactor = (float)1,
                    FrequencyPenalty = (float)0,
                    PresencePenalty = (float)0,
                    GenerationSampleCount = 1,
                });            
            foreach (Choice choice in completionsResponse.Value.Choices) Console.Write(choice.Text);
        }

        // Generate a product description (bullet points)
        public void UnitTest8(string deploymentOrModelName)
        {
            if (completion is null) throw new System.Exception("Client not initialized");
            Response<Completions> completionsResponse = completion.GetCompletions(
                deploymentOrModelName: "text-davinci-003",
                new CompletionsOptions()
                {
                    Prompts = { "Write a product description in bullet points for a renters insurance product that offers customizable coverage, rewards and incentives, flexible payment options and a peer-to-peer referral program. The tone should be persuasive and professional." },
                    Temperature = (float)1,
                    MaxTokens = 150,
                    NucleusSamplingFactor = (float)1,
                    FrequencyPenalty = (float)1.6,
                    PresencePenalty = (float)0,
                    GenerationSampleCount = 1,
                });
            foreach (Choice choice in completionsResponse.Value.Choices) Console.Write(choice.Text);
        }

        // Generate a listicle-style blog
        public void UnitTest9(string deploymentOrModelName)
        {
            if (completion is null) throw new System.Exception("Client not initialized");
            Response<Completions> completionsResponse = completion.GetCompletions(
                deploymentOrModelName: "text-davinci-003",
                new CompletionsOptions()
                {
                    Prompts = { "Write a catchy and creative listicle style blog on the topic of emerging trends in e-commerce that are shaping the future of retail. The blog should have a memorable headline and a clear call to action in the end encouraging the reader to engage further." },
                    Temperature = (float)1,
                    MaxTokens = 600,
                    NucleusSamplingFactor = (float)1,
                    FrequencyPenalty = (float)0,
                    PresencePenalty = (float)0,
                    GenerationSampleCount = 1,
                });
            foreach (Choice choice in completionsResponse.Value.Choices) Console.Write(choice.Text);
        }


    }
}
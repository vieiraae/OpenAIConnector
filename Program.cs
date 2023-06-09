﻿namespace OpenAIConnector
{
    using System;
    using System.IO;
    using System.Reflection;
    using System.Threading.Tasks;
    using Azure;
    using Azure.AI.OpenAI;

    class Program
    {
        static async Task Main(string[] args)
        {
            if (args.Length < 3)
            {
                Console.WriteLine("Please specify the deployment and the use case number:");
                return;
            }
            string? key = Environment.GetEnvironmentVariable("OPENAI_KEY");
            if (key   == null || key.Length == 0)
            {
                Console.WriteLine("Please set the environment variables:");
                Console.WriteLine("AZURE_OPENAI_KEY");
                return;
            }
            string? endpoint = Environment.GetEnvironmentVariable("AZURE_OPENAI_ENDPOINT");

            ClientBuilder clientBuilder = (endpoint   == null || endpoint.Length == 0)?(new ClientBuilder(key)):(new ClientBuilder(new Uri(endpoint), key));

            string deploymentOrModelName = args[1];
            switch (args[0])
            {
                case "completion":
                    Type? completionUnitTestsType = typeof(CompletionUnitTests);
                    if (completionUnitTestsType is not null)
                    {
                        Console.WriteLine("Running UnitTest" + args[2] + " ...");
                        MethodInfo? methodInfo = completionUnitTestsType.GetMethod("UnitTest" + args[2]);
                        if (methodInfo is not null)
                            methodInfo.Invoke(new CompletionUnitTests(clientBuilder), new object[] { deploymentOrModelName});
                    }
                    break;
                case "chat":
                    Type? chatUnitTestsType = typeof(ChatUnitTests);
                    if (chatUnitTestsType is not null)
                    {
                        Console.WriteLine("Running UnitTest" + args[2] + " ...");
                        MethodInfo? methodInfo = chatUnitTestsType.GetMethod("UnitTest" + args[2]);
                        if (methodInfo is not null)
                            await (dynamic)methodInfo.Invoke(new ChatUnitTests(clientBuilder), new object[] { deploymentOrModelName});
                    }
                    break;
                case "embedding":
                    Type? embeddingUnitTests = typeof(EmbeddingUnitTests);
                    if (embeddingUnitTests is not null)
                    {
                        Console.WriteLine("Running UnitTest" + args[2] + " ...");
                        MethodInfo? methodInfo = embeddingUnitTests.GetMethod("UnitTest" + args[2]);
                        if (methodInfo is not null)
                            await (dynamic)methodInfo.Invoke(new EmbeddingUnitTests(clientBuilder), new object[] { deploymentOrModelName});
                    }
                    break;
                default:
                    Console.WriteLine("Action not recongnized");
                    return;

            }

        }

    }
}
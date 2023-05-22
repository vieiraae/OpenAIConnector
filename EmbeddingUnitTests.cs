namespace OpenAIConnector
{
    using Azure;
    using Azure.AI.OpenAI;
    using Microsoft.ML;
    using Microsoft.ML.Data;
    using Microsoft.ML.AutoML;    
    public class EmbeddingUnitTests
    {

        private ClientBuilder? clientBuilder;
        private Embedding? embedding;
        public EmbeddingUnitTests(ClientBuilder clientBuilder)
        {
            this.clientBuilder = clientBuilder;
            this.embedding = new(clientBuilder);
        }

        // Amazon Reviews using OpenAI embeddings and ML.NET AutoML: https://github.com/luisquintanilla/OpenAIAutoMLNET/tree/main
        public async Task UnitTest1(string deploymentOrModelName)
        {
            if (embedding is null) throw new System.Exception("Client not initialized");


            // Initialize MLContext
            var ctx = new MLContext();

            // Get column information
            // Dataset https://www.kaggle.com/datasets/snap/amazon-fine-food-reviews?resource=download
            var inferResults = ctx.Auto().InferColumns("Reviews.csv", hasHeader: true, labelColumnIndex:6, separatorChar:',', groupColumns:false);

            // Load data
            var textLoader = ctx.Data.CreateTextLoader(inferResults.TextLoaderOptions);
            var data = textLoader.Load("Reviews.csv");

            // Drop columns
            var dropOperation = ctx.Transforms.DropColumns(new [] {"Id", "ProductId", "UserId","ProfileName", "HelpfulnessNumerator", "HelpfulnessDenominator"});
            var droppedColsDv = dropOperation.Fit(data).Transform(data);

            // Get most recent reviews (lower bound is unix timestamp for Jan 1, 2012)
            var mostRecentDv = ctx.Data.FilterRowsByColumn(droppedColsDv, "Time", lowerBound: 1325394000.0);

            // Shuffle data
            var shuffledData = ctx.Data.ShuffleRows(mostRecentDv);

            // Create a subset of 500 rows
            var dataSubset = ctx.Data.TakeRows(shuffledData, 500);

            // Custom transform to get OpenAI embeddings
            var getEmbedding = (EmbeddingInput input, EmbeddingOutput output) => 
            {
                // Initialize embedding options using text from PreembedText column
                var embeddingOptions = new EmbeddingsOptions(input.PreembedText);

                // Get embeddings of PreembedText column
                Embeddings embeddingResult = embedding.GetEmbeddings(deploymentOrModelName, embeddingOptions);

                // Store embeddings in Embeddings column
                output.Embeddings = embeddingResult.Data[0].Embedding.ToArray();
            };

            // Prepare data
            var dataPrepPipeline = 
                ctx.Transforms.CopyColumns("Label","Score") // Create column called label
                .Append(ctx.Transforms.DropColumns("Score")) // Drop original Score column
                .Append(ctx.Transforms.Expression(
                    outputColumnName: "PreembedText",
                    expression: "(summary,text) => left(concat(summary,text), 4096)",
                    inputColumnNames: new []{"Summary", "Text"})) // Concatenate summary and text columns and take first 4096 elements
                .Append(ctx.Transforms.CustomMapping(getEmbedding, "GetEmbedding")); // Get OpenAI embeddings

            // Apply preprocessing transforms
            var preprocessedData = 
                dataPrepPipeline.Fit(dataSubset).Transform(dataSubset);

            // Split data into train (400 reviews) / test (100 reviews) sets
            var trainTestData = ctx.Data.TrainTestSplit(preprocessedData, testFraction: 0.2);

            // Define AutoML Task
            var regressionPipeline = ctx.Auto().Regression(labelColumnName: "Label", featureColumnName: "Embeddings", useLgbm: false);

            // Initialize Experiment
            var experiment = ctx.Auto().CreateExperiment();

            // Configure experiment
            // This may take longer than 180 seconds
            experiment
                .SetDataset(trainTestData.TrainSet)
                .SetPipeline(regressionPipeline)
                .SetRegressionMetric(RegressionMetric.MeanAbsoluteError)
                .SetTrainingTimeInSeconds(180);

            // Configure monitor
            var monitor = new ExperimentMonitor(regressionPipeline);
            experiment.SetMonitor<ExperimentMonitor>(monitor);

            // Run experiment
            var experimentResult = await experiment.RunAsync();

            // Get model
            var model = experimentResult.Model;

            // Use model to make predictions on test dataset
            var predictions = model.Transform(trainTestData.TestSet);

            // Calculate evaluation metrics on test dataset
            var testEvaluationMetrics = ctx.Regression.Evaluate(predictions); 

            // Output Mean Absoute Error
            Console.WriteLine($"Test Set MAE: {testEvaluationMetrics.MeanAbsoluteError}");

        }
            
        public class EmbeddingInput
        {
            public string PreembedText { get; set; }

        }

        public class EmbeddingOutput
        {
            public string PreembedText { get; set; }
            
            [VectorType(4096)]
            public float[] Embeddings { get; set; }
        }        

    }

    public class ExperimentMonitor : IMonitor
    {
        private readonly SweepablePipeline _pipeline;
        private readonly List<TrialResult> _completedTrials;
        public ExperimentMonitor(SweepablePipeline pipeline)
        {
            _pipeline = pipeline;
            _completedTrials = new List<TrialResult>();
        }

        public IEnumerable<TrialResult> GetCompletedTrials() => _completedTrials;

        public void ReportBestTrial(TrialResult result)
        {
            return;
        }

        public void ReportCompletedTrial(TrialResult result)
        {
            var trialId = result.TrialSettings.TrialId;
            var timeToTrain = result.DurationInMilliseconds;
            var pipeline = _pipeline.ToString(result.TrialSettings.Parameter);
            _completedTrials.Add(result);
            Console.WriteLine($"Trial {trialId} finished training in {timeToTrain}ms with pipeline {pipeline}");
        }

        public void ReportFailTrial(TrialSettings settings, Exception exception = null)
        {
            if (exception.Message.Contains("Operation was canceled."))
            {
                Console.WriteLine($"{settings.TrialId} cancelled. Time budget exceeded.");
            }
            Console.WriteLine($"{settings.TrialId} failed with exception {exception.Message}");
        }

        public void ReportRunningTrial(TrialSettings setting)
        {
            return;
        }
    }    
}
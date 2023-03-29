using Microsoft.ML;
using Microsoft.ML.Data;
using Microsoft.ML.Tokenizers;
using Microsoft.ML.TorchSharp;
using Microsoft.ML.TorchSharp.NasBert;
using System;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using TorchSharp;

// Reference articles & code:
// - https://devblogs.microsoft.com/dotnet/announcing-ml-net-2-0/#text-classification-scenario-in-model-builder
// - https://devblogs.microsoft.com/dotnet/introducing-the-ml-dotnet-text-classification-api-preview/
// - https://learn.microsoft.com/en-us/dotnet/machine-learning/tutorials/sentiment-analysis-model-builder
// - https://github.com/dotnet/machinelearning-samples/blob/main/samples/csharp/getting-started/MLNET2/TextClassification/ReviewSentiment.training.cs

namespace NasBertApp.Models
{
    internal class TextClassifier
    {
        public string DataSetPath { get; set; } = string.Empty;
        public string SavaFolderPath { get; set; } = string.Empty;
        public string ModelName { get; set; } = string.Empty;
        public string ModelPath { get; set; } = string.Empty;
        public string InputText { get; set; } = string.Empty;
        public string ResultClass { get; set; } = string.Empty;
        public float MaxScore { get; set; }

        public async Task TrainingModelAsync()
        {
            await Task.Run(() => this.TrainingModel());
        }

        public void TrainingModel()
        {
            // Initialize MLContext
            MLContext mlContext = new MLContext()
            {
                GpuDeviceId = 0,
                FallbackToCpu = true
            };

            //torch.InitializeDeviceType(DeviceType.CUDA);

            // Load the data source
            Debug.WriteLine("Loading data...");
            IDataView dataView = mlContext.Data.LoadFromTextFile<ModelInput>(
                this.DataSetPath,
                separatorChar: '\t',
                hasHeader: false
            );

            // To evaluate the effectiveness of machine learning models we split them into a training set for fitting
            // and a testing set to evaluate that trained model against unknown data
            DataOperationsCatalog.TrainTestData dataSplit = mlContext.Data.TrainTestSplit(dataView, testFraction: 0.2, seed: 1234);
            IDataView trainData = dataSplit.TrainSet;
            IDataView testData = dataSplit.TestSet;

            // Create a pipeline for training the model
            var pipeline = mlContext.Transforms.Conversion.MapValueToKey(
                                        outputColumnName: "Label",
                                        inputColumnName: "Label")
                                    .Append(mlContext.MulticlassClassification.Trainers.TextClassification(
                                        labelColumnName: "Label",
                                        sentence1ColumnName: "Sentence",
                                        architecture: BertArchitecture.Roberta))
                                    .Append(mlContext.Transforms.Conversion.MapKeyToValue(
                                        outputColumnName: "PredictedLabel",
                                        inputColumnName: "PredictedLabel"));

            // Train the model using the pipeline
            Debug.WriteLine("Training model...");
            ITransformer model = pipeline.Fit(trainData);
            Debug.WriteLine("Training Completed.");

            var modelPath = Path.Combine(this.SavaFolderPath, this.ModelName + ".zip");
            Debug.WriteLine(modelPath);
            mlContext.Model.Save(model, dataView.Schema, modelPath);
        }

        public async Task ClassifyTextAsync()
        {
            await Task.Run(() => this.ClassifyText());
        }

        public void ClassifyText()
        {
            // Initialize MLContext
            MLContext mlContext = new MLContext()
            {
                GpuDeviceId = 0,
                FallbackToCpu = true
            };

            //Define DataViewSchema for data preparation pipeline and trained model
            DataViewSchema modelSchema;

            // Load trained model
            ITransformer model = mlContext.Model.Load(this.ModelPath, out modelSchema);

            // Generate a prediction engine
            Debug.WriteLine("Creating prediction engine...");
            PredictionEngine<ModelInput, ModelOutput> engine = mlContext.Model.CreatePredictionEngine<ModelInput, ModelOutput>(model);

            Debug.WriteLine("Ready to generate predictions.");

            // Generate a series of predictions based on user input
            // Get a prediction
            ModelInput sampleData = new(this.InputText);
            ModelOutput result = engine.Predict(sampleData);

            this.ResultClass = Sentiments.SentimentsDict[result.PredictedLabel];
            this.MaxScore = result.Score[(uint)result.PredictedLabel] * 100;
        }
    }

    public class ModelInput
    {
        public ModelInput(string utterance)
        {
            Sentence = utterance;
        }

        [LoadColumn(0)]
        [ColumnName(@"Sentence")]
        public string Sentence { get; set; }

        [LoadColumn(1)]
        [ColumnName(@"Label")]
        public float Label { get; set; }
    }

    public class ModelOutput
    {
        [ColumnName(@"Sentence")]
        public string Sentence { get; set; }

        [ColumnName(@"Label")]
        public uint Label { get; set; }

        [ColumnName(@"PredictedLabel")]
        public float PredictedLabel { get; set; }

        [ColumnName(@"Score")]
        public float[] Score { get; set; }
    }
}

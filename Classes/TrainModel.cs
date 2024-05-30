using Microsoft.ML;
using Microsoft.ML.Vision;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SpoofingDetectionWinformApp.Classes
{
    public class TrainModel
    {

        public void StartTrainModel(string pathDirTrain)
        {
            var imageFolder = Path.Combine(Environment.CurrentDirectory, "..", "..", "..", pathDirTrain);
            var files = Directory.GetFiles(imageFolder, "*", SearchOption.AllDirectories);
            var images = files.Select(file => new ImageData
            {
                ImagePath = file,
                Label = Directory.GetParent(file).Name
            });

            var context = new MLContext();
            var imageData = context.Data.LoadFromEnumerable(images);
            var imageDataShuffled = context.Data.ShuffleRows(imageData);

            var testTrainData = context.Data.TrainTestSplit(imageDataShuffled, testFraction: 0.2);

            var validationData = context.Transforms.Conversion.MapValueToKey("LabelKey", "Label",
                keyOrdinality: Microsoft.ML.Transforms.ValueToKeyMappingEstimator.KeyOrdinality.ByValue)
                .Append(context.Transforms.LoadRawImageBytes("Image", imageFolder, "ImagePath"))
                .Fit(testTrainData.TestSet)
                .Transform(testTrainData.TestSet);

            var options = new ImageClassificationTrainer.Options()
            {
                Arch = ImageClassificationTrainer.Architecture.ResnetV250,
                Epoch = 100,
                BatchSize = 20,
                LearningRate = 0.01f,
                LabelColumnName = "LabelKey",
                FeatureColumnName = "Image",
                ValidationSet = validationData
            };

            var imagePipeline = context.Transforms.Conversion.MapValueToKey("LabelKey", "Label", keyOrdinality: Microsoft.ML.Transforms.ValueToKeyMappingEstimator.KeyOrdinality.ByValue)
                .Append(context.Transforms.LoadRawImageBytes("Image", imageFolder, "ImagePath"));

            var imageDataView = imagePipeline.Fit(testTrainData.TrainSet).Transform(testTrainData.TrainSet);

            var pipeline = context.MulticlassClassification.Trainers.ImageClassification(options)
                .Append(context.Transforms.Conversion.MapKeyToValue("PredictedLabel"));

            var model = pipeline.Fit(imageDataView);


            context.Model.Save(model, imageDataView.Schema, "model.zip");
        }

        public void UseModel(string modelName, string pathDirTest)
        {
            var context = new MLContext();
            var modelSave = context.Model.Load(modelName, out DataViewSchema modelSchema);

            var testImagesFolder = Path.Combine(Environment.CurrentDirectory, "..", "..", "..", pathDirTest);
            var testFiles = Directory.GetFiles(testImagesFolder, "*", SearchOption.AllDirectories);

            var testImages = testFiles.Select(file => new ImageModelInput
            {
                ImagePath = file,
            });
            Console.WriteLine(Environment.NewLine);

            var testImageData = context.Data.LoadFromEnumerable(testImages);
            var imagePipeline = context.Transforms.Conversion.MapValueToKey("LabelKey", "Label", keyOrdinality: Microsoft.ML.Transforms.ValueToKeyMappingEstimator.KeyOrdinality.ByValue)
                .Append(context.Transforms.LoadRawImageBytes("Image", testImagesFolder, "ImagePath"));
            var testImageDataView = imagePipeline.Fit(testImageData).Transform(testImageData);
            var predictions = modelSave.Transform(testImageDataView);

            var testPredictions = context.Data.CreateEnumerable<ImagePrediction>(predictions, reuseRowObject: false);
            int countLive = 0;
            int countSpoof = 0;
            foreach (var prediction in testPredictions)
            {
                //Console.WriteLine($"Image: {Path.GetFileName(prediction.ImagePath)}, Predicted Label: {prediction.PredictedLabel}");
                if (prediction.PredictedLabel == "live")
                {
                    countLive++;
                }
                if (prediction.PredictedLabel == "spoof")
                {

                    countSpoof++;
                }
            }
            Console.WriteLine($"Live: {countLive}");
            Console.WriteLine($"Spoof: {countSpoof}");
        }
    }
}

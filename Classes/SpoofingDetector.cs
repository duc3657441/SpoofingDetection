using Microsoft.ML;
using Microsoft.ML.Data;
using OpenCvSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpoofingDetectionWinformApp.Classes
{
    public class SpoofingDetector
    {
        private string __modelPath;
        //private string __lePath;
        private float __confidence = Config.DEFAULT_CONFIDENCE;
        private FaceDetector __faceDetector;
        private object __spoofingModel; 
        private object __spoofingLe;
        private MLContext context;
        private ITransformer model;
        private bool __predictOne;

        public event Action<string> LogMessageEvent;

        protected virtual void OnLogMessage(string message)
        {
            LogMessageEvent?.Invoke(message);
        }

        public SpoofingDetector(string modelPath, float confidence = 0, bool predictOne = true)
        {
            this.__modelPath = modelPath;
            //this.__lePath = lePath;
            if (__confidence != 0)
            {
                this.__confidence = confidence;
            }
            this.__faceDetector = null;
            this.__spoofingModel = null;
            this.__spoofingLe = null;
            context = new MLContext();
            this.__predictOne = predictOne;
           
        }
        ~SpoofingDetector() 
        {
            this.__modelPath = null;
            //this.__lePath = null;
            this.__confidence = 0;
            this.__faceDetector = null;
            this.__spoofingLe = null;
            this.__spoofingModel = null;
            this.context = null;
            this.model = null;
            
        }

        public ITransformer LoadModel () 
        {
           
            OnLogMessage("[INFO] Loading OpenCV face caffe model ...");
            __faceDetector = new FaceDetector(caffe_model_path: Config.OPENCV_FACE_CAFFE_MODEL, 
                                            proto_text_path: Config.OPENCV_FACE_CAFFE_PROTOTXT,
                                            confidence: __confidence);
            if (__faceDetector.load_net_model() == null)
            {
                throw new Exception("[ERROR] Can't load OpenCV face caffe net model!");
            }
            OnLogMessage(string.Format("[INFO] Loading {0} ",__modelPath));
            if (!string.IsNullOrEmpty(__modelPath) && File.Exists(__modelPath))
            {
                model = context.Model.Load(__modelPath, out DataViewSchema modelSchema);
            }
            else
            {
                model = null;
            }
            return model;
        }

        public List<SpoofingPredictions> Predict (Mat frame, bool predictOne = true)
        {
            List<SpoofingPredictions> spoofingPredictions = new List<SpoofingPredictions>();
            
            if (model == null)
            {
                throw new Exception("[ERROR] Can't load model");
            }
            List<FacePrediction> facePredictions;
            if (predictOne)
            {
                var facePrediction = __faceDetector.Predict_one(frame);
               
                facePredictions = new List<FacePrediction> { facePrediction };
            }
            else
            {
                facePredictions = __faceDetector.Predict(frame);
            }
            

            foreach (var facePred in facePredictions)
            {
                Mat face = new Mat();

                //Cv2.Resize(facePred.Face, face, new OpenCvSharp.Size(Config.FACE_WIDTH, Config.FACE_HEIGHT), interpolation: InterpolationFlags.Linear);
                if (facePred.Face == null || facePred == null)
                {
                    return null;
                }
                face = facePred.Face;
                
                byte[] byteArray = face.ToBytes();
                var imageDataList = new List<ImageModelInput>
                                    {
                                        new ImageModelInput
                                        {
                                            Image = byteArray
                                        }
                                    };
                IEnumerable<ImageModelInput> imageData = imageDataList;
                var imageDataView = context.Data.LoadFromEnumerable(imageData);
                var predictions = model.Transform(imageDataView);
                var predictionEnumerator = context.Data.CreateEnumerable<ImagePrediction>(predictions, reuseRowObject: false).GetEnumerator();
                if (predictionEnumerator.MoveNext())
                {
                    var prediction = predictionEnumerator.Current;
                    var confidence = prediction.Score[Array.IndexOf(prediction.Score, prediction.Score.Max())];
                    //OnLogMessage($"Image: {Path.GetFileName(prediction.ImagePath)}");
                    OnLogMessage($"Predicted Label: {prediction.PredictedLabel}");
                    //OnLogMessage($"Live Score: {liveScore}");
                    //OnLogMessage($"Spoof Score: {spoofScore}");
                    if (confidence > __confidence)
                    {
                        spoofingPredictions.Add(new SpoofingPredictions()
                        {
                            Face = facePred.Face,
                            ROI = facePred.ROI,
                            Label = prediction.PredictedLabel,
                            Confidence = confidence
                        });
                    }
                }
            }
            return spoofingPredictions;
        }
    }

}

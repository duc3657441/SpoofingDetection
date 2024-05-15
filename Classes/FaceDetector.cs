using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenCvSharp;
using OpenCvSharp.Dnn;

//https://www.youtube.com/watch?v=pRVWdwFy7dM

namespace SpoofingDetectionWinformApp.Classes
{
    public class FaceDetector
    {
        private string __caffe_model_path = Config.OPENCV_FACE_CAFFE_MODEL;
        private string __proto_text_path = Config.OPENCV_FACE_CAFFE_PROTOTXT;
        private float __confidence = Config.DEFAULT_CONFIDENCE;
        private Net? __dnn_net = null;
        private OpenCvSharp.Size size = new OpenCvSharp.Size(300, 300);

        public FaceDetector(string caffe_model_path = null, string proto_text_path = null, float confidence = 0)
        {
            if (!string.IsNullOrEmpty(caffe_model_path))
            {
                __caffe_model_path = caffe_model_path;
            }
            if (!string.IsNullOrEmpty(proto_text_path))
            {
                __proto_text_path = proto_text_path;
            }
            if (confidence != 0)
            {
                __confidence = confidence;
            }
        }



        // Chuyển img sang dạng blob
        public Mat __blob_from_Image(Mat img)
        {
            var resizedImg = img.Resize(this.size);
            Scalar myScalar = new Scalar(104.0, 177.0, 123.0);
            var blob = CvDnn.BlobFromImage(resizedImg, 1, size, myScalar, true);
            // resizedImg = CvDnn.BlobFromImage(resizedImg, 1.0, new Size(300, 300), new Scalar(104.0, 177.0, 123.0));
            return blob;
        }

        // Kiểm tra và load model 
        public Net? load_net_model()
        {
            this.__dnn_net = CvDnn.ReadNetFromCaffe(__proto_text_path, __caffe_model_path);
            return this.__dnn_net;
        }

        // Dự đoán 1 list khuôn mặt trong 1 frame
        public List<FacePrediction> Predict(Mat frame)
        {
            List<FacePrediction> facePredictions = new List<FacePrediction>();
            frame = frame.Resize(this.size);

            this.__dnn_net = load_net_model();
            // get the frame dimensions and convert it to a blob (Lấy kích thước ảnh và chuyển thành blob) 
            var blob = __blob_from_Image(frame);

            //  input the blob to the network and get the detections and predictions (cho blob vào mạng nơ ron)

            this.__dnn_net.SetInput(blob);
            var detections = this.__dnn_net.Forward();
            Mat detectionmat = new Mat(detections.Size(2), detections.Size(3), MatType.CV_32F, detections.Ptr(0));

            // Loop over detections
            for (int i = 0; i < detectionmat.Rows; i++)
            {

                // extract the confidence
                float confidence = detectionmat.At<float>(i, 2);

                // Filter out weak detections
                if (confidence > this.__confidence)
                {
                    int start_x = (int)(detectionmat.At<float>(i, 3) * size.Width);
                    int start_y = (int)(detectionmat.At<float>(i, 4) * size.Height);
                    int end_x = (int)(detectionmat.At<float>(i, 5) * size.Width);
                    int end_y = (int)(detectionmat.At<float>(i, 6) * size.Height);
                    if (start_x >= end_x || start_y >= end_y)
                    {
                        continue;
                    }
                    Cv2.Rectangle(frame, new OpenCvSharp.Point(start_x, start_y), new OpenCvSharp.Point(end_x, end_y), Scalar.Black);
                    var face = frame.SubMat(start_y, end_y, start_x, end_x);
                    List<int> roi = new List<int>();
                    roi.Add(start_x);
                    roi.Add(start_y);
                    roi.Add(end_x);
                    roi.Add(end_y);

                    facePredictions.Add(new FacePrediction() { Face = face, ROI = roi, Confidence = confidence });

                }
            }
            return facePredictions;

        }

        public FacePrediction Predict_one(Mat frame)
        {
            FacePrediction facePredictions = new FacePrediction();
            frame = frame.Resize(this.size);

            this.__dnn_net = load_net_model();
            var blob = __blob_from_Image(frame);
            this.__dnn_net.SetInput(blob);
            var detections = this.__dnn_net.Forward();
            Mat detectionmat = new Mat(detections.Size(2), detections.Size(3), MatType.CV_32F, detections.Ptr(0));

            int i = 0;
            float confidence = detectionmat.At<float>(i, 2);
            for (int j = 1; j < detectionmat.Rows; j++)
            {
                if (confidence < detectionmat.At<float>(j, 2))
                {
                    confidence = detectionmat.At<float>(j, 2);
                    i = j;
                }
            }

            if (confidence > this.__confidence)
            {
                int start_x = (int)(detectionmat.At<float>(i, 3) * size.Width);
                int start_y = (int)(detectionmat.At<float>(i, 4) * size.Height);
                int end_x = (int)(detectionmat.At<float>(i, 5) * size.Width);
                int end_y = (int)(detectionmat.At<float>(i, 6) * size.Height);
                Cv2.Rectangle(frame, new OpenCvSharp.Point(start_x, start_y), new OpenCvSharp.Point(end_x, end_y), Scalar.Black);
                
                var face = frame.SubMat(start_y, end_y, start_x, end_x);
                List<int> roi = new List<int>();
                roi.Add(start_x);
                roi.Add(start_y);
                roi.Add(end_x);
                roi.Add(end_y);
                
                facePredictions.Face = face;
                facePredictions.ROI = roi;
                facePredictions.Confidence = confidence;
            }

            return facePredictions;
        }

    }
}

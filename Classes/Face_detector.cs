using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SpoofingDetection.Classes;
using OpenCvSharp;
using OpenCvSharp.Dnn;

namespace SpoofingDetection.Classes
{
    

    internal class Face_detector
    {
        public string __caffe_model_path = Config.OPENCV_FACE_CAFFE_MODEL;
        public string __proto_text_path = Config.OPENCV_FACE_CAFFE_PROTOTXT;
        public float __confidence = Config.DEFAULT_CONFIDENCE;
        public Net? __dnn_net = null;

        // Chuyển img sang dạng blob
        public Mat __blob_from_Image(Mat img)
        {
            Mat resizedImg = img.Resize(new Size(300, 300));
            resizedImg = CvDnn.BlobFromImage(resizedImg, 1.0, new Size(300, 300), new Scalar(104.0, 177.0, 123.0));
            return resizedImg;
        }

        // Kiểm tra và load model 
        public Net load_net_model()
        {
            if (File.Exists(__proto_text_path) && File.Exists(__caffe_model_path))
            {
                this.__dnn_net = Net.ReadNetFromCaffe(__proto_text_path, __caffe_model_path);
                
            }
            else
            {
                this.__dnn_net = null;
                throw new ArgumentException("File __proto_text_path or __caffe_model_path do not exist");
            }

            return this.__dnn_net;
        }

        // Dự đoán khuôn mặt trong 1 frame
        public List<FacePrediction> Predict(Mat frame)
        {
            List<FacePrediction> facePredictions = new List<FacePrediction>();

            // get the frame dimensions and convert it to a blob (Lấy kích thước ảnh và chuyển thành blob)
            int height = frame.Rows;
            int width = frame.Cols;
            Mat blob = __blob_from_Image(frame);

            //  input the blob to the network and get the detections and predictions (cho blob vào mạng nơ ron)
            __dnn_net.SetInput(blob);
            Mat predictions = __dnn_net.Forward();

            // Loop over detections
            for (int i = 0; i < predictions.Cols; i++)
            {
               ;
                // extract the confidence
                double confidence = predictions.At<float>(0, 0, i, 2);

                // Filter out weak detections

                if (confidence > this.__confidence)
                {
                    // Compute bounding box (adjusting indices for OpenCV structure)
                    var boundingBox = new Scalar(
                            predictions.At<float>(0, 0, i, 3) * width,
                            predictions.At<float>(0, 0, i, 4) * height,
                            predictions.At<float>(0, 0, i, 5) * width,
                            predictions.At<float>(0, 0, i, 6) * height
                    );

                    // correct the bounding box
                    int start_x = (int)Math.Max(0, boundingBox[0]);
                    int start_y = (int)Math.Max(0, boundingBox[1]);
                    int end_x = (int)Math.Min(width, boundingBox[2]);
                    int end_y = (int)Math.Min(height, boundingBox[3]);

                    if (start_x >= end_x || start_y >= end_y)
                    {
                        continue;
                    }

                    // extract the face ROI
                    Mat face = frame.SubMat(start_y, end_y - start_y, start_x, end_x - start_x);

                    List<int> roi = new List<int>();
                    roi.Add(start_x);
                    roi.Add(start_y);
                    roi.Add(end_x);
                    roi.Add(end_y);

                    //Add face prediction
                   
                    facePredictions.Add(new FacePrediction(){Face = face, ROI = roi, Confidence = confidence});

                }

            }
            return facePredictions;
        }




    }
}

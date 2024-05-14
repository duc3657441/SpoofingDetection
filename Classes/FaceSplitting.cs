using OpenCvSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpoofingDetectionWinformApp.Classes
{
    // Crop face ROI to create face spooling training data set.

    
    public class FaceSplitting
    {
        public string __WINDOW_CAPTION = "Face Splitter - (press q to exit or esc to toggle predict one/multi)";

        private string __outputDir;
        private string? __srcInput = null;
        private OpenCvSharp.Size __resolution = default;
        private string __proto_text_path = Config.OPENCV_FACE_CAFFE_PROTOTXT;
        private string __caffe_model_path = Config.OPENCV_FACE_CAFFE_MODEL;
        private float __confidence = Config.DEFAULT_CONFIDENCE;
        private float __skip_frames = 0;
        private Measure? __measure;

        public FaceSplitting(string outputDir, string srcInput = null, OpenCvSharp.Size resolution = default, bool measure = false, float skipFrames = 0)
        {
            __outputDir = outputDir;
            __srcInput = srcInput;
            __resolution = resolution;
            __skip_frames = skipFrames;

            if (measure)
            {
                __measure = new Measure();
            }
            else
            {
                __measure = null;
            }
        }

        ~FaceSplitting()
        {
            __outputDir = null;
            __srcInput = null;
            __resolution = default;
            __skip_frames = 0;
            __measure = null;
        }

        public bool IsProcessFrame(int frameNumber)
        {
            return __skip_frames <= 0 || frameNumber % __skip_frames == 0;
        }

        public double Key(FacePrediction e)
        {
            return e.Confidence;
        }

        public void Start()
        {
            MessageBox.Show("[INFO] Loading OpenCV face caffe net model ...");
            FaceDetector faceDetector = new FaceDetector(caffe_model_path: __caffe_model_path, proto_text_path: __proto_text_path, confidence: __confidence);
            if (faceDetector.load_net_model() == null)
            {
                MessageBox.Show("[ERROR] Can't load OpenCV face caffe net model!");
                return;
            }

            MessageBox.Show(string.Format("[INFO] Starting video stream (src={0}) ...", __srcInput));
            ImageIO imageIO = new ImageIO(srcInput: __srcInput, resolution: __resolution);


            // starting detect face
            MessageBox.Show("[INFO] Starting face splitter... (press q to exit or esc to toggle predict one/multi)");
            int frameNumber = 0;
            bool predictOne = true;
            bool sortConfidence = true;

            Mat frame = new Mat();

            while (imageIO.HaveNextImage())
            {
                frame = imageIO.GrabImage();

                if (frame == null)
                {
                    MessageBox.Show("[ERROR] Can't grab frame!");
                    continue;
                }

                frameNumber ++;
                //checking for skip this frame
                if (IsProcessFrame(frameNumber))
                {
                    if (__measure != null)
                    {
                        __measure.Begin();
                    }

                    // Split faces

                    List<FacePrediction> facePredictions;
                    if (predictOne)
                    {
                        var facePrediction = faceDetector.Predict_one(frame);
                        facePredictions = new List<FacePrediction> {facePrediction};
                    }
                    else
                    {
                        facePredictions = faceDetector.Predict(frame);
                    }

                    long nanoseconds;
                    int fps;
                    if (__measure != null)
                    {
                        __measure.End();
                        nanoseconds = __measure.GetSpentTimeNs();
                        fps = (int)Math.Round(1e+9 / nanoseconds);
                        MessageBox.Show(string.Format("[INFO] Frame processing time {0} nanoseconds --> fps ~ {1}", nanoseconds, fps));
                    }
                    if (facePredictions.Count > 0)
                    {
                        if (sortConfidence == true)
                        {
                            //sort detected face with max confidence
                            facePredictions.Sort((e1, e2) => e2.Confidence.CompareTo(e1.Confidence));
                        }
                        else
                        {
                            //sort detected face with max roi
                            facePredictions.Sort((e1, e2) => ((e2.ROI[2] - e2.ROI[0]) * (e2.ROI[3] - e2.ROI[1])).CompareTo((e1.ROI[2] - e1.ROI[0]) * (e1.ROI[3] - e1.ROI[1])));
                        }
                        string path;

                        do
                        {
                            path = Path.Combine(__outputDir, $"{DateTime.Now.Ticks}.png");

                        } while (File.Exists(path));

                        var facePred = facePredictions[0];
                        ImageIO.Save_image(path, facePred.Face);

                        //draw the bounding box on the frame
                        var roi = facePred.ROI;
                        string label = facePred.Confidence.ToString("F4");
                        Cv2.PutText(frame, label, new OpenCvSharp.Point(roi[0], roi[1] - 10), HersheyFonts.HersheySimplex, 0.5 ,new Scalar(255,0,0), 2);
                        Cv2.Rectangle(frame, new OpenCvSharp.Point(roi[0], roi[1]), new OpenCvSharp.Point(roi[2], roi[3]), new Scalar(255, 0, 0), 2);
                    }
                    ImageIO.show_image(__WINDOW_CAPTION, frame);
                }
                int key = Cv2.WaitKey(1) & 0xFF;
                if (key == 'q')
                {
                    break;
                }
                else
                {
                    if (key == 27)
                    {
                        predictOne = !predictOne;
                    }
                }
            }
            ImageIO.Destroy_all_windows();
        }

    }
}

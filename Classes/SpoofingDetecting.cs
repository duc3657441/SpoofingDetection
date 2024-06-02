using Newtonsoft.Json.Linq;
using OpenCvSharp;
using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpoofingDetectionWinformApp.Classes
{
    // Detect live/spoof face on real camera or video file.
    public class SpoofingDetecting
    {
        private string __srcInput;
        private string __videoOutput;
        private string __cnnNet;
        private double __confidence;
        private OpenCvSharp.Size __imgResolution;
        private int __livePredictions;
        private int __spoofPredictions;
        private int __totalFramePredictions;
        private double __totalTimePredictions;
        private string __modelPath;
        public event Action<string> LogMessageEvent;

        protected virtual void OnLogMessage(string message)
        {
            LogMessageEvent?.Invoke(message);
        }

        public SpoofingDetecting(
            string srcInput,
            string videoOutput,
            string cnnNet,
            float confidence = 0,
            OpenCvSharp.Size resolution = default)
        {
            if (confidence == 0)
            {
                this.__confidence = Config.DEFAULT_CONFIDENCE;
            }
            this.__srcInput = srcInput;
            this.__videoOutput = videoOutput;
            this.__cnnNet = cnnNet;
            this.__imgResolution = resolution;
            this.__livePredictions = 0;
            this.__spoofPredictions = 0;
            this.__totalFramePredictions = 0;
            this.__totalTimePredictions = 0;
        }
        ~SpoofingDetecting()
        {
            this.__srcInput = null;
            this.__videoOutput = null;
            this.__cnnNet = null;
            this.__imgResolution = default;
            this.__livePredictions = 0;
            this.__spoofPredictions = 0;
            this.__totalFramePredictions = 0;
            this.__totalTimePredictions = 0;
        }

        public string __get_window_caption(bool predictOne)
        {
            if (predictOne)
            {
                return $"Face {__cnnNet} prediction (one) - (press q to exit or esc to toggle predict one/multi)";
            }
            return $"Face {__cnnNet} prediction (multi) - (press q to exit or esc to toggle predict one/multi)";
        }

        public void Start(PictureBox picture, bool flag = true)
        {
            OnLogMessage(string.Format("[INFO] Loading {0} model ...", __cnnNet));
            var spoofingDetector = new SpoofingDetector(__cnnNet);
            //var check = spoofingDetector.LoadModel();
            if (spoofingDetector.LoadModel() == null)
            {
                MessageBox.Show(string.Format("[ERROR] Can't load {0} model!", __cnnNet));
                return;
            }

            OnLogMessage(string.Format("[INFO] Starting video stream (src = {0}) ...",__srcInput));
            ImageIO imageIO = new ImageIO(srcInput: __srcInput, videoOutput: __videoOutput, resolution: __imgResolution);
            var measure = new Measure();
            var predictOne = true;

            // starting detect face spoofing
            OnLogMessage("[INFO] Starting face spoofing detector... (press q to exit or esc to toggle predict one/multi)\n");
            // loop over frames from the video stream

            while (imageIO.HaveNextImage() && flag == true)
            {
                //grab the frame
                var frame = imageIO.GrabImage();
                float width = frame.Width;
                float height = frame.Height;
                if (frame == null)
                {
                    MessageBox.Show("[ERROR] Can't grab frame!");
                    return;
                }
                measure.Begin();

                //detect face spoofing
                var spoofingPredictions = spoofingDetector.Predict(frame, predictOne);

                measure.End();
                if (spoofingPredictions != null)
                {
                    var nanoseconds = measure.GetSpentTimeNs();
                    __totalTimePredictions += nanoseconds;
                    __totalFramePredictions += 1;
                    int fps = (int)Math.Round(1e9 / nanoseconds);
                    OnLogMessage(string.Format("[INFO] Frame processing time {0} nanoseconds --> fps ~ {1}", nanoseconds, fps));
                    foreach (var spoofingPred in spoofingPredictions)
                    {
                        //red color for "spoofing" or blue color for "live"
                        var roi = spoofingPred.ROI;
                        var label = spoofingPred.Label;
                        var confidence = spoofingPred.Confidence;
                        OnLogMessage(string.Format("[INFO]\t\tPredictions: {0}: {1:.4f}", label, confidence));
                        Scalar color = new Scalar(255, 0, 0);
                        if (label == Config.CLASSES[0])
                        {
                            color = new Scalar(0, 0, 225);
                            __spoofPredictions += 1;
                        }
                        else
                        {
                            __livePredictions += 1;
                        }
                        label = label + " " + confidence;
                        //draw the label and bounding box on the frame
                        Cv2.PutText(frame, label, new OpenCvSharp.Point(roi[0] * (width / 300), (roi[1] * (height / 300)) - 10), HersheyFonts.HersheySimplex, 0.5, color, 2);
                        Cv2.Rectangle(frame, new OpenCvSharp.Point(roi[0] * (width / 300), roi[1] * (height / 300)), new OpenCvSharp.Point(roi[2] * (width / 300), roi[3] * (height / 300)), color, 2);
                    }
                }
                
                //show frame
                var windowCaption = __get_window_caption(predictOne);
                //ImageIO.show_image(windowCaption, frame);
                ImageIO.Show_Image_Picturebox(frame, picture);

                //save frame into video
                imageIO.SaveFrame(frame, windowCaption);

                //checking for key press
            }
            ImageIO.Destroy_all_windows();
        }

    }
}

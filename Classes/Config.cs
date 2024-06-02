using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpoofingDetectionWinformApp.Classes
{
    public class Config
    {
        public const string MODEL_DIR = "Model";
        public const string LOG_DIR = "Logs";
        public const string DATA_SET_DIR = "DataSet";
        public string TRAINING_DATA_DIR = Path.Combine(DATA_SET_DIR, "Train");
        public string TESTING_DATA_DIR = Path.Combine(DATA_SET_DIR, "Test");

        public static string OPENCV_FACE_CAFFE_MODEL = "res10_300x300_ssd_iter_140000.caffemodel";
        public static string OPENCV_FACE_CAFFE_PROTOTXT = "deploy.prototxt";

        public string SPOOFNING_NET = "spoofingnet";
        public string SPOOFNING_MOBILE_NET = "mobilenet";
        public string SPOOFNING_RES_NET = "resnet";
        public string SPOOFNING_VGG = "vgg";

        public static readonly string[] CLASSES = new string[]{
            "spoof",
            "live"
        };

        public static readonly int[] DEFAULT_RESOLUTION = new int[] { 800, 600 };
        public static readonly OpenCvSharp.Size VIDEO_OUT_DEFAULT_RESOLUTION = new OpenCvSharp.Size(800, 600);
        public static string VIDEO_OUT_DEFAULT_FOURCC = "MP4V";
        public const float VIDEO_OUT_DEFAULT_FPS = 16.0f;
        public static float DEFAULT_CONFIDENCE = 0.5f;
        public static int DEFAULT_BATCH_SIZE = 8;
        public static int DEFAULT_EPOCHS = 50;
        public const double INIT_LR = 1e-4;
        //public static int FACE_HEIGHT = 56;
        //public static int FACE_WIDTH = 56;
        public static int FACE_DEPTH = 3;
    }
}

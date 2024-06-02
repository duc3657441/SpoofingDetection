using OpenCvSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using OpenCvSharp.Extensions;
using Microsoft.ML.Data;

namespace SpoofingDetectionWinformApp.Classes
{
    // Class Image_io,    Grab a video frame from video file or web-cam.    Save a frame to the video file
    
    public class ImageIO
    {
        private VideoCapture? __videoStream;
        private VideoWriter? __videoWriter;
        private IEnumerator<string>? __imagePathIterator;
        private List<string>? __imagePathList = new List<string>();
        private OpenCvSharp.Size __resolution = new OpenCvSharp.Size(Config.DEFAULT_RESOLUTION[0], Config.DEFAULT_RESOLUTION[1]);
        private bool __haveNext = false;
        private string[] imageExtensions = { "*.jpg", "*.jpeg", "*.png", "*.bmp", "*.gif" };
        public event Action<string> LogMessageEvent;

        protected virtual void OnLogMessage(string message)
        {
            LogMessageEvent?.Invoke(message);
        }

        public ImageIO(string srcInput = null,
                        string videoOutput = null,
                        OpenCvSharp.Size resolution = default,
                        int fps = 0,
                        bool shuffle = false)
        {
            //video stream

            if (string.IsNullOrEmpty(srcInput))
            {
                OnLogMessage("[INFO] Starting video stream from cam 0...");
                __videoStream = new VideoCapture(0);
                __imagePathIterator = null;
            }
            else if (int.TryParse(srcInput, out _))
            {
                OnLogMessage(string.Format("[INFO] Starting video stream from cam {0} ...", srcInput));
                __videoStream = new VideoCapture(int.Parse(srcInput));
                __imagePathIterator = null;
            }
            else if (File.Exists(srcInput))
            {
                OnLogMessage(string.Format("[INFO] Starting video stream from file {0}...", srcInput));
                __videoStream = new VideoCapture(srcInput);
                __imagePathIterator = null;
            }
            else if (Directory.Exists(srcInput))
            {
                OnLogMessage(string.Format("[INFO] Starting image from folder {0}...", srcInput));
                __videoStream = null;
                //this.__imagePathList = Directory.GetFiles(srcInput, "*.jpg");

                foreach (string extension in imageExtensions)
                {
                    var imagePathList = Directory.GetFiles(srcInput, extension);
                    
                    foreach (string imagePath in imagePathList)
                    {
                        __imagePathList.Add(imagePath);
                    }

                    if (shuffle)
                    {
                        var random = new Random(42);
                        __imagePathList = __imagePathList.OrderBy(x => random.Next()).ToList();
                    }
                    __imagePathIterator = __imagePathList.GetEnumerator();
                }
               
            }

            // video writer
            if (string.IsNullOrEmpty(videoOutput))
            {
                __videoWriter = null;
            }
            else
            {
                string dirName = Path.GetDirectoryName(videoOutput);
                if (!Directory.Exists(dirName) && !string.IsNullOrEmpty(dirName))
                {
                    Directory.CreateDirectory(dirName);
                    
                }

                var videoOutputFps = fps > 0 ? fps : Config.VIDEO_OUT_DEFAULT_FPS;
                FourCC fourcc = FourCC.FromString(Config.VIDEO_OUT_DEFAULT_FOURCC);
                __videoWriter = new VideoWriter(videoOutput, fourcc, videoOutputFps, Config.VIDEO_OUT_DEFAULT_RESOLUTION, true);

            }

            //resolution
            if (resolution != default)
            {
                __resolution = resolution;
            }
            else
            {
                __resolution = new OpenCvSharp.Size(Config.DEFAULT_RESOLUTION[0], Config.DEFAULT_RESOLUTION[1]);
            }
            Thread.Sleep(1000);
            __haveNext = __imagePathIterator != null || (__videoStream != null && __videoStream.IsOpened());
        }

        ~ImageIO()
        {
            if (__videoStream != null)
            {
                OnLogMessage("[INFO] Stopping video stream ...");
                
                __videoStream.Dispose();
                __videoStream.ThrowIfDisposed();
                __videoStream = null;
            }

            if (__videoWriter != null)
            {
                OnLogMessage("[INFO] Stopping video writer ...");
                
                __videoWriter.Dispose();
                __videoWriter.ThrowIfDisposed(); 
                __videoWriter = null;
            }
            Cv2.DestroyAllWindows();
        }

        public bool HaveNextImage()
        {
            return __haveNext;
        }

        public Mat GrabImage()
        {
            Mat image = new Mat();
            if (__videoStream != null)
            {
                __videoStream.Read(image);
            }
            if (__imagePathIterator != null)
            {
                if (__imagePathIterator.MoveNext())
                {
                    // image = new Mat(__imagePathIterator.Current);
                    image = Cv2.ImRead(__imagePathIterator.Current);
                }
                else
                {
                    image = null;
                    __haveNext = false;
                }
            }
            if (image == null)
            {
                    __haveNext = false;
            }
            else
            {
                if (image.Width > __resolution.Width)
                {
                    Cv2.Resize(image, image, new OpenCvSharp.Size(__resolution.Width, (int)(image.Height * ((double)__resolution.Width / image.Width))));
                }
            }
            return image;
        }
       
        public IEnumerable<Mat> GrabImageFromFile()
        {
            if (__imagePathList != null)
            {
                foreach (string imagePath in __imagePathList)
                {
                    yield return new Mat(imagePath);
                }
            }
            
        }

        public void Stream(PictureBox pictureBox)
        {
            Mat img = new Mat();

            if (__videoStream!= null)
            {
                while (true)
                {
                    __videoStream.Read(img);
                    pictureBox.Image = img.ToBitmap();
                }
            }
           
        }

        public void ShowVideo()
        {
            if (__videoStream != null)
            {
                if (!__videoStream.IsOpened())
                {
                    MessageBox.Show("Cannot open video");
                }

                using(Mat frame = new Mat())
                {
                    while (true)
                    {
                        __videoStream.Read(frame);

                        if (frame.Empty())
                        {
                            OnLogMessage("The video has been read");
                        }

                        Cv2.ImShow("Video", frame);
                        Cv2.WaitKey(10);

                        if (Cv2.WaitKey(1) == 'q')
                        {
                            break;
                        }
                    }
                }
            }
        }

        public void SaveFrame(Mat frame, string? caption = null)
        {

            if (__videoWriter != null)
            {

                if (caption != null)
                {
                    
                    Cv2.PutText(frame, caption, new OpenCvSharp.Point(30, 30), HersheyFonts.HersheySimplex, 0.8, new Scalar(127, 127, 0), 2);
                    
                }
                __videoWriter.Write(frame);
            }
        } 

        public static void show_image( string caption, Mat image)
        {
            //  Kiểm tra xem chiều rộng của hình ảnh có lớn hơn chiều rộng mặc định được cấu hình hay không
            if (Config.DEFAULT_RESOLUTION[0] < image.Width)
            {
                Cv2.Resize(image, image, new OpenCvSharp.Size(Config.DEFAULT_RESOLUTION[0], (int)(image.Height * ((double)Config.DEFAULT_RESOLUTION[0] / image.Width)) ));
            }
            Cv2.ImShow(caption, image);
            
        }

        public static void Show_Image_Picturebox(Mat image, PictureBox pictureBox ) 
        {
            if (Config.DEFAULT_RESOLUTION[0] < image.Width)
            {
                Cv2.Resize(image, image, new OpenCvSharp.Size(Config.DEFAULT_RESOLUTION[0], (int)(image.Height * ((double)Config.DEFAULT_RESOLUTION[0] / image.Width))));
            }
            
            Bitmap bitmapImage = image.ToBitmap();
            pictureBox.Image = bitmapImage;
        }

        

        public static void Destroy_Window(string caption)
        {
            Cv2.DestroyWindow(caption);
        }

        public static void Destroy_all_windows()
        {
            Cv2.DestroyAllWindows();
        }

        public static void Save_image(string path, Mat image)
        {
            Cv2.ImWrite(path, image);
            //MessageBox.Show("[INFO] Saved image to " + path);
        }
    }
}

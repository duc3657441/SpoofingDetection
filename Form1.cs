using OpenCvSharp;
using SpoofingDetectionWinformApp.Classes;
using System.IO;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace SpoofingDetectionWinformApp
{
    public partial class Form1 : Form
    {
        VideoCapture _capture = new VideoCapture(0);
        Mat _image = new Mat();
        public Form1()
        {
            InitializeComponent();

            

            var image = Cv2.ImRead("C:\\Users\\Duc\\Pictures\\Camera Roll\\1.jpg");
            //Cv2.ImShow("hinh", image);
            //FacePrediction p = new FacePrediction();
            //FaceDetector face_detector = new FaceDetector();
            //p = face_detector.Predict_one(image);
            //Cv2.ImShow("hinh", p.Face);
            //Window.ShowImages(p.Face);
            //string srcInput = @"D:\TaiLieuCu\HInh";



            //ImageIO image_Io = new ImageIO(srcInput: "C:\\Users\\Duc\\Videos\\Captures\\1.mp4");
            //image_Io.ShowVideo();


            //Lam viec qua thu muc hinh anh
            //Mat img = new Mat();
            //ImageIO image_Io = new ImageIO(srcInput: "D:\\DoAn\\Hinh");
            //img = image_Io.GrabImage();
            ////img = image_Io.GrabImage();
            //Cv2.ImShow("hinh", img);
            //string outputDir = @"D:\DoAn\OutputDir";
            //string path;
            //string fileName = $"{DateTime.Now.Ticks}.png";
            //MessageBox.Show(fileName);
            //do
            //{
            //    path = Path.Combine(outputDir, fileName);

            //} while (File.Exists(path));
            //MessageBox.Show(path);
            //ImageIO.Save_image(path, img);
            //image_Io.SaveFrame(img, "");
            //image_Io.show_image("hinh", img);

         

            //FaceSplitting faceSplitting = new FaceSplitting("D:\\DoAn\\OutputDir");
            //faceSplitting.Start();


        }
        Thread Thread;

        private void Form1_Load(object sender, EventArgs e)
        {
            
            //Thread = new Thread(new ThreadStart(CaptureVideo));
            //Thread.Start();
        }

        public void CaptureVideo()
        {

            ImageIO image_Io = new ImageIO();
            image_Io.Stream(pictureBox1);
  
               
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            // Thread.Abort();
        }
    }
}

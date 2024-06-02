﻿using OpenCvSharp;
using SpoofingDetectionWinformApp.Classes;
using System.IO;
using Tensorflow.Keras.Engine;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace SpoofingDetectionWinformApp
{
    public partial class Form1 : Form
    {
        VideoCapture _capture = new VideoCapture(0);
        Mat _image = new Mat();
        private Model _model;
        private bool flag = true;
        public Form1()
        {
            InitializeComponent();

            //var image = Cv2.ImRead("C:\\Users\\Duc\\Pictures\\Camera Roll\\1.jpg");
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


        }
        Thread Thread;

        private void Form1_Load(object sender, EventArgs e)
        {

            Thread = new Thread(new ThreadStart(CaptureVideo));
            Thread.Start();
        }

        public void CaptureVideo()
        {

            //ImageIO image_Io = new ImageIO();
            //image_Io.Stream(pictureBox1);

            /*// cat hinh anh tu camera
            FaceSplitting faceSplitting = new FaceSplitting("D:\\DoAn\\OutputDir");
            faceSplitting.LogMessageEvent += LogMessage;
            faceSplitting.Start(pictureBox1);
            */

            /*
            var image = Cv2.ImRead("D:\\TaiLieuCu\\HInh\\Anh.jpg");
            SpoofingDetector spoofingDetector = new SpoofingDetector("model.zip");
            spoofingDetector.LogMessageEvent += LogMessage;
            spoofingDetector.LoadModel();
            spoofingDetector.Predict(image);
            */

            // su dung camera check hinh anh live/spoof
            SpoofingDetecting spoofingDetecting = new SpoofingDetecting("0", "D:\\DoAn\\OutputDir\\dirName", "model.zip");
            spoofingDetecting.LogMessageEvent += LogMessage;
            spoofingDetecting.Start(pictureBox1, flag);

            //ImageIO imageIO = new ImageIO();
            //imageIO.ShowVideo();
            //FaceSplitting faceSplitting = new FaceSplitting("D:\\DoAn\\OutputDir", "D:\\TaiLieuCu\\SpoofingDetection\\DataSet\\Test\\NUAA\\ImposterRaw\\00");
            //faceSplitting.LogMessageEvent += LogMessage;
            //faceSplitting.Start(pictureBox1);
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            Thread.DisableComObjectEagerCleanup();
            Thread.Interrupt();
        }

        // Hàm để ghi thông báo vào RichTextBox
        private void LogMessage(string message)
        {
            if (InvokeRequired)
            {
                Invoke(new Action<string>(LogMessage), new object[] { message });
                return;
            }
            outputRichTextBox.AppendText(message + Environment.NewLine);
            outputRichTextBox.ScrollToCaret();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Application.Exit();
            Thread.DisableComObjectEagerCleanup();
            Thread.Interrupt();
        }

        private void btnCloseCamera_Click(object sender, EventArgs e)
        {
            flag = false;
        }
    }
}

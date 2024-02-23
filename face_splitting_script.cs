using SpoofingDetection.Classes;
using OpenCvSharp;

namespace SpoofingDetection
{
    class face_splitting_script
    {
        static void Main(string[] args)
        {
            var image = Cv2.ImRead("D:\\DoAn\\WorkSpace\\face-detection-using-opencvsharp\\Images\\duc.jpg");
            FacePrediction p = new FacePrediction();
            Face_detector face_detector = new Face_detector();
            p = face_detector.Predict_one(image);        

        }
    }

}
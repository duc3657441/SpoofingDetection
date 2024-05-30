using OpenCvSharp;

namespace SpoofingDetectionWinformApp.Classes
{
    public class SpoofingPredictions
    {
        public Mat Face { get; set; }
        public List<int> ROI { get; set; }
        public string Label { get; set; }
        public double Confidence { get; set; }
    }
}
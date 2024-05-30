namespace SpoofingDetectionWinformApp.Classes
{
    public class ImagePrediction
    {
        public string ImagePath { get; set; }
        public string Label { get; set; }
        public string PredictedLabel { get; set; }
        public float[] Score { get; set; }
    }
}
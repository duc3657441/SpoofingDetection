using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpoofingDetectionWinformApp.Classes
{
    public class SpoofingDetecting
    {
        private string srcInput;
        private string videoOutput;
        private string cnnNet;
        private double confidence;
        private string imgResolution;
        private int livePredictions;
        private int spoofPredictions;
        private int totalFramePredictions;
        private double totalTimePredictions;
        private string modelPath;
        private string lePath;

        public SpoofingDetecting(
            string srcInput,
            string videoOutput,
            string cnnNet,
            float confidence = 0,
            string resolution = null)
        {
            if (confidence == 0)
            {
                this.confidence = Config.DEFAULT_CONFIDENCE;
            }
            this.srcInput = srcInput;
            this.videoOutput = videoOutput;
            this.cnnNet = cnnNet;
            this.imgResolution = resolution;
            this.livePredictions = 0;
            this.spoofPredictions = 0;
            this.totalFramePredictions = 0;
            this.totalTimePredictions = 0;
        }
    }
}

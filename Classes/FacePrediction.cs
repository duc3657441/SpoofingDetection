using OpenCvSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpoofingDetectionWinformApp.Classes
{
    public class FacePrediction
    {
        public  Mat Face { get; set; }
        public  List<int> ROI { get; set; }
        public double Confidence { get; set; }
    }
}
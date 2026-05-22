using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities.SystemModule.ModelsModule
{
    public class Video_Detect_Model : Model 
    {
        public string Framework { get; set; } = "PyTorch/YOLO";
        public double AccuracyThreshold { get; set; } = 0.85;
    }
}

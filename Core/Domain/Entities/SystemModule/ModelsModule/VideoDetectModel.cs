namespace Domain.Entities.SystemModule.ModelsModule
{
    public class Video_Detect_Model : AIModel 
    {
        public string Framework { get; set; } = "PyTorch/YOLO";
        public double AccuracyThreshold { get; set; } = 0.85;
    }
}

using System.Text.Json.Serialization;

namespace FindGenderAPI.DTO
{
    public class DataDto
    {
        public string Name { get; set; }
        public string Gender { get; set; }
        public float Probability { get; set; }
        public int SampleSize { get; set; }
        public bool IsConfident { get; set; }
        public DateTime ProcessedAt { get; set; }
  
    }
}
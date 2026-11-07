namespace ST10296771_CLDV7112_Practicum.Models
{
    public class TelemetryViewModel
    {
        public string SensorId { get; set; } = "sensor-001";

        public List<TelemetryReadingDto> Readings { get; set; } = new();
        public string? Error { get; set; }
    }
}

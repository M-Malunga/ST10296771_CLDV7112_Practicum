namespace ST10296771_CLDV7112_Practicum.Models
{
    public class TelemetryReadingDto
    {
        public string PartitionKey { get; set; } = "";
        public DateTimeOffset? ReadingTimestamp { get; set; }
        public double MoisturePercent { get; set; }
        public double TemperatureCelsius { get; set; }
    }
}

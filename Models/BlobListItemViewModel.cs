namespace ST10296771_CLDV7112_Practicum.Models;

public class BlobListItemViewModel
{
    public string Name { get; set; } = "";
    public string ContentType { get; set; } = "";
    public long SizeBytes { get; set; }
    public DateTimeOffset? UploadedUtc { get; set; }

    public string Url { get; set; } = "";
    public bool IsImage =>
        ContentType.StartsWith("image/", StringComparison.OrdinalIgnoreCase);
}
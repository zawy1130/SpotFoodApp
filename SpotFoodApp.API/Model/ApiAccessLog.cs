public class ApiAccessLog
{
    public long LogId { get; set; }

    public string DeviceId { get; set; } = string.Empty;

    public string Endpoint { get; set; } = string.Empty;

    public string HttpMethod { get; set; } = string.Empty;

    public int? PoiId { get; set; }

    public int? StatusCode { get; set; }

    public DateTime CreatedAt { get; set; }
}
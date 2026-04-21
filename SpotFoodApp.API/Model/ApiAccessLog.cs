// Model/ApiAccessLog.cs
public class ApiAccessLog
{
    public int Id { get; set; }

    public string Endpoint { get; set; } = "";

    public string Method { get; set; } = "";

    public DateTime AccessedAt { get; set; }
}
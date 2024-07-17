namespace EagleEyeLogger.Models
{
    public class HttpLogRecord
    {
        public int Id { get; set; }
        public Guid UUID { get; set; }
        public DateTime TimeStamp { get; set; }
        public string Direction { get; set; } // "Incoming" or "Outgoing"
        public string Type { get; set; } // "Request" or "Response"

        public string Method { get; set; }
        public string Path { get; set; }
        public int? StatusCode { get; set; }
        public string Headers { get; set; }
        public string Body { get; set; }
        public string Exception { get; set; }
    }
}

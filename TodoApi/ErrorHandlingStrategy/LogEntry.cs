namespace TodoApi.ErrorHandlingStrategy
{
    public class LogEntry
    {
        public System.DateTime? Timestamp { get; set; }
        public string User { get; set; }
        public string RequestUri { get; set; }
        public string RequestMethod { get; set; }
        public int? ResponseStatusCode { get; set; }
        public double? TimeTaken { get; set; }
        public int RequestThreadId { get; set; }
        public string RequestIpAddress { get; set; }
        public string RequestContentType { get; set; }
        public string RequestContentBody { get; set; }
        public string ResponseContentType { get; set; }
        public string ResponseContentBody { get; set; }
    }
}
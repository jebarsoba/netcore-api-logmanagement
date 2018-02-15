using System;

namespace TodoApi.ErrorHandlingStrategy
{
    public class ErrorLogEntry
    {
        public DateTime? Timestamp { get; set; }
        public string User { get; set; }
        public string RequestUri { get; set; }
        public string RequestMethod { get; set; }
        public string ExceptionType { get; set; }
        public string ExceptionMessage { get; set; }
        public string RequestIpAddress { get; set; }
        public int RequestThreadId { get; set; }
        public string RequestContentType { get; set; }
        public string RequestContentBody { get; set; }
        public string ExceptionFullMessage { get; set; }
    }
}
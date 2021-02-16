namespace app.Models
{
    public class ReportResponse
    {
        public Report Counters { get; set; }
        public Report Duplicates { get; set; }
        public Report Errors { get; set; }
    }
    public class Report
    {
        public int Orders { get; internal set; }
        public int Payments { get; internal set; }
        public int Mails { get; internal set; }
        public int Events { get; internal set; }
    }
}

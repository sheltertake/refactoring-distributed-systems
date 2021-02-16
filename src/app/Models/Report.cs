using app.Entities;
using System.Collections.Generic;

namespace app.Models
{
    public class ReportResponse
    {
        public int Counter { get; set; }
        public Report Counters { get; set; }
        public Report Duplicates { get; set; }
        public Report Errors { get; set; }
        public Report Requests { get; internal set; }
        public int CounterErrors { get; internal set; }
    }
    public class Report
    {
        public int Orders { get; internal set; }
        public int Payments { get; internal set; }
        public int Mails { get; internal set; }
        public int Events { get; internal set; }
    }
    public class MockReportResponse
    {
        public int Errors { get; set; }
        public int Counter { get; set; }
        public IEnumerable<Order> Items { get; set; }
    }
}

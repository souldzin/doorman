using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Doorman.Web.Models
{
    public class HistoricData
    {
        public decimal DailyAvg { get; set; }
        public decimal DailyStdev { get; set; }
        public int DailyMax { get; set; }
        public string PeakWeekDay { get; set; }
        public string PeakTime { get; set; }

        public HistoricData(string StartDate = "", string EndDate = "", int ChartType = 1)
        {
            Random rnd = new Random();
            DailyAvg = rnd.Next(52);
            DailyStdev = rnd.Next(52);
            DailyMax = rnd.Next(52);
            PeakWeekDay = "Tuesday";
            PeakTime = "11:45 AM";
        }
    }
}
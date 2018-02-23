using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Doorman.Web.Models
{
    public class HistoricChartData
    {
        public List<int> Series { get; set; }
        public List<string> Categories { get; set; }

        public HistoricChartData(string StartDate = "", string EndDate = "", int ChartType = 1)
        {
            Random rnd = new Random();
            Categories = new List<string>()
            {"01/02/2018","01/05/2018","01/07/2018","01/10/2018", "01/11/2018","01/12/2018", "01/21/2018", "01/22/2018", "01/28/2018", "02/11/2018" };
            Series = new List<int>()
            {
                rnd.Next(52),
                rnd.Next(52),
                rnd.Next(52),
                rnd.Next(52),
                rnd.Next(52),
                rnd.Next(52),
                rnd.Next(52),
                rnd.Next(52),
                rnd.Next(52),
                rnd.Next(52)
            };
        }
    }

    public class ChartSeries
    {
        public string x { get; set; }
        public decimal y { get; set; }
    }
}
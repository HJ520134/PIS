using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDMS.Model.EntityDTO
{
    public class TwoHourCapacityDTO
    {
        public string Line { get; set; }
        public string Category { get; set; }
        public string Total { get; set; }
        public string HourFrom06 { get; set; }
        public string HourFrom08 { get; set; }
        public string HourFrom10 { get; set; }
        public string HourFrom12 { get; set; }
        public string HourFrom14 { get; set; }
        public string HourFrom16 { get; set; }
        public string HourFrom18 { get; set; }
        public string HourFrom20 { get; set; }
        public string HourFrom22 { get; set; }
        public string HourFrom00 { get; set; }
        public string HourFrom02 { get; set; }
        public string HourFrom04 { get; set; }
    }
}

using System;
using System.Collections.Generic;

namespace PDMS.Model
{
    public class EnumVM
    {
        public string Enum_Name { get; set; }
        public string Enum_Value { get; set; }
    }
    public class IntervalEnum
    {
        public string OpEnumType { get; set; }
        public string NowDate { get; set; }
        public string IntervalNo { get; set; }
        public string Time_Interval { get; set; }
        public DateTime BeginTime { get; set; }
        public DateTime EndTime { get; set; }
    }
}

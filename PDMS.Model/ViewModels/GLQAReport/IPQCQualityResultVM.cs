using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDMS.Model.ViewModels
{
    public class IPQCQualityResultVM:BaseModel
    {
        public string IndexName
        {
            get; set;
        }
        public string TimeInterval1
        {
            get; set;
        }
        public string TimeInterval2
        {
            get; set;
        }
       
        public string TimeInterval3
        {
            get; set;
        }
        public string TimeInterval4
        {
            get; set;
        }
        public string TimeInterval5
        {
            get; set;
        }
        public string TimeInterval6
        {
            get; set;
        }
        public string Toatal
        {
            get; set;
        }
    }

    public enum QualityType
    {
        [Description("一次目标良率")]
        FirstTargetYield = 0,
        [Description("一次良率")]
        FirstYield =1,
        [Description("二次目标良率")]
        SecondTargetYield = 3,
        [Description("二次良率")]
        SecondYield = 4,
        [Description("进料数")]
        InputNumber = 5,
        [Description("检验数")]
        TestNumber = 6,
        [Description("一次OK数")]
        FirstPassNumber = 7,
        [Description("二次OK数")]
        SecondPassNumber = 8,
        [Description("返修数")]
        RepairNumber = 9,
        [Description("NG数")]
        NGNumber =10,
        [Description("WIP")]
        WIP = 11,
    }


    public class IPQCQualityMonthVM : BaseModel
    {
        public string ProductDate
        {
            get; set;
        }
        public string FirstYield
        {
            get; set;
        }
        public string FirstTargetYield
        {
            get; set;
        }
        public string SecondYield
        {
            get; set;
        }
        public string SecondTargetYield
        {
            get; set;
        }
    }
}

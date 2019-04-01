using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDMS.Model.ViewModels
{
    public class QATargetRateVM
    {
        public int FlowChart_Master_UID { set; get; }
        public int Process_seq { set; get; }
        public string Process { set; get; }
        public DateTime Product_Date { get; set; }
        public int Flowchart_Detail_UID { set; get; }
        public string Color { set; get; }


        public decimal? MondayQAFirstRejectionRate { set; get; }
        public decimal? MondayQASecondRejectionRate { set; get; }

        public decimal? TuesdayQAFirstRejectionRate { set; get; }
        public decimal? TuesdayQASecondRejectionRate { set; get; }

        public decimal? WednesdayQAFirstRejectionRate { set; get; }
        public decimal? WednesdayQASecondRejectionRate { set; get; }

        public decimal? ThursdayQAFirstRejectionRate { set; get; }
        public decimal? ThursdayQASecondRejectionRate { set; get; }

        public decimal? FridayQAFirstRejectionRate { set; get; }
        public decimal? FridayQASecondRejectionRate { set; get; }

        public decimal? SaterdayQAFirstRejectionRate { set; get; }
        public decimal? SaterdayQASecondRejectionRate { set; get; }

        public decimal? SundayQAFirstRejectionRate { set; get; }
        public decimal? SundayQASecondRejectionRate { set; get; }
    }

    public class QATargetYieldVM : BaseModel
    {
        public int FlowChart_Master_UID { set; get; }
        public int Process_seq { set; get; }
        public string Process { set; get; }
        public string Product_Date { get; set; }
        public int Flowchart_Detail_UID { set; get; }
        public string Color { set; get; }

        public string MondayQAFirstRejectionRate { set; get; }
        public string MondayQASecondRejectionRate { set; get; }

        public string TuesdayQAFirstRejectionRate { set; get; }
        public string TuesdayQASecondRejectionRate { set; get; }

        public string WednesdayQAFirstRejectionRate { set; get; }
        public string WednesdayQASecondRejectionRate { set; get; }

        public string ThursdayQAFirstRejectionRate { set; get; }
        public string ThursdayQASecondRejectionRate { set; get; }

        public string FridayQAFirstRejectionRate { set; get; }
        public string FridayQASecondRejectionRate { set; get; }

        public string SaterdayQAFirstRejectionRate { set; get; }
        public string SaterdayQASecondRejectionRate { set; get; }

        public string SundayQAFirstRejectionRate { set; get; }
        public string SundayQASecondRejectionRate { set; get; }
    }

    public class QAMgDataListVM
    {
        public List<QualityAssurance_MgDataDTO> QAMgDataList = new List<QualityAssurance_MgDataDTO>();
    }
}

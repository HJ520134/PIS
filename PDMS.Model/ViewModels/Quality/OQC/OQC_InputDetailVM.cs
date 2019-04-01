using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDMS.Model
{
    public class OQC_InputDetailVM : BaseModel
    {
        public Nullable<int> ExceptionType_UID { get; set; }
        public int OQCDetail_UID { get; set; }
        public Nullable<int> OQCMater_UID { get; set; }
        public string FunPlant { get; set; }
        public string TypeClassify { get; set; }
        public int Qty { get; set; }
        public Nullable<int> Creator_UID { get; set; }
        public Nullable<System.DateTime> Create_Date { get; set; }
        public Nullable<int> Modified_UID { get; set; }
        public System.DateTime Modified_Date { get; set; }
        public string ExcetionTypeName { set; get; }
        public bool CanModify { set; get; }

        public string SunDefectRate { get { return (DailyRate*100).ToString("F4")+"%"; } }
        public decimal DailyRate { set; get; }
        public string NightDefectRate { get { return (NightRate * 100).ToString("F4") + "%"; } }
        public decimal NightRate { set; get; }
        public string DayDefectRate { get { return (DayRate * 100).ToString("F4") + "%"; } }
        public decimal DayRate { set; get; }
    }



    public class ExceptionTypeWithFlowchartVM: BaseModel
    {
        public int ExceptionTypeWithFlowchart_UID { get; set; }
        public int FlowChart_Detail_UID { get; set; }
        public int ExceptionType_UID { get; set; }
        public string FunPlant { get; set; }
        public Nullable<int> Creator_UID { get; set; }
        public System.DateTime Creator_Date { get; set; }
        public string TypeClassify { get; set; }

        public int FlowChart_Master_UID { get; set; }
        public int Process_Seq { set; get; }

    }

    public class ExceptionTypesAddToFlowChartVM:BaseModel
    {
        public List<ExceptionTypeWithFlowchartVM> ExcetionTypes = new List<ExceptionTypeWithFlowchartVM>();
    }


}

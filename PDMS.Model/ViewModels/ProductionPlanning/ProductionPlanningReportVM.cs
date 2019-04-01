using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDMS.Model.ViewModels.ProductionPlanning
{
    public class ProductionPlanningReportVM : BaseModel
    {
        //厂区
        public int PlantUID { get; set; }
        //OpType
        public int OpTypeUID { get; set; }
        //专案
        public int ProjectUID { get; set; }
        //部件
        public int PartTypeUID { get; set; }
        //报表类型
        public int ReportType { get; set; }
        //时间
        public string DateFrom { get; set; }
        //开始日期
        public string StartDate { get; set; }
        //结束日期
        public string EndDate { get; set; }
        //查询模式
        public int QueryMode { get; set; }
        //标签页
        public string hidTab { get; set; }
        //子标签页
        public string hidSubTab { get; set; }
    }

    public class ProductionPlanningReportGetProject : BaseModel
    {
        public Dictionary<int, string> plantDir { get; set; }
        public Dictionary<int, string> opTypeDir { get; set; }
        public Dictionary<int, string> partTypeDir { get; set; }
        public Dictionary<int, string> FunPlantDir { get; set; }
    }

    //不指定OPType的查询的结果集返回对象
    public class ProductionPlanningReportSearchVM : BaseModel
    {
        public string FunPlant { get; set; }

        public string Equipment_Name { get; set; }

        public int? OP1 { get; set; }

        public int? OP2 { get; set; }

        public int? OP3 { get; set; }

        public string Product_Date {get;set;}

        public int? Request_OP1 { get; set; }

        public int? Request_OP2 { get; set; }

        public int? Request_OP3 { get; set; }

        //统计单个OP的情况
        public int? Organization_UID { get; set; }
        public int? MP_CurrentQty { get; set; }
        public int? NPI_CurrentQty { get; set; }
        public int? Request_MPANDNPI { get; set; }
        public int? System_FunPlant_UID { get; set; }
    }

    public class ReportBy_SingleProject : BaseModel
    {
        public string Process { get; set; }
        public string Pink { get; set; }
        public int Input { get; set; }
        public string Product_Date { get; set; }
    }

    public class ReportByProject : BaseModel
    {
        public int Organization_UID { get; set; }
        public int System_FunPlant_UID { get; set; }
        public string time { get; set; }
        public int FlowChart_Master_UID { get; set; }
        public int FlowChart_Version { get; set; }
        public string Project_Name { get; set; }
        public string Product_Phase { get; set; }
        public string Equipment_Name { get; set; }
        public int Input { get; set; }
        public int Request_MPANDNPI { get; set; }
        public int QueryMode { get; set; }
    }

    public class ReportByPlant : BaseModel
    {
        public string Optype { get; set; }
        public string FunPlant { get; set; }
        public string Equipment_Name { get; set; }
        public string Product_Date { get; set; }
        public int CurrentQty { get; set; }
        public string QtyMode { get; set; }

    }

    public class ReportByDatePhase
    {
        public string StartDate { get; set; }
        public string EndDate { get; set; }
    }

    public class ReportDate
    {
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }

    }


}

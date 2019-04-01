using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDMS.Model.ViewModels
{
    public class PPForQAInterfaceModel : BaseModel
    {
        public Nullable<int> Flowchart_Master_UID { set; get; }
        public Nullable<int> Process_Seq { set; get; }
        public Nullable<int> Input_Qty { set; get; }
        public Nullable<int> NG_Qty { set; get; }
        public DateTime Product_date { set; get; }
        public string Time_interval { set; get; }
        public string Color { set; get; }
        public string MaterielType { set; get; }
    }

    public class QAMasterVM : BaseModel
    {
        public int QualityAssurance_InputMaster_UID { get; set; }
        public Nullable<int> FlowChart_Detail_UID { get; set; }
        public string Process { get; set; }
        public string Color { get; set; }
        public string MaterialType { get; set; }
        public System.DateTime Product_Date { get; set; }
        public string Time_Interval { get; set; }
        public Nullable<int> Input { get; set; }
        public Nullable<int> FirstCheck_Qty { get; set; }
        public Nullable<int> FirstOK_Qty { get; set; }
        public string FirstRejectionRate { get; set; }
        public Nullable<int> NG_Qty { get; set; }
        public Nullable<int> SurfaceSA_Qty { get; set; }
        public Nullable<int> SizeSA_Qty { get; set; }
        public Nullable<int> RepairCheck_Qty { get; set; }
        public Nullable<int> RepairOK_Qty { get; set; }
        public Nullable<int> Shipment_Qty { get; set; }
        public Nullable<int> WIPForCheck_Qty { get; set; }
        public int Creator_UID { get; set; }
        public System.DateTime Create_Date { get; set; }
        public int Modified_UID { get; set; }
        public System.DateTime Modified_Date { get; set; }
        public bool NGFlag { get; set; }
        public bool FirstCheckFlag { get; set; }

        public bool CanModify { set; get; }
        /// <summary>
        /// 置换数量
        /// </summary>
        public Nullable<int> Displace_Qty { set; get; }
        public bool DisplaceFlag { set; get; }

    }

    public class IPQCInputDataVM
    {
        public QualityAssurance_InputMasterDTO MasterData = new QualityAssurance_InputMasterDTO();
        public List<QAInputDetailVM> DetailList = new List<QAInputDetailVM>();
    }

    public class IPQCDailyDataVM
    {
        public List<IPQCInputDataVM> DataList = new List<IPQCInputDataVM>();
    }

}

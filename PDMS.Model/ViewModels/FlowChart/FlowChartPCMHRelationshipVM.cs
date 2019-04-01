using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDMS.Model.ViewModels
{
    #region Excel批量导入
    public class FlowChartPCMHRelationshipVM : BaseModel
    {
        public string UserNTID { get; set; }

        public SystemUserDTO UserDTOItem { get; set; }

        public SystemUserRoleDTO UserRoleDTOItem { get; set; }

        public FlowChartPCMHRelationshipDTO FlowchartPCDTOItem { get; set; }

        public bool isAdd { get; set; }
    }

    public class EXLResult : BaseModel
    {
        public int Process_Seq { get; set; }
        public string Process { get; set; }
        public string Place { get; set; }
        public string Color { get; set; }
        public int FlowChart_Detail_UID { get; set; }
        public string User_Name { get; set; }
    }

    public class FlowChartDetailAndMgData
    {
        public int FlowChart_Detail_UID { get; set; }
        public int FlowChart_Master_UID { get; set; }
        public int System_FunPlant_UID { get; set; }
        public int Process_Seq { get; set; }
        public string DRI { get; set; }
        public string Place { get; set; }
        public string Process { get; set; }
        public int Product_Stage { get; set; }
        public string Color { get; set; }
        public string Process_Desc { get; set; }
        public string Material_No { get; set; }
        public int FlowChart_Version { get; set; }
        public string FlowChart_Version_Comment { get; set; }
        public Nullable<int> FatherProcess_UID { get; set; }
        public string FatherProcess { get; set; }
        public string Rework_Flag { get; set; }
        public string IsQAProcess { get; set; }
        public string IsQAProcessName { get; set; }
        public List<FlowChartMgDataDTO> MgDataList { get; set; }
    }

    #endregion
}

using Newtonsoft.Json;
using PDMS.Common.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDMS.Model.EntityDTO
{

    //用于主画面查询
    public class FixtureRepairSearch : EntityDTOBase
    {
        public int Fixture_Repair_M_UID { get; set; }

        public int Plant_Organization_UID { get; set; }

        public int BG_Organization_UID { get; set; }
        public int? FunPlant_Organization_UID { get; set; }
        public string Plant_Organization_Name { get; set; }
        public string BG_Organization_Name { get; set; }
        public string FunPlant_Organization_Name { get; set; }
        public string Repair_NO { get; set; }  
        public string Repair_Location_Name { get; set; }
        public int Repair_Location_UID{get;set;}
        public int? Status { get; set; }
        public string StatusName { get; set; }
        public int? Workstation_UID { get; set; }
        public int? Workshop_UID { get; set; }
        public string Project { get; set; }
        public string Workshop { get; set; }
        public string Workstation { get; set; }
        public int? Production_Line_UID { get; set; }
        public string Process_Info { get; set; }
        public int? Process_Info_UID { get; set; }
        public string Production_Line_Name { get; set; }
        public string Equipment_No { get; set; }
        public string Fixture_NO { get; set; }
        public int? Vendor_Info_UID { get; set; }
        public string Vendor_Info { get; set; }
        public string Fixture_Name { get; set; }
        public string ShortCode { get; set; }
        public int? Fixture_Machine_UID { get; set; }
        public string Machine_Name { get; set; }
        public System.DateTime Created_Date { get; set; }
        new public int? Modified_UID { get; set; }
        new public string Modified_UserNTID { get; set; }
        /// <summary>
        /// 创建者代號
        /// </summary>
        public int Created_UID { get; set; }
        /// <summary>
        /// 治具狀態
        /// </summary>
        //public int Status { get; set; }
        /// <summary>
        /// 时间段开始
        /// </summary>
        public System.DateTime? End_Date_From { get; set; }
        /// <summary>
        /// 时间段结束
        /// </summary>
        public System.DateTime? End_Date_To { get; set; }
        /// <summary>
        /// 是否编辑
        /// </summary>
        //public bool IsEdit { get; set; }
    }
    //用于主画面显示
    public class FixtureRepairDTO : EntityDTOBase
    {
        public int Fixture_Repair_D_UID { get; set; }
        public int Fixture_Repair_M_UID { get; set; }

        public int Plant_Organization_UID { get; set; }

        public int BG_Organization_UID { get; set; }
        public int? FunPlant_Organization_UID { get; set; }
        public string Plant_Organization_Name { get; set; }
        public string BG_Organization_Name { get; set; }
        public string FunPlant_Organization_Name { get; set; }
        public string Repair_NO { get; set; }
        public string Repair_Location_Name { get; set; }
        public int Repair_Location_UID { get; set; }
        public int Status { get; set; }
        public string StatusName { get; set; }
        public int? Workstation_UID { get; set; }
        public int? Workshop_UID { get; set; }
        public string Project { get; set; }
        public string Workshop { get; set; }
        public string Workstation { get; set; }
        public int? Production_Line_UID { get; set; }
        public string Process_Info { get; set; }
        public int? Process_Info_UID { get; set; }
        public string Production_Line_Name { get; set; }
        public int? Fixture_Machine_UID { get; set; }
        public string Machine_Name { get; set; }
        public string Equipment_No { get; set; }
        public string Fixture_NO { get; set; }
        public string Fixture_Unique_ID { get; set; }
        public int? Vendor_Info_UID { get; set; }
        public string Vendor_Info { get; set; }

        public string Fixture_Name { get; set; }
        public string ShortCode { get; set; }
        public System.DateTime Created_Date { get; set; }
        /// <summary>
        /// 创建者代號
        /// </summary>
        public int Created_UID { get; set; }
        public string ModifyUserName { get; set; }
        /// <summary>
        /// 治具狀態
        /// </summary>
        //public int Status { get; set; }
        /// <summary>
        /// 时间段开始
        /// </summary>
        public System.DateTime? End_Date_From { get; set; }
        /// <summary>
        /// 时间段结束
        /// </summary>
        public System.DateTime? End_Date_To { get; set; }
        /// <summary>
        /// 是否编辑
        /// </summary>
        //public bool IsEdit { get; set; }
    }


    //用于单个查询编辑保存

    public class FixtureRepairItem: EntityDTOBase
    {   
        public int Fixture_Repair_M_UID { get; set; }
        public int Plant_Organization_UID { get; set; }

        public int BG_Organization_UID { get; set; }

        public int? FunPlant_Organization_UID { get; set; }


        public string Plant_Organization_Name { get; set; }

        public string BG_Organization_Name { get; set; }

        public string FunPlant_Organization_Name { get; set; }

        public string Repair_NO { get; set; }
        public string Repair_Location_Name { get; set; }
        public int Repair_Location_UID { get; set; }
        [JsonConverter(typeof(SPPDateTimeConverter))]
        public DateTime SentOut_Time { get; set; }
        public string SentOut_Number { get; set; }
        public string SentOut_Name { get; set; }
        public int Receiver_UID { get; set; }
        public string Receiver_Name { get; set; }
        List<FixtureRepairDItem> FixtureRepairDItems{ get; set; }
    }


    //用于维修明细 
    public class FixtureRepairD_DTO :EntityDTOBase
    {
        public int Fixture_Repair_D_UID { get; set; }
        public int Repair_Staff_UID { get; set; }
        public string Repair_Staff_Name { get; set; }
        public string Fixture_Unique_ID { get; set; }
        public string Fixture_Name { get; set; }
        public int Status { get; set; }
        public string StatusName { get; set; }
        public DateTime Completion_Date { get; set; }
  
    }

    public class FixtureRepairDItem : EntityDTOBase
    {
        public int Fixture_Repair_D_UID { get; set; }
        public int Repair_Staff_UID { get; set; }
        public string Repair_Staff_Name { get; set; }
        public string Fixture_Unique_ID { get; set; }
        public string Fixture_Name { get; set; }
        public int Status { get; set; }
        public string StatusName { get; set; }
        public DateTime Completion_Date { get; set; }
        List<FixtureRepairSolutionItem> FixtureRepairSoulutionItems { get; set; }
    }


    //用于维修原因及对策
    public class FixtureRepairSolutionItem : EntityDTOBase
    {
        public int Fixture_Repair_D_Defect_UID { get; set; }
        public int Defect_Code_UID { get; set; }
        public string Defect_Code_Name { get; set; }
   
        public int Solution_UID { get; set; }
        public string Solution_Name { get; set; }
  
    }

    public class FixtureRepairSolution_DTO : EntityDTOBase
    {
        public int Fixture_Repair_D_Defect_UID { get; set; }
        public int Defect_Code_UID { get; set; }
        public string Defect_Code_Name { get; set; }

        public int Solution_UID { get; set; }
        public string Solution_Name { get; set; }

    }

}

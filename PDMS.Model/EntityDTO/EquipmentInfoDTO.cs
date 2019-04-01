using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using PDMS.Common.Helpers;

namespace PDMS.Model.EntityDTO
{
    public class EquipmentInfoDTO : EntityDTOBase
    {
        public int EQP_Uid { get; set; }
        public string CoCd { get; set; }
        public string OpU { get; set; }
        public string C { get; set; }
        public string Process_Group { get; set; }
        public string Class { get; set; }
        public string Class_Desc { get; set; }
        public string Mfg_Of_Asset { get; set; }
        public string Model_Number { get; set; }
        public string Mfg_Serial_Num { get; set; }
        public string Mfg_Part_Number { get; set; }
        public string Equipment { get; set; }
        public string Asset { get; set; }
        public string User_Status { get; set; }
        public string AM_CostCtr { get; set; }
        public string Description_1 { get; set; }
        public Nullable<System.DateTime> Cap_date { get; set; }
        public decimal Acquisition_Value { get; set; }
        public int Asset_Life { get; set; }
        public decimal Net_Book_Value { get; set; }
        public decimal Monthly_Depreciation { get; set; }
        public int Remaining_Life { get; set; }
        public string Func_Loc { get; set; }
        public string Room { get; set; }
        public string MCtry { get; set; }
        public decimal Weight { get; set; }
        public string Un { get; set; }
        public string Size_dimension { get; set; }
        public string ConY { get; set; }
        public string CM { get; set; }
        public string Int_Note_L2 { get; set; }
        public string Description_2 { get; set; }
        public string Characteristic_1 { get; set; }
        public string Description_3 { get; set; }
        public string Characteristic_2 { get; set; }
        public string Description_4 { get; set; }
        public string Characteristic_3 { get; set; }
        public string Description_5 { get; set; }
        public string Characteristic_4 { get; set; }
        public string Description_6 { get; set; }
        public string Characteristic_5 { get; set; }
        public string Int_Note_L1 { get; set; }
        public string EQP_Plant_No { get; set; }
        public string EQP_Location { get; set; }
        public int System_FunPlant_UID { get; set; }
        public Nullable<int> Project_UID { get; set; }
        public string process { get; set; }
        public string OP_TYPES { get; set; }
        public string ProductDate { get; set; }

        public Nullable<int> Organization_UID { get; set; }
        public string  Project_Name {get;set;}
        public string FunPlant { get; set; }
        public int Modified_UID { get; set; }
        public System.DateTime Modified_Date { get; set; }

        public int  Plant_Organization_UID { get; set; }

        public int BG_Organization_UID { get; set; }
        public int? FunPlant_Organization_UID { get; set; }

    }

    public class EquipmentInfoSearchDTO : EntityDTOBase
    {

        public int EQP_Uid { get; set; }
        public int Plant_OrganizationUID { get; set; }
        public string CoCd { get; set; }
        public string OpU { get; set; }
        public string C { get; set; }
        public string Process_Group { get; set; }
        public string Class { get; set; }
        public string Class_Desc { get; set; }
        public string Mfg_Of_Asset { get; set; }
        public string Model_Number { get; set; }
        public string Mfg_Serial_Num { get; set; }
        public string Mfg_Part_Number { get; set; }
        public string Equipment { get; set; }
        public string Asset { get; set; }
        public string User_Status { get; set; }
        public string AM_CostCtr { get; set; }
        public string Description_1 { get; set; }
        public Nullable<System.DateTime> Cap_date { get; set; }
        public decimal Acquisition_Value { get; set; }
        public int Asset_Life { get; set; }
        public decimal Net_Book_Value { get; set; }
        public decimal Monthly_Depreciation { get; set; }
        public int Remaining_Life { get; set; }
        public string Func_Loc { get; set; }
        public string Room { get; set; }
        public string MCtry { get; set; }
        public decimal Weight { get; set; }
        public string Un { get; set; }
        public string Size_dimension { get; set; }
        public string ConY { get; set; }
        public string CM { get; set; }
        public string Int_Note_L2 { get; set; }
        public string Description_2 { get; set; }
        public string Characteristic_1 { get; set; }
        public string Description_3 { get; set; }
        public string Characteristic_2 { get; set; }
        public string Description_4 { get; set; }
        public string Characteristic_3 { get; set; }
        public string Description_5 { get; set; }
        public string Characteristic_4 { get; set; }
        public string Description_6 { get; set; }
        public string Characteristic_5 { get; set; }
        public string Int_Note_L1 { get; set; }
        public string EQP_Plant_No { get; set; }
        public string EQP_Location { get; set; }
        public int System_FunPlant_UID { get; set; }
        public Nullable<int> Project_UID { get; set; }
        public string process { get; set; }
        public string OP_TYPES { get; set; }
        public string ProductDate { get; set; }

        public Nullable<int> Organization_UID { get; set; }
        public string Project_Name { get; set; }
        public string FunPlant { get; set; }
        public int Modified_UID { get; set; }
        public System.DateTime Modified_Date { get; set; }
    }

    public class EqumentOrgInfo
    {
        public int BG_Organization_UID { get; set; }
        public Nullable<int> FunPlant_Organization_UID { get; set; }
        public Nullable<int> Project_UID { get; set; }

        public string  OP { get; set; }
        public string FuncPlant { get; set; }
        public string Project { get; set; }
    }
}

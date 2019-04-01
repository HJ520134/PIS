using Newtonsoft.Json;
using PDMS.Common.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDMS.Model.ViewModels.Fixture
{
    public class FixturePart_OrderVM : BaseModel
    {
        public int Fixture_Part_Order_M_UID { get; set; }
        public int Plant_Organization_UID { get; set; }
        public int BG_Organization_UID { get; set; }
        public int? FunPlant_Organization_UID { get; set; }
        public string Order_ID { get; set; }
        public string Part_ID { get; set; }
        [JsonConverter(typeof(SPPDateTimeConverterToDate))]
        public DateTime? Order_Date { get; set; }
        public bool Is_Complated { get; set; }
        public string Is_ComplatedValue { get; set; }
        public string Search_Is_Submit { get; set; }
        public string Remarks { get; set; }
        public bool Del_Flag { get; set; }
        public int Del_Flag_Value { get; set; }
        public int Created_UID { get; set; }
        public string Created_Name { get; set; }
        public DateTime? SubMit_Date { get; set; }
        public DateTime Created_Date { get; set; }
        public string PlantName { get; set; }
        public string OpType_Name { get; set; }
        public string Func_Name { get; set; }
        public string ModifyName { get; set; }
        public int? Vendor_Info_UID { get; set; }
        public DateTime? ModifyTime { get; set; }
        
    }

    public class FixturePart_OrderQueryVM : BaseModel
    {
        public int Fixture_Part_Order_M_UID { get; set; }
        public int Plant_Organization_UID { get; set; }
        public int BG_Organization_UID { get; set; }
        public int? FunPlant_Organization_UID { get; set; }
        public string Order_ID { get; set; }
        public string Part_ID { get; set; }
        [JsonConverter(typeof(SPPDateTimeConverterToDate))]
        public DateTime? Order_Date { get; set; }
        public bool Is_Complated { get; set; }

        //提交状态
        public bool Is_SubmitFlag { get; set; }
        public string Remarks { get; set; }
        public bool Del_Flag { get; set; }
        //public int? Created_UID { get; set; }
        public DateTime Created_Date { get; set; }
        public string PlantName { get; set; }
        public string OpType_Name { get; set; }
        public string Func_Name { get; set; }
        public string ModifyName { get; set; }
        public DateTime? ModifyTime { get; set; }
        public string CreatName { get; set; }
        public int? Vendor_Info_UID { get; set; }
    }

    public class FixturePart_OrderEdit : BaseModel
    {
        public int Fixture_Part_Order_M_UID { get; set; }
        public int Plant_Organization_UID { get; set; }
        public int BG_Organization_UID { get; set; }
        public int? FunPlant_Organization_UID { get; set; }
        public DateTime Created_Date { get; set; }
        public string PlantName { get; set; }
        public string OpType_Name { get; set; }
        public string Func_Name { get; set; }
        public string Order_ID { get; set; }
        public string Part_ID { get; set; }
        [JsonConverter(typeof(SPPDateTimeConverterToDate))]
        public DateTime? Order_Date { get; set; }
        public bool Is_Complated { get; set; }
        public bool Is_SubmitFlag { get; set; }
        public string Remarks { get; set; }
        public bool Del_Flag { get; set; }
        public int Created_UID { get; set; }

        public string ModifyName { get; set; }
        public DateTime? ModifyTime { get; set; }
        public string CreatName { get; set; }

        //绑定二级明细项
        public List<FixturePartOrderDList> FixturePartOrderDList { get; set; }
        //绑定三级明细项
        public List<FixturePartOrderScheduleList> FixturePartOrderScheduleList { get; set; }
    }

    public class FixturePartOrderDList
    {
        public int index { get; set; }
        public int Fixture_Part_Order_D_UID { get; set; }
        public int Fixture_Part_Order_M_UID { get; set; }
        public int Fixture_Part_UID { get; set; }
        public string Part_ID { get; set; }
        public string Part_Name { get; set; }
        public string Part_Spec { get; set; }
        public decimal Price { get; set; }
        public decimal Qty { get; set; }
        public decimal? Actual_Receive_Qty { get; set; }
        [JsonConverter(typeof(SPPDateTimeConverterToDate))]
        public DateTime Forcast_Complation_Date { get; set; }
        public bool Del_Flag { get; set; }
        public int Vendor_Info_UID { get; set; }
        public string Vendor_ID { get; set; }
        public string Vendor_Name { get; set; }
        public decimal? SumActualQty { get; set; }
        //绑定供应商下拉框
        public List<VendorInfoList> VendorInfoList { get; set; }

    }

    public class FixturePartOrderScheduleList
    {
        public int dIndex { get; set; }
        public int mIndex { get; set; }
        public int Fixture_Part_Order_Schedule_UID { get; set; }
        public int Fixture_Part_Order_D_UID { get; set; }
        [JsonConverter(typeof(SPPDateTimeConverterToDate))]
        public DateTime Receive_Date { get; set; }
        public decimal Forcast_Receive_Qty { get; set; }
        public decimal? Actual_Receive_Qty { get; set; }
        public bool Is_Complated { get; set; }
        public bool Del_Flag { get; set; }
        public int? Delivery_UID { get; set; }
        public string Delivery_Name { get; set; }
        public DateTime? Delivery_Date { get; set; }
        public int? DeliveryPeriod_UID { get; set; }
        public string DeliveryPeriod_Name { get; set; }
        public DateTime? DeliveryPeriod_Date { get; set; }
    }

    public class FixturePartSettingMVM
    {
        public int Fixture_Part_Setting_M_UID { get; set; }
        public int Plant_Organization_UID { get; set; }
        public int BG_Organization_UID { get; set; }
        public int? FunPlant_Organization_UID { get; set; }
        public string Fixture_NO { get; set; }
    }

    public class FixturePartPurchaseVM
    {
        public int Fixture_Part_Setting_D_UID { get; set; }
        public int Fixture_Part_Setting_M_UID { get; set; }
        public int Fixture_Part_UID { get; set; }
        public string Part_ID { get; set; }
        public string Part_Name { get; set; }
        public string Part_Spec { get; set; }
        public bool Is_Enable { get; set; }
    }

    public class VendorInfoList
    {
        public int Vendor_Info_UID { get; set; }
        public int Plant_Organization_UID { get; set; }
        public int? BG_Organization_UID { get; set; }
        public string Vendor_ID { get; set; }
        public string Vendor_Name { get; set; }
        public bool Is_Enable { get; set; }
    }

    public class SubmitFixturePartOrderVM:BaseModel
    {
        public int Fixture_Part_Order_M_UID { get; set; }
        public int Plant_Organization_UID { get; set; }
        public int BG_Organization_UID { get; set; }
        public int? FunPlant_Organization_UID { get; set; }
        public string Order_ID { get; set; }
        public DateTime Order_Date { get; set; }
        public bool Is_Complated { get; set; }
        public string Remarks { get; set; }
        public bool Del_Flag { get; set; }
        //订单的提交状态
        public bool Is_SubmitFlag { get; set; }
        public int Created_UID { get; set; }
        public DateTime Created_Date { get; set; }
        public string Is_Submit { get; set; } //是否是交货
        public List<FixturePartOrderDList> FixturePartOrderDList { get; set; }
        public List<FixturePartOrderScheduleList> FixturePartOrderScheduleList { get; set; }
        
    }


}

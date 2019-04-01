using Newtonsoft.Json;
using PDMS.Common.Helpers;
using System;

namespace PDMS.Model
{
    public class EditWIPDTO : BaseModel
    {

        public string Interval_Now { get; set; }

        public string Date_Now { get; set; }

        public string Interval_Last { get; set; }

        public string Date_Last { get; set; }

        public int FlowChart_Master_UID { get; set; }

    }

    public class EditProductDTO : BaseModel
    {
        public int Product_UID { get; set; }

        public int Picking_QTY { get; set; }

        public int Good_QTY { get; set; }

        public int Adjust_QTY { get; set; }

        public int NG_QTY { get; set; }

        public int WH_Picking_QTY { get; set; }

        public int WH_QTY { get; set; }

        public int WIP_QTY { get; set; }

        public int Is_Comfirm { get; set; }

        public int Is_Last { get; set; }

    }

    public class EditFlagDTO : BaseModel
    {
        public string Product_Date { get; set; }

        public string Interval_Time { get; set; }

        public int Master_UID { get; set; }

        public string Func_Plant { get; set; }

        public int Is_Comfirmed { get; set; }

        public int Is_Lasted { get; set; }
    }

    public class EditEnumDTO : BaseModel
    {
        public int Enum_UID { get; set; }
        public string Enum_Type { get; set; }

        public string Enum_Name { get; set; }

        public string Enum_Value { get; set; }

        public string Decription { get; set; }

    }


    public class SQLDTO : BaseModel
    {
        public string SQLText { get; set; }

    }

}

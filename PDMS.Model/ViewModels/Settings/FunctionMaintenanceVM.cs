using System;
using System.Collections.Generic;

namespace PDMS.Model
{
    public class FunctionItem : EntityDTOBase
    {
        public int Function_UID { get; set; }

        public string Parent_Function_ID { get; set; }

        public string Parent_Function_Name { get; set; }

        public string Function_ID { get; set; }

        public string Function_Name { get; set; }

        public int Order_Index { get; set; }

        public string URL { get; set; }

        public bool Is_Show { get; set; }

        public int SubFunctionCount { get; set; }

    }

    public class FunctionWithSubs : EntityDTOBase
    {
        public int Function_UID { get; set; }

        public string Parent_Function_ID { get; set; }

        public string Parent_Function_Name { get; set; }

        public string Function_ID { get; set; }

        public string Function_Name { get; set; }

        public string Function_Desc { get; set; }

        public string Icon_ClassName { get; set; }

        public int Order_Index { get; set; }

        public string URL { get; set; }

        public bool Is_Show { get; set; }

        public string Mobile_URL { get; set; }

        public List<SystemFunctionSubDTO> FunctionSubs { get; set; }
    }

    public class FunctionModelSearch : BaseModel
    {
        public string Parent_Function_ID { get; set; }

        public string Parent_Function_Name { get; set; }

        public string Function_ID { get; set; }

        public string Function_Name { get; set; }

        public string Modified_By { get; set; }

        public DateTime? Modified_Date_From { get; set; }

        public DateTime? Modified_Date_End { get; set; }
    }
}

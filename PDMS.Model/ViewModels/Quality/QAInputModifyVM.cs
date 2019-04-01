﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDMS.Model.ViewModels
{
    public class QAInputModifySearch : BaseModel
    {
        public string Project { get; set; }
        public string CheckPoint { get; set; }
        public string Color { get; set; }
        public string MaterielType { get; set; }
        public DateTime Product_Date { get; set; }
        public string Time_Interval { get; set; }
    }
    public class QAInputModifyVM : BaseModel
    {
        public int Log_UID { get; set; }
        public Nullable<int> QualityAssurance_InputDetail_UID { get; set; }
        public Nullable<int> QualityAssurance_InputMaster_UID { get; set; }
        public bool IsDeleted { get; set; }
        public Nullable<int> LastRepair_Qty { get; set; }
        public Nullable<int> LastSepcialAccept_Qty { get; set; }
        public Nullable<int> LastNG_Qty { get; set; }
        public Nullable<int> LastRepairNG_Qty { get; set; }

        public Nullable<int> LastDisplace_Qty { get; set; }
        public Nullable<int> NewDisplace_Qty { get; set; }

        public Nullable<int> NewRepair_Qty { get; set; }
        public Nullable<int> NewSepcialAccept_Qty { get; set; }
        public Nullable<int> NewNG_Qty { get; set; }
        public Nullable<int> NewRepairNG_Qty { get; set; }

        public string Modified_Date { get; set; }
        public string Modified_UserName { get; set; }

        public string ModifiedReason { get; set; }
        public string Project { get; set; }
        public string CheckPoint { get; set; }
        public string Color { get; set; }
        public string MaterielType { get; set; }
        public string Product_Date { get; set; }
        public string Time_Interval { get; set; }
        public string TypeName { get; set; }

        //需要四个表联合  log>InputDetail>ExceptionType>InputMaster
    }



}
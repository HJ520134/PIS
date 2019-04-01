using System;
using System.Collections.Generic;

namespace PDMS.Model
{
    public class CostCenterItem : EntityDTOBase
    {
        public int Plant_Organization_UID { get; set; }

        public int BG_Organization_UID { get; set; }

        public int? FunPlant_Organization_UID { get; set; }

        public string CostCtr_ID { get; set; }

        public string CostCtr_Description { get; set; }

        public int CostCtr_UID { get; set; }

        public string factory { get; set; }

        public string op_Type { get; set; }

        public string funPlant { get; set; }
    }

    public class CostCtr_infoDTO : EntityDTOBase
    {
        public int CostCtr_UID { get; set; }
        public int Plant_Organization_UID { get; set; }
        public int BG_Organization_UID { get; set; }
        public Nullable<int> FunPlant_Organization_UID { get; set; }
        public string CostCtr_ID { get; set; }
        public string CostCtr_Description { get; set; }
    }

    public class CostCtrDTO : EntityDTOBase
    {
        public int CostCtr_UID { get; set; }
        public string CostCtr { get; set; }
    }

    public class CostCenterModelSearch : BaseModel
    {
        public int Plant_Organization_UID { get; set; }

        public int BG_Organization_UID { get; set; }

        public int FunPlant_Organization_UID { get; set; }

        public string CostCtr_ID { get; set; }

        public string CostCtr_Description { get; set; }

        public string Modified_By { get; set; }

        public DateTime? Modified_Date_From { get; set; }

        public DateTime? Modified_Date_End { get; set; }
    }
}

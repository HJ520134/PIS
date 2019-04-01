using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDMS.Model.EntityDTO
{
    public class EQPMaterialDTO : EntityDTOBase
    {
        public int EQP_Material_UID { get; set; }
        public int EQP_Type_UID { get; set; }
        public int Material_Uid { get; set; }
        public decimal BOM_Qty { get; set; }
        public bool Is_Enable { get; set; }
        public string Desc { get; set; }
        public string Material_Id { get; set; }
        public string Material_Name { get; set; }
        public string Material_Types { get; set; }
        public string FunPlant_Organization_UID { get; set; }
        public int FunPlantUID { get; set; }
        public string Plant { get; set; }
        public string Org_CTU { get; set; }
        public string OPType { get; set; }
        public string Funplant { get; set; }
        public string EQP_Type { get; set; }
        public int BG_Organization_UID { get; set; }
        public int Plant_UID { get; set; }

    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDMS.Model.EntityDTO
{
    public class Machine_CustomerAndStationDTO : EntityDTOBase
    {
        public int Machine_Customer_UID { get; set; }
        public string MES_Customer_Name { get; set; }
        public string PIS_Customer_Name { get; set; }
        public Nullable<int> Created_UID { get; set; }
        public Nullable<System.DateTime> Created_Date { get; set; }
        public Nullable<int> Modified_UID { get; set; }
        public Nullable<System.DateTime> Modified_Date { get; set; }
        public Nullable<bool> Is_Enable { get; set; }
        public int Plant_Organization_UID { get; set; }
        public int BG_Organization_UID { get; set; }
        public Nullable<int> FunPlant_Organization_UID { get; set; }
        public string Plant_Organization { get; set; }
        public string BG_Organization { get; set; }
        public string FunPlant_Organization { get; set; }
        public string Createder { get; set; }
        public string Modifieder { get; set; }
        public string DataSourceType { get; set; }
        public List<Machine_StationDTO> Storages { get; set; }

        public string Customer { get; set; }
        public string Project { get; set; }
        public string Product_Phase { get; set; }
        public string Part_Types { get; set; }

    }
}

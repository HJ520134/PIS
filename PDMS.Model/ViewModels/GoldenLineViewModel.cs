using Newtonsoft.Json;
using PDMS.Common.Helpers;
using PDMS.Model.EntityDTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDMS.Model.ViewModels
{
    public class GoldenLineNormalQueryViewModel : BaseModel
    {
        public int Plant_Organization_UID { get; set; }
        public int? FunPlant_Organization_UID { get; set; }
        public int BG_Organization_UID { get; set; }
        public bool? IsEnabled { get; set; }
        public int LineID { get; set; }
    }
    public class GL_RestTimeDTO : EntityDTOBase
    {
        public int RestID { get; set; }
        public int ShiftTimeID { get; set; }
        public int Plant_Organization_UID { get; set; }
        public int? FunPlant_Organization_UID { get; set; }
        public int BG_Organization_UID { get; set; }
        public string Shift { get; set; }
        public string StartTime { get; set; }
        public string EndTime { get; set; }
        public  int SEQ { get; set; }
        public string Plant_Organization_Name { get; set; }
        public string BG_Organization_Name { get; set; }
        public DateTime Modified_Time { get; set; }
    }
    public class RestTimeQueryViewModel : BaseModel
    {
        public int ShiftID { get; set; }
    }

    public class GroupLineItem
    {
        public int LineGroup_UID { get; set; }
        public int Plant_Organization_UID { get; set; }
        public int BG_Organization_UID { get; set; }
        public int? FunPlant_Organization_UID { get; set; }
        public int? LineID { get; set; }
        public int? LineParent_ID { get; set; }
        public int? CustomerID { get; set; }
        public string Project_Name { get; set; }
        public string LineParent_Name { get; set; }
        public string LineName { get; set; }
        public string factory { get; set; }
        public string op_Type { get; set; }
        public string funPlant { get; set; }
        [JsonConverter(typeof(SPPDateTimeConverterToDateByMin))]
        public DateTime? Modified_Date { get; set; }
        public string Modified_UserName { get; set; }
        public string MESProject_Name { get; set; }
    }

    public class SubLineItem
    {
        public int Line_ID { get; set; }
        public string LineName { get; set; }
    }
}

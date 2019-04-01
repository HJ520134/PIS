using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDMS.Model
{
    public class ExceptionTypeVM : EntityDTOBase
    {
        public int ExceptionType_UID { get; set; }
        public string TypeName { get; set; }
        public string Org_TypeCode { get; set; }
        public string ShortName { get; set; }
        public bool EnableFlag { get; set; }
        public string TypeClassify { get; set; }
        public int Creator_UID { get; set; }
        public System.DateTime Create_Date { get; set; }
        public int Modified_UID { get; set; }
        public System.DateTime Modified_Date { get; set; }
        public string Project { get; set; }
        public string First_Type { set; get; }
        public string Second_Type { set; get; }
        public int TypeLevel { set; get; }
        public int Flowchart_Master_UID { set; get; }

        public string BadTypeCode { set; get; }
        public string BadTypeEnglishCode { set; get; }
    }
}

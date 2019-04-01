using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using PDMS.Common.Helpers;

namespace PDMS.Model
{
    public class BadTypeSearch : BaseModel
    { 
        public string TypeName { get; set; }
        public string Org_TypeCode { get; set; }
        public string Father_TypeCode { get; set; }
        public string ShortName { get; set; }
        public string Project { get; set; }

        public int FlowChart_Master_UID { set; get; }
    }

    public class ExceptionTypeList : BaseModel
    {
        public List<ExceptionTypeVM> ExceptionTypeLists { set; get; }

    }

    public class ExceptionTypeTempList:BaseModel
    {
        public List<ExceptionTypeTempVM> ImportList { set; get; }
    }


    public class ExceptionTypeTempVM:BaseModel
    {
        public int UID { get; set; }
        public Nullable<int> Creator_UID { get; set; }
        public string TypeName { get; set; }
        public string ShortName { get; set; }
        public DateTime Create_Date { get; set; }
        public string TypeClassify { get; set; }
        public int TypeLevel { get; set; }
        public string FatherNode { get; set; }
        public string Project { get; set; }
        public int Flowchart_Master_UID { set; get; }

        /// <summary>
        /// 不良类型编码
        /// </summary>
        public string BadTypeCode { get; set; }

        /// <summary>
        /// 不良类型英文名称
        /// </summary>
        public string BadTypeEnglishCode { get; set; }
    }

   

}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDMS.Model
{
    public class QAReportExceptionTypeRankDTO: EntityDTOBase
    {
        public int RankNum { set; get; }
        public int TotalCount { set; get; }
        public string TypeName { set; get; }
        public decimal RejectionRate { set; get; }
        public string ExceptionType { set; get; }
        public string BadTypeEnglishCode { set; get; }
    }
}

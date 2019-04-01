
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDMS.Model
{
    public class QueryModel<T> : BaseModel
    {
        //等于
        public T Equal { get; set; }
        //不等于
        public T NotEqual { get; set; }
        //大于
        public T GreaterThan { get; set; }
        //小于
        public T LessThan { get; set; }
        //大于或等于
        public T GreaterThanOrEqual { get; set; }
        //小于或等于
        public T LessThanOrEqual { get; set; }
        //包含
        public T Like { get; set; }
    }
}

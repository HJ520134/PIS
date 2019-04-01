using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDMS.Common.Helpers
{
    public class DataFilter
    {
        /// <summary>   
        /// 按照指定的逻辑过滤数据   
        /// </summary>   
        public static IEnumerable<T> Filter<T>(IEnumerable<T> ObjectList, Func<T, bool> FilterFunc)
        {
            List<T> ResultList = new List<T>();
            foreach (var item in ObjectList)
            {
                if (FilterFunc(item))
                    ResultList.Add(item);
            }
            return ResultList;
        }
    }
}

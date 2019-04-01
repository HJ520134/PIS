using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDMS.Model
{
    public class PageUnauthorizedElementEntity
    {
        public string PageURL { get; set; }
        public string PageElements { get; set; }
    }

    public class SPReturnMessage
    {
        public string Message
        {
            set; get;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDMS.Model.ViewModels
{
    public class API_CNCCollectionData
    {
        public int ID { get; set; }

        public string dSn { get; set; }

        public string AuthCode { get; set; }

        public string ScanType { get; set; }

        public string Data { get; set; }

        public DateTime? CreateTime { get; set; }


    }


}

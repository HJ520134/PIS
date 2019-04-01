using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDMS.Model.ViewModels
{
    public class API_RegisteredMachine
    {

        public int ID { get; set; }
        public string dSn { get; set; }
        public string AuthCode { get; set; }
        public string Name { get; set; }
        public string chamber { get; set; }
        public string batch { get; set; }
        public string Computer { get; set; }
        public string Customer { get; set; }
        public string Assembly { get; set; }
        public string Factory { get; set; }
        public string Building { get; set; }
        public string MA { get; set; }
        public string Route { get; set; }
        public string RouteStep { get; set; }
        public string RouteStepInstance { get; set; }
        public string Station { get; set; }
        public string Line { get; set; }
        public string Category { get; set; }
        public string Vendor { get; set; }
        public string CheckPV { get; set; }
        public string Model { get; set; }
        public string Version { get; set; }
        public DateTime? CreateTime { get; set; }
        public string Creator { get; set; }
        public DateTime? EditTime { get; set; }
        public string Editor { get; set; }
        public string MAC { get; set; }
        public string IP { get; set; }
        public string PorCT { get; set; }


    }
}

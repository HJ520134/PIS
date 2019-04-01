using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDMS.Model.ViewModels
{
    //映射FixtureManage.FX_SNFixture
    public class FX_SNFixtureModel
    {
        public int ID { get; set; }
        public string Customer { get; set; }
        public string Line { get; set; }
        public string Machine { get; set; }
        public string Station { get; set; }
        public string SN { get; set; }
        public string BG { get; set; }
        public string Fixture { get; set; }
        public DateTime LastUpdated { get; set; }
        public string UserID { get; set; }
        public string LinkStatus { get; set; }
        public string ReturnMsg { get; set; }
        public string CNReturnMsg { get; set; }
    }
}

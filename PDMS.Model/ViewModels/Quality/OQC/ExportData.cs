using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDMS.Model
{
    public class OQCExportModel
    {
        public OQC_InputMasterVM MasterData = new OQC_InputMasterVM();
        public List<OQC_InputDetailVM> DetailDatas = new List<OQC_InputDetailVM>();
    }

    public class ExportOQCDataForExcel
    {
        public List<OQCExportModel> OQCDatas = new List<OQCExportModel>();
    }


}

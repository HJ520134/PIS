using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PDMS.Common.Constants;
using PDMS.Model.ViewModels;
using PDMS.Model.ViewModels.Common;

namespace PDMS.Web.Business.Flowchart
{
    public abstract class AbsFlowchart
    {
        public abstract string AddOrUpdateExcel(HttpPostedFileBase uploadName, int FlowChart_Master_UID, string FlowChart_Version_Comment, bool isEdit, FlowChartImport importItem);

        public abstract ActionResult DoHistoryExcelExport(int id, int version);

    }
}
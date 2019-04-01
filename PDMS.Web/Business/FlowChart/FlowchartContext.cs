using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PDMS.Common.Constants;
using PDMS.Model.ViewModels;
using PDMS.Web.Business.FlowChart;

namespace PDMS.Web.Business.Flowchart
{
    public class FlowchartContext
    {
        public delegate string Call(HttpPostedFileBase uploadName_update, int FlowChart_Master_UID, string FlowChart_Version_Comment, bool isEdit, FlowChartImport importItem);
        private AbsFlowchart absFlowcharChart = null;
        public FlowchartContext(bool isEtransferUser)
        {
            absFlowcharChart = new FlowchartCTUImport();
            if (isEtransferUser)
            {
                absFlowcharChart = new FlowchartEtransferImport();
            }
            else
            {
                absFlowcharChart = new FlowchartCTUImport();
            }
        }

        public string CallImportAdd(Call A, HttpPostedFileBase uploadName, int FlowChart_Master_UID, string FlowChart_Version_Comment, bool isEdit, FlowChartImport importItem)
        {
            var error = A(uploadName, FlowChart_Master_UID, FlowChart_Version_Comment, isEdit, importItem);
            return error;
        }

        public string AddOrUpdateExcel(HttpPostedFileBase uploadName, int FlowChart_Master_UID, string FlowChart_Version_Comment, bool isEdit, FlowChartImport importItem)
        {
            return absFlowcharChart.AddOrUpdateExcel(uploadName, FlowChart_Master_UID, FlowChart_Version_Comment, isEdit, importItem);
        }

        public ActionResult DoHistoryExcelExport(int id, int version)
        {
            return absFlowcharChart.DoHistoryExcelExport(id, version);
        }


    }
}
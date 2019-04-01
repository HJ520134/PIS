using PDMS.Data;
using PDMS.Model;
using PDMS.Model.ViewModels;
using PDMS.Model.ViewModels.Common;
using PDMS.Model.ViewModels.ProductionPlanning;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDMS.Service
{
    public interface IProductionResourcePlanService
    {
        PagedListModel<DemissionRateAndWorkScheduleDTO> GetDSPlanList(DRAWS_QueryParam searchModel, Page page);
        PagedListModel<CurrentStaffDTO> QueryCurrentStaffInfo(CurrentStaffDTO dto, Page page);
        string ImportCurrentStaffInfo(List<CurrentStaffDTO> list);
        string CheckImportCurrentStaffExcel(List<CurrentStaffDTO> list);
        string SaveStaffInfo(CurrentStaffDTO dto);

        PagedListModel<DemissionRateAndWorkScheduleDTO> QueryTurnoverSchedulingInfo(DemissionRateAndWorkScheduleDTO searchModel, Page page);

        DemissionRateAndWorkScheduleDTO GetDemissionInfoByID(int demissionID);
        bool DeleteDemissionInfoByID(int demissionID);

        List<Enumeration> GetWorkScheduleList();


        string SaveDemissionInfo(DemissionRateAndWorkScheduleDTO dto);
        string CheckImportTurnoverExcel(List<DemissionRateAndWorkScheduleDTO> list);
        string ImportTurnoverExcel(List<DemissionRateAndWorkScheduleDTO> list);
        List<DemissionRateAndWorkScheduleDTO> ExportDemissionRateInfo(DemissionRateAndWorkScheduleDTO searchModel, Page page);
        List<DemissionRateAndWorkScheduleDTO> ExportDemissionRateInfoByID(string uids);

        #region 专案
        ProductionPlanningReportGetProject GetProjectList(CustomUserInfoVM vm);
        Dictionary<int, string> GetOpTypesByPlantName(string plantName);
        Dictionary<int, string> GetProjectByOpType(int OpTypeUID);
        Dictionary<int, string> GetPartTypesByProject(int ProjectUID);
        Dictionary<int, string> GetFunPlantByOpType(int OpTypeUID);
        #endregion

        #region ME
        /// <summary>
        /// 取得Quotations資料
        /// </summary>
        /// <param name="search">查詢條件集合</param>
        /// <param name="page">分頁參數</param>        
        /// <returns></returns>
        PagedListModel<RP_ME_VM> QueryMEs(RP_MESearch search, Page page);
        /// <summary>
        /// 匯入ME資料
        /// </summary>
        /// <param name="all_vm">資料集合</param>
        void ImportFlowchartME(RP_All_VM all_vm);
        /// <summary>
        /// 驗證ME Excel匯入資訊
        /// </summary>
        /// <param name="parasItem">匯入資訊</param>
        /// <returns></returns>
        string CheckMEIsExists(RP_ME_ExcelImportParas parasItem);
        /// <summary>
        /// 取得ME_D資料清單by ME主檔流水號
        /// </summary>
        /// <param name="rP_Flowchart_Master_UID">ME主檔流水號</param>
        /// <returns></returns>
        List<RP_ME_D> GetME_Ds(int rP_Flowchart_Master_UID);
        /// <summary>
        /// 取得ME主檔 Change History
        /// </summary>
        /// <param name="plant_Organization_UID">plant_Organization_UID</param>
        /// <param name="bG_Organization_UID">bG_Organization_UID</param>
        /// <param name="project_UID">project_UID</param>
        /// <returns></returns>
        List<RP_M> GetME_ChangeHistory(int plant_Organization_UID, int bG_Organization_UID, int project_UID);
        /// <summary>
        /// 取得設備明細檔 RP_Flowchart_Detail_ME_Equipment清單
        /// </summary>
        /// <param name="search">搜尋條件集合</param>
        /// <param name="page">分頁參數</param>
        /// <returns></returns>
        PagedListModel<RP_ME_D_Equipment> GetME_D_Equipments(ME_EquipmentSearchVM search, Page page);
        /// <summary>
        /// 取得設備明細檔 RP_Flowchart_Detail_ME_Equipment (單筆)
        /// </summary>
        /// <param name="rP_Flowchart_Detail_ME_Equipment_UID">設備明細檔流水號</param>
        /// <returns></returns>
        RP_ME_D_Equipment GetME_D_Equipment(int rP_Flowchart_Detail_ME_Equipment_UID);
        /// <summary>
        /// 保存設備明細檔 RP_Flowchart_Detail_ME_Equipment (單筆)
        /// </summary>
        /// <param name="rP_Flowchart_Detail_ME_Equipment_UID">設備明細檔流水號</param>
        /// <param name="wquipment_Manpower_Ratio">人力配比</param>
        /// <param name="wQP_Variable_Qty">設備變動數量</param>
        /// <param name="nPI_Current_Qty">NPI當前數量</param>
        /// <returns></returns>
        MessageStatus SaveME_D_Equipment(SaveME_EquipmentVM vm);
        #endregion
    }
}

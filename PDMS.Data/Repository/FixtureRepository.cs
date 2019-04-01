using PDMS.Data.Infrastructure;
using PDMS.Model;
using PDMS.Model.ViewModels;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using PDMS.Common.Helpers;
using PDMS.Model.EntityDTO;
using System.Data.Entity.SqlServer;
using System.Data.Entity;
using PDMS.Common.Constants;
using System.Text;
using System.IO;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using System.Drawing;
using PDMS.Model.ViewModels.Batch;
using System.Configuration;
using PDMS.Common.Common;

namespace PDMS.Data.Repository
{

    public interface IFixtureRepository : IRepository<Fixture_M>
    {
        List<SystemFunctionPlantDTO> GetFunPlantByOPType(int Optype, string Optypes = "");
        IQueryable<FixtureDTO> QueryFixture(FixtureDTO searchModel, Page page, out int totalcount);
        IQueryable<FixtureDTO> QueryFixtureStatusMoniter(FixtureDTO searchModel, Page page, out int totalcount);
        List<FixtureDTO> QueryFixtureStatusMoniterBySearch(FixtureDTO searchModel);
        List<FixtureDTO> QueryFixtureStatusMoniterBySelected(FixtureDTO searchModel);
        List<FX_SNFixtureModel> GetFixtureStatusMoniterInIDs(List<string> scanedTimeList);
        IList<FX_SNFixtureModel> GetFX_SNFixtureByMachineID(string Machine_ID);
        IList<FX_SNFixtureModel> GetFX_SNFixtureNotInCurerntMechine(string Machine_ID, string BarCode);
        FixtureDTO QueryFixtureByUid(int fixture_UID);
        List<FixtureDTO> FixtureList(FixtureDTO searchModel);
        string BatchEnableFixturematerial(FixtureDTO searchModel, List<FixtureDTO> ListFixtureDTO, int status);

        List<FixtureDTO> FixtureList(int Plant_Organization_UID, int BG_Organization_UID, int FunPlant_Organization_UID);
        List<FixtureStatuDTO> GetFixtureStatuDTO();
        List<Vendor_InfoDTO> GetVendor_InfoList(int Plant_Organization_UID, int BG_Organization_UID);
        List<Production_LineDTO> GetProductionLineDTOList(int Plant_Organization_UID, int BG_Organization_UID, int FunPlant_Organization_UID);
        string Updatefixture_MAPI(List<FixtureDTO> dtolist);

        List<WorkshopDTO> GetWorkshopList(int Plant_Organization_UID, int BG_Organization_UID, int FunPlant_Organization_UID);
        List<WorkStationDTO> GetWorkstationList(int Plant_Organization_UID, int BG_Organization_UID, int FunPlant_Organization_UID);
        List<Process_InfoDTO> GetProcess_InfoList(int Plant_Organization_UID, int BG_Organization_UID, int FunPlant_Organization_UID);
        List<Fixture_DefectCodeDTO> GetDefectCodeList(int Plant_Organization_UID, int BG_Organization_UID, int FunPlant_Organization_UID);
        List<Fixture_RepairSolutionDTO> GeRepairSolutionList(int Plant_Organization_UID, int BG_Organization_UID, int FunPlant_Organization_UID);
        List<SystemProjectDTO> GetProjectList(int Plant_Organization_UID, int BG_Organization_UID, int FunPlant_Organization_UID);
        Production_LineDTO GetProductionLineDTO(int Production_Line_UID);
        List<FixtureMachineDTO> GetFixtureMachineDTOList(int Plant_Organization_UID, int BG_Organization_UID, int FunPlant_Organization_UID, int Production_Line_UID);
        FixtureMachineDTO GetFixtureMachineByUid(int Fixture_Machine_UID);
        string DeleteByUid(int Fixture_M_UID);
        List<FixtureDTO> DoExportFixtureReprot(string Fixture_M_UIDs);
        PagedListModel<FixtureResumeSearchVM> FixtureResumeSearchVM(FixtureResumeSearchVM searchVM, Page page, out int totalcount);
        ViewResumeByUID QueryFixtureResumeByUID(int Fixture_Resume_UID, int Fixture_M_UID);

        Fixture_M GetFixtureByUid(FixtureDTO searchModel);
        Dictionary<string, string> GetMaintenanceStatus(string Maintenance_Type);
        int GetFixtureCount(FixtureDTO searchModel);
        List<FixtureSystemUserDTO> GetFixtureSystemUser(int Plant_Organization_UID, int BG_Organization_UID, int FunPlant_Organization_UID);
        List<NotMaintenanceSearchVM> QueryFixtureNotMaintained(NotMaintenanceSearchVM search, Page page, out int totalcount);
        List<FixtureDTO> GetFixture_MByPlant(int Plant_Organization_UID);
        int GetFixture_MCount(int Plant_Organization_UID, int BG_Organization_UID, string Fixture_Unique_ID);
        List<FixtureResumeSearchVM> ExportFixtureResumeByUID(string uids);
        List<FixtureResumeSearchVM> DoAllExportFixtureResumeReprot(FixtureResumeSearchVM searchVM);
        List<NotMaintenanceSearchVM> ExportFixtureNotMaintainedByUID(string uids, string hidDate);
        List<NotMaintenanceSearchVM> DoAllExportFixtureNotMaintainedReprot(NotMaintenanceSearchVM searchVM);
        List<ReportByRepair> QueryReportByRepair(ReportByRepair model, Page page, out int totalcount);
        List<ReportByRepair> ExportReportByRepair(ReportByRepair model);
        List<ReportByRepair> QueryReportByRepairPerson(ReportByRepair model, Page page, out int totalcount, bool isExport);
        List<ReportByRepair> ExportReportByRepairPersonValid(ReportByRepair model);
        List<ReportByRepair> QueryReportByPage(ReportByRepair model, Page page, out int totalcount, bool isExport);
        List<ReportByRepair> ExportReportByPageValid(ReportByRepair model);
        List<ReportByRepair> QueryFixtureReportByDetail(ReportByRepair model, Page page, out int totalcount, bool isExport);
        List<ReportByRepair> ExportReportByDetailValid(ReportByRepair model);
        List<ReportByRepair> QueryFixtureReportByAnalisis(ReportByRepair model, Page page, out int totalcount, bool isExport);
        List<ReportByRepair> ExportReportByAnalisisValid(ReportByRepair model);
        List<string> GetFixtureNoList(int Plant_Organization_UID, int BG_Organization_UID, int FunPlant_Organization_UID);
        List<ReportByRepair> QueryFixtureReportByStatus(ReportByRepair model, Page page, out int totalcount, bool isExport);
        List<ReportByRepair> ExportReportByStatusValid(ReportByRepair model);
        List<ReportByStatusAnalisis> QueryFixtureReportByStatusAnalisis(ReportByRepair model, Page page, out int totalcount, bool isExport);
        List<ReportByStatusAnalisis> ExportReportByStatusAnalisisValid(ReportByRepair model);
        List<Batch_ReportByStatus> QueryFixtureReportByFMT(Batch_ReportByStatus model, Page page, out int totalcount, bool isExport, DateTime StartDate, DateTime EndDate);
        List<Batch_ReportByStatus> QueryQueryFixtureReportByFMTDetail(int Process_Info_UID, DateTime startDate, DateTime endDate, int SheetCount);
        List<Batch_ReportByStatus> ExportReportByFMTValid(Batch_ReportByStatus model);
        void ExecFMTDashboard(string functionName, int Plant_Organization_UID, int System_Schedule_UID);
        FixtureDTO GetFixtureDTO(int plantID, int optypeID, string SN);
        Fixture_Part_Setting_MDTO GetFixture_Part_Setting_MDTO(int plantID, int optypeID, string Fixture_NO);
        List<Fixture_Part_Setting_DDTO> GetFixture_Part_Setting_DDTOs(int fixture_Part_Setting_M_UID);
        Fixture_M_UseScanHistoryDTO GetFixture_M_UseScanHistoryDTO(int Fixture_M_UID);
        List<Fixture_Part_UseTimesDTO> GetFixture_Part_UseTimesDTO(int Fixture_M_UID);
        string SaveFixturePartScanCodeDTO(string strsql);
        List<FixturePartScanDTO> GetFixturePartScanDTOs(int Fixture_M_UID);
    }
    public class FixtureRepository : RepositoryBase<Fixture_M>, IFixtureRepository
    {
        //MES的LocalDB 数据库
        private string connStr = "Server=CNWXIG0LSQLV01B\\INST1;DataBase=FixtrueManage;uid=PISuser;pwd=PISuser123";
        public FixtureRepository(IDatabaseFactory databaseFactory)
            : base(databaseFactory)
        {

        }
        /// <summary>
        /// 获取OP
        /// </summary>
        /// <param name="Optype"></param>
        /// <param name="Optypes"></param>
        /// <returns></returns>
        public List<SystemFunctionPlantDTO> GetFunPlantByOPType(int Optype, string Optypes = "")
        {
            string sql = @"SELECT Organization_Name AS FunPlant ,  Organization_UID AS FunPlant_OrganizationUID FROM  System_Organization WHERE Organization_UID IN (
                          SELECT ChildOrg_UID FROM System_OrganizationBOM WHERE ParentOrg_UID IN(SELECT
                          ChildOrg_UID
                          FROM System_OrganizationBOM A, System_Organization B WHERE
                          A.ParentOrg_UID ={0} AND A.ChildOrg_UID = B.Organization_UID AND B.Organization_Name = 'OP')
                          )";
            sql = string.Format(sql, Optype);
            var dblist = DataContext.Database.SqlQuery<SystemFunctionPlantDTO>(sql).ToList();
            return dblist;
        }
        /// <summary>
        /// 获取列表的治具资料
        /// </summary>
        /// <param name="searchModel"></param>
        /// <param name="page"></param>
        /// <param name="totalcount"></param>
        /// <returns></returns>
        public IQueryable<FixtureDTO> QueryFixture(FixtureDTO searchModel, Page page, out int totalcount)
        {
            var query = from fixture in DataContext.Fixture_M
                        select new FixtureDTO
                        {
                            Fixture_M_UID = fixture.Fixture_M_UID,
                            Plant_Organization_UID = fixture.Plant_Organization_UID,
                            BG_Organization_UID = fixture.BG_Organization_UID,
                            FunPlant_Organization_UID = fixture.FunPlant_Organization_UID,
                            Fixture_NO = fixture.Fixture_NO,
                            Version = fixture.Version,
                            Fixture_Seq = fixture.Fixture_Seq,
                            Fixture_Unique_ID = fixture.Fixture_Unique_ID,
                            Fixture_Name = fixture.Fixture_Name,
                            Project_UID = fixture.Project_UID,
                            Fixture_Machine_UID = fixture.Fixture_Machine_UID,
                            Vendor_Info_UID = fixture.Vendor_Info_UID,
                            Production_Line_UID = fixture.Production_Line_UID,
                            Status = fixture.Status,
                            ShortCode = fixture.ShortCode,
                            TwoD_Barcode = fixture.TwoD_Barcode,
                            Created_Date = fixture.Created_Date,
                            Created_UID = fixture.Created_UID,
                            Modified_Date = fixture.Modified_Date,
                            Modified_UID = fixture.Modified_UID,
                            Equipment_No = fixture.Fixture_Machine.Equipment_No,
                            Machine_Type = fixture.Fixture_Machine.Machine_Type,
                            Machine_ID = fixture.Fixture_Machine.Machine_ID,
                            Line_Name = fixture.Production_Line.Line_Name,
                            Workshop = fixture.Production_Line.Workshop.Workshop_Name,
                            Workshop_UID = fixture.Production_Line.Workshop_UID,
                            Workstation = fixture.Production_Line.WorkStation.WorkStation_Name,
                            Workstation_UID = fixture.Production_Line.Workstation_UID,
                            Process_Info = fixture.Production_Line.Process_Info.Process_Name,
                            Process_Info_UID = fixture.Production_Line.Process_Info_UID,
                            Project = fixture.System_Project.Project_Name,
                            Createder = fixture.System_Users.User_Name,
                            Modifieder = fixture.System_Users1.User_Name
                        };
            if (searchModel.Plant_Organization_UID != 0)
                query = query.Where(m => m.Plant_Organization_UID == searchModel.Plant_Organization_UID);
            if (searchModel.BG_Organization_UID != 0)
                query = query.Where(m => m.BG_Organization_UID == searchModel.BG_Organization_UID);
            if (searchModel.FunPlant_Organization_UID != null && searchModel.FunPlant_Organization_UID != 0)
                query = query.Where(m => m.FunPlant_Organization_UID == searchModel.FunPlant_Organization_UID);
            if (!string.IsNullOrWhiteSpace(searchModel.Fixture_NO))
                query = query.Where(m => m.Fixture_NO.Contains(searchModel.Fixture_NO));
            if (!string.IsNullOrWhiteSpace(searchModel.Version))
                query = query.Where(m => m.Version.Contains(searchModel.Version));
            if (!string.IsNullOrWhiteSpace(searchModel.Fixture_Seq))
                query = query.Where(m => m.Fixture_Seq.Contains(searchModel.Fixture_Seq));
            if (!string.IsNullOrWhiteSpace(searchModel.Fixture_Unique_ID))
                query = query.Where(m => m.Fixture_Unique_ID == searchModel.Fixture_Unique_ID);
            if (!string.IsNullOrWhiteSpace(searchModel.Fixture_Name))
                query = query.Where(m => m.Fixture_Name.Contains(searchModel.Fixture_Name));
            if (searchModel.Project_UID != null && searchModel.Project_UID != 0)
                query = query.Where(m => m.Project_UID == searchModel.Project_UID);
            if (searchModel.Fixture_Machine_UID != null && searchModel.Fixture_Machine_UID != 0)
                query = query.Where(m => m.Fixture_Machine_UID == searchModel.Fixture_Machine_UID);
            if (searchModel.Vendor_Info_UID != null && searchModel.Vendor_Info_UID != 0)
                query = query.Where(m => m.Vendor_Info_UID == searchModel.Vendor_Info_UID);
            if (searchModel.Production_Line_UID != null && searchModel.Production_Line_UID != 0)
                query = query.Where(m => m.Production_Line_UID == searchModel.Production_Line_UID);
            if (searchModel.Process_Info_UID != null && searchModel.Process_Info_UID != 0)
                query = query.Where(m => m.Process_Info_UID == searchModel.Process_Info_UID);
            if (searchModel.Workshop_UID != null && searchModel.Workshop_UID != 0)
                query = query.Where(m => m.Workshop_UID == searchModel.Workshop_UID);
            if (searchModel.Workstation_UID != null && searchModel.Workstation_UID != 0)
                query = query.Where(m => m.Workstation_UID == searchModel.Workstation_UID);
            if (searchModel.Status != 0)
                query = query.Where(m => m.Status == searchModel.Status);
            if (!string.IsNullOrWhiteSpace(searchModel.ShortCode))
                query = query.Where(m => m.ShortCode.Contains(searchModel.ShortCode));
            if (!string.IsNullOrWhiteSpace(searchModel.TwoD_Barcode))
                query = query.Where(m => m.TwoD_Barcode.Contains(searchModel.TwoD_Barcode));
            if (searchModel.Created_Date != null)
            {
                DateTime nextTime = searchModel.Created_Date.Value.Date.AddDays(1);
                query = query.Where(m => m.Created_Date >= searchModel.Created_Date && m.Created_Date < nextTime);
            }

            if (!string.IsNullOrWhiteSpace(searchModel.Modifieder))
                query = query.Where(m => m.Modifieder.Contains(searchModel.Modifieder));
            if (searchModel.Modified_UID != 0)
                query = query.Where(m => m.Modified_UID == searchModel.Modified_UID);
            if (searchModel.End_Date_From != null)
                query = query.Where(m => m.Modified_Date >= searchModel.End_Date_From);
            if (searchModel.End_Date_To != null)
            {
                DateTime nextTime = searchModel.End_Date_To.Value.Date.AddDays(1);
                query = query.Where(m => m.Modified_Date < nextTime);
            }
       
            totalcount = query.Count();
            query = query.OrderByDescending(m => m.Modified_Date).ThenBy(m => m.Fixture_M_UID).GetPage(page);
            query = SetFixtureDTO(query.ToList());
            return query;
        }

        public IQueryable<FixtureDTO> QueryFixtureStatusMoniter(FixtureDTO searchModel, Page page, out int totalcount)
        {
            var query = from fixture in DataContext.Fixture_M select fixture;
            if (searchModel.Plant_Organization_UID != 0)
                query = query.Where(m => m.Plant_Organization_UID == searchModel.Plant_Organization_UID);
            if (searchModel.BG_Organization_UID != 0)
                query = query.Where(m => m.BG_Organization_UID == searchModel.BG_Organization_UID);
            if (searchModel.FunPlant_Organization_UID != null && searchModel.FunPlant_Organization_UID != 0)
                query = query.Where(m => m.FunPlant_Organization_UID == searchModel.FunPlant_Organization_UID);
            if (searchModel.Project_UID != null && searchModel.Project_UID != 0)
                query = query.Where(m => m.Project_UID == searchModel.Project_UID);
            if (searchModel.Fixture_Machine_UID != null && searchModel.Fixture_Machine_UID != 0)
                query = query.Where(m => m.Fixture_Machine_UID == searchModel.Fixture_Machine_UID);
            if (searchModel.Production_Line_UID != null && searchModel.Production_Line_UID != 0)
                query = query.Where(m => m.Production_Line_UID == searchModel.Production_Line_UID);

            //获取机台
            var machine = DataContext.Fixture_Machine.FirstOrDefault(m => m.Fixture_Machine_UID == searchModel.Fixture_Machine_UID);

            //根据机台从LocalDB 获取当前机台扫过的数据(白班8:00~20:00,夜班20:00~8:00)
            var SNFixtureList = GetFX_SNFixtureByMachineID(machine.Machine_ID);
            totalcount = SNFixtureList.Count();
            var pagedSNFixtureQuery = (SNFixtureList.AsQueryable()).OrderByDescending(m => m.LastUpdated).GetPage(page);
            var pagedSNFixtureList = pagedSNFixtureQuery.ToList();
            var dtoList = new List<FixtureDTO>();
            foreach (var item in pagedSNFixtureList)
            {
                var dto = new FixtureDTO() { FX_SNFixtureID = item.ID, TwoD_Barcode = item.Fixture, IsPass = false, ScanedTime = item.LastUpdated };
                if (string.IsNullOrWhiteSpace(item.Fixture))
                {
                    dto.MoniterStatusMark = "未扫到治具二维码";
                    dto.TwoD_Barcode = "NG";
                }
                else
                {
                    var fixture = query.FirstOrDefault(f => f.TwoD_Barcode == item.Fixture);
                    if (fixture != null)
                    {
                        dto.MoniterStatusMark = fixture.Enumeration.Enum_Value;
                        if (fixture.Enumeration.Enum_Value.Contains("使用中"))
                        {
                            dto.IsPass = true;
                        }
                        dto.PlantName = fixture.System_Organization.Organization_Name;
                        dto.OPName = fixture.System_Organization1.Organization_Name;
                        dto.FunPlantName = fixture.System_Organization2 == null ? "" : fixture.System_Organization2.Organization_Name;
                        dto.Fixture_M_UID = fixture.Fixture_M_UID;
                        dto.Fixture_NO = fixture.Fixture_NO;
                        dto.Fixture_Unique_ID = fixture.Fixture_Unique_ID;
                        dto.Fixture_Name = fixture.Fixture_Name;
                        dto.Project_UID = fixture.Project_UID;
                        dto.Fixture_Machine_UID = fixture.Fixture_Machine_UID;
                        dto.ShortCode = fixture.ShortCode;
                        dto.Machine_Name = fixture.Fixture_Machine.Machine_Name;
                        dto.Line_Name = fixture.Production_Line.Line_Name;
                        dto.Workshop = fixture.Production_Line.Workshop.Workshop_Name;
                        dto.Workstation = fixture.Production_Line.WorkStation.WorkStation_Name;
                        dto.Process_Info = fixture.Production_Line.Process_Info.Process_Name;
                        dto.Project = fixture.System_Project.Project_Name;
                    }
                    else
                    {
                        dto.MoniterStatusMark = "非此机台治具";
                        fixture = DataContext.Fixture_M.FirstOrDefault(f => f.TwoD_Barcode == item.Fixture);
                        if (fixture != null)
                        {
                            dto.PlantName = fixture.System_Organization.Organization_Name;
                            dto.OPName = fixture.System_Organization1.Organization_Name;
                            dto.FunPlantName = fixture.System_Organization2 == null ? "" : fixture.System_Organization2.Organization_Name;
                            dto.Fixture_M_UID = fixture.Fixture_M_UID;
                            dto.Fixture_NO = fixture.Fixture_NO;
                            dto.Fixture_Unique_ID = fixture.Fixture_Unique_ID;
                            dto.Fixture_Name = fixture.Fixture_Name;
                            dto.Project_UID = fixture.Project_UID;
                            dto.Fixture_Machine_UID = fixture.Fixture_Machine_UID;
                            dto.ShortCode = fixture.ShortCode;
                            dto.Machine_Name = fixture.Fixture_Machine.Machine_Name;
                            dto.Line_Name = fixture.Production_Line.Line_Name;
                            dto.Workshop = fixture.Production_Line.Workshop.Workshop_Name;
                            dto.Workstation = fixture.Production_Line.WorkStation.WorkStation_Name;
                            dto.Process_Info = fixture.Production_Line.Process_Info.Process_Name;
                            dto.Project = fixture.System_Project.Project_Name;
                        }
                    }
                }
                dtoList.Add(dto);
            }
            return dtoList.AsQueryable();
        }

        public List<FixtureDTO> QueryFixtureStatusMoniterBySearch(FixtureDTO searchModel)
        {
            var query = from fixture in DataContext.Fixture_M select fixture;
            if (searchModel.Plant_Organization_UID != 0)
                query = query.Where(m => m.Plant_Organization_UID == searchModel.Plant_Organization_UID);
            if (searchModel.BG_Organization_UID != 0)
                query = query.Where(m => m.BG_Organization_UID == searchModel.BG_Organization_UID);
            if (searchModel.FunPlant_Organization_UID != null && searchModel.FunPlant_Organization_UID != 0)
                query = query.Where(m => m.FunPlant_Organization_UID == searchModel.FunPlant_Organization_UID);
            if (searchModel.Project_UID != null && searchModel.Project_UID != 0)
                query = query.Where(m => m.Project_UID == searchModel.Project_UID);
            if (searchModel.Fixture_Machine_UID != null && searchModel.Fixture_Machine_UID != 0)
                query = query.Where(m => m.Fixture_Machine_UID == searchModel.Fixture_Machine_UID);
            if (searchModel.Production_Line_UID != null && searchModel.Production_Line_UID != 0)
                query = query.Where(m => m.Production_Line_UID == searchModel.Production_Line_UID);

            //获取机台
            var machine = DataContext.Fixture_Machine.FirstOrDefault(m => m.Fixture_Machine_UID == searchModel.Fixture_Machine_UID);

            //根据机台从LocalDB 获取当前机台扫过的数据(白班8:00~20:00,夜班20:00~8:00)
            var SNFixtureList = GetFX_SNFixtureByMachineID(machine.Machine_ID).OrderByDescending(i => i.LastUpdated);
            var dtoList = new List<FixtureDTO>();
            foreach (var item in SNFixtureList)
            {
                var dto = new FixtureDTO() { TwoD_Barcode = item.Fixture, IsPass = false, ScanedTime = item.LastUpdated };
                if (string.IsNullOrWhiteSpace(item.Fixture))
                {
                    dto.MoniterStatusMark = "未扫到治具二维码";
                    dto.TwoD_Barcode = "NG";
                }
                else
                {
                    var fixture = query.FirstOrDefault(f => f.TwoD_Barcode == item.Fixture);
                    if (fixture != null)
                    {
                        dto.MoniterStatusMark = fixture.Enumeration.Enum_Value;
                        if (fixture.Enumeration.Enum_Value.Contains("使用中"))
                        {
                            dto.IsPass = true;
                        }
                        dto.PlantName = fixture.System_Organization.Organization_Name;
                        dto.OPName = fixture.System_Organization1.Organization_Name;
                        dto.FunPlantName = fixture.System_Organization2 == null ? "" : fixture.System_Organization2.Organization_Name;
                        dto.Fixture_M_UID = fixture.Fixture_M_UID;
                        dto.Fixture_NO = fixture.Fixture_NO;
                        dto.Fixture_Unique_ID = fixture.Fixture_Unique_ID;
                        dto.Fixture_Name = fixture.Fixture_Name;
                        dto.Project_UID = fixture.Project_UID;
                        dto.Fixture_Machine_UID = fixture.Fixture_Machine_UID;
                        dto.ShortCode = fixture.ShortCode;
                        dto.Machine_Name = fixture.Fixture_Machine.Machine_Name;
                        dto.Line_Name = fixture.Production_Line.Line_Name;
                        dto.Workshop = fixture.Production_Line.Workshop.Workshop_Name;
                        dto.Workstation = fixture.Production_Line.WorkStation.WorkStation_Name;
                        dto.Process_Info = fixture.Production_Line.Process_Info.Process_Name;
                        dto.Project = fixture.System_Project.Project_Name;
                    }
                    else
                    {
                        dto.MoniterStatusMark = "非此机台治具";
                        fixture = DataContext.Fixture_M.FirstOrDefault(f => f.TwoD_Barcode == item.Fixture);
                        if (fixture != null)
                        {
                            dto.PlantName = fixture.System_Organization.Organization_Name;
                            dto.OPName = fixture.System_Organization1.Organization_Name;
                            dto.FunPlantName = fixture.System_Organization2 == null ? "" : fixture.System_Organization2.Organization_Name;
                            dto.Fixture_M_UID = fixture.Fixture_M_UID;
                            dto.Fixture_NO = fixture.Fixture_NO;
                            dto.Fixture_Unique_ID = fixture.Fixture_Unique_ID;
                            dto.Fixture_Name = fixture.Fixture_Name;
                            dto.Project_UID = fixture.Project_UID;
                            dto.Fixture_Machine_UID = fixture.Fixture_Machine_UID;
                            dto.ShortCode = fixture.ShortCode;
                            dto.Machine_Name = fixture.Fixture_Machine.Machine_Name;
                            dto.Line_Name = fixture.Production_Line.Line_Name;
                            dto.Workshop = fixture.Production_Line.Workshop.Workshop_Name;
                            dto.Workstation = fixture.Production_Line.WorkStation.WorkStation_Name;
                            dto.Process_Info = fixture.Production_Line.Process_Info.Process_Name;
                            dto.Project = fixture.System_Project.Project_Name;
                        }
                    }
                }
                dtoList.Add(dto);
            }
            return dtoList;
        }

        public List<FixtureDTO> QueryFixtureStatusMoniterBySelected(FixtureDTO searchModel)
        {
            var query = from fixture in DataContext.Fixture_M select fixture;
            if (searchModel.Plant_Organization_UID != 0)
                query = query.Where(m => m.Plant_Organization_UID == searchModel.Plant_Organization_UID);
            if (searchModel.BG_Organization_UID != 0)
                query = query.Where(m => m.BG_Organization_UID == searchModel.BG_Organization_UID);
            if (searchModel.FunPlant_Organization_UID != null && searchModel.FunPlant_Organization_UID != 0)
                query = query.Where(m => m.FunPlant_Organization_UID == searchModel.FunPlant_Organization_UID);
            if (searchModel.Project_UID != null && searchModel.Project_UID != 0)
                query = query.Where(m => m.Project_UID == searchModel.Project_UID);
            if (searchModel.Fixture_Machine_UID != null && searchModel.Fixture_Machine_UID != 0)
                query = query.Where(m => m.Fixture_Machine_UID == searchModel.Fixture_Machine_UID);
            if (searchModel.Production_Line_UID != null && searchModel.Production_Line_UID != 0)
                query = query.Where(m => m.Production_Line_UID == searchModel.Production_Line_UID);

            //获取机台
            var IDList = searchModel.Fixture_Name.Split(',').ToList();
            //根据机台从LocalDB 获取当前机台扫过的数据(白班8:00~20:00,夜班20:00~8:00)
            var SNFixtureList = GetFixtureStatusMoniterInIDs(IDList).OrderByDescending(i => i.LastUpdated);

            var dtoList = new List<FixtureDTO>();
            foreach (var item in SNFixtureList)
            {
                var dto = new FixtureDTO() { TwoD_Barcode = item.Fixture, IsPass = false, ScanedTime = item.LastUpdated };
                if (string.IsNullOrWhiteSpace(item.Fixture))
                {
                    dto.MoniterStatusMark = "未扫到治具二维码";
                    dto.TwoD_Barcode = "NG";
                }
                else
                {
                    var fixture = query.FirstOrDefault(f => f.TwoD_Barcode == item.Fixture);
                    if (fixture != null)
                    {
                        dto.MoniterStatusMark = fixture.Enumeration.Enum_Value;
                        if (fixture.Enumeration.Enum_Value.Contains("使用中"))
                        {
                            dto.IsPass = true;
                        }
                        dto.PlantName = fixture.System_Organization.Organization_Name;
                        dto.OPName = fixture.System_Organization1.Organization_Name;
                        dto.FunPlantName = fixture.System_Organization2 == null ? "" : fixture.System_Organization2.Organization_Name;
                        dto.Fixture_M_UID = fixture.Fixture_M_UID;
                        dto.Fixture_NO = fixture.Fixture_NO;
                        dto.Fixture_Unique_ID = fixture.Fixture_Unique_ID;
                        dto.Fixture_Name = fixture.Fixture_Name;
                        dto.Project_UID = fixture.Project_UID;
                        dto.Fixture_Machine_UID = fixture.Fixture_Machine_UID;
                        dto.ShortCode = fixture.ShortCode;
                        dto.Machine_Name = fixture.Fixture_Machine.Machine_Name;
                        dto.Line_Name = fixture.Production_Line.Line_Name;
                        dto.Workshop = fixture.Production_Line.Workshop.Workshop_Name;
                        dto.Workstation = fixture.Production_Line.WorkStation.WorkStation_Name;
                        dto.Process_Info = fixture.Production_Line.Process_Info.Process_Name;
                        dto.Project = fixture.System_Project.Project_Name;
                    }
                    else
                    {
                        dto.MoniterStatusMark = "非此机台治具";
                        fixture = DataContext.Fixture_M.FirstOrDefault(f => f.TwoD_Barcode == item.Fixture);
                        if (fixture != null)
                        {
                            dto.PlantName = fixture.System_Organization.Organization_Name;
                            dto.OPName = fixture.System_Organization1.Organization_Name;
                            dto.FunPlantName = fixture.System_Organization2 == null ? "" : fixture.System_Organization2.Organization_Name;
                            dto.Fixture_M_UID = fixture.Fixture_M_UID;
                            dto.Fixture_NO = fixture.Fixture_NO;
                            dto.Fixture_Unique_ID = fixture.Fixture_Unique_ID;
                            dto.Fixture_Name = fixture.Fixture_Name;
                            dto.Project_UID = fixture.Project_UID;
                            dto.Fixture_Machine_UID = fixture.Fixture_Machine_UID;
                            dto.ShortCode = fixture.ShortCode;
                            dto.Machine_Name = fixture.Fixture_Machine.Machine_Name;
                            dto.Line_Name = fixture.Production_Line.Line_Name;
                            dto.Workshop = fixture.Production_Line.Workshop.Workshop_Name;
                            dto.Workstation = fixture.Production_Line.WorkStation.WorkStation_Name;
                            dto.Process_Info = fixture.Production_Line.Process_Info.Process_Name;
                            dto.Project = fixture.System_Project.Project_Name;
                        }
                    }
                }
                dtoList.Add(dto);
            }
            return dtoList;
        }
        public List<FX_SNFixtureModel> GetFixtureStatusMoniterInIDs(List<string> scanedTimeList)
        {
            var modelList = new List<FX_SNFixtureModel>();
            if (scanedTimeList.Count() > 0)
            {
                var sqlStr = new StringBuilder();
                sqlStr.Append(@"select * from FX_SNFixture where ID in (");
                for (int i = 0; i < scanedTimeList.Count(); i++)
                {
                    if (i > 0)
                    {
                        sqlStr.Append(",");
                    }
                    sqlStr.Append(string.Format("'{0}'", scanedTimeList[i]));
                }
                sqlStr.Append(")");
                using (SqlConnection conn = new SqlConnection(connStr))
                {
                    using (SqlCommand cmd = new SqlCommand(sqlStr.ToString(), conn))
                    {
                        try
                        {
                            conn.Open();
                            using (var read = cmd.ExecuteReader())
                            {
                                while (read.Read())//读取每行数据
                                {
                                    var model = new FX_SNFixtureModel();
                                    //获取字段信息
                                    model.ID = Convert.ToInt32(read["ID"]);
                                    model.Customer = (read["Customer"]).ToString();
                                    model.Line = (read["Line"]).ToString();
                                    model.Machine = (read["Machine"]).ToString();
                                    model.Station = (read["Station"]).ToString();
                                    model.SN = (read["SN"]).ToString();
                                    model.BG = (read["BG"]).ToString();
                                    model.Fixture = (read["Fixture"]).ToString();
                                    model.LastUpdated = Convert.ToDateTime(read["LastUpdated"]);
                                    model.UserID = (read["UserID"]).ToString();
                                    model.LinkStatus = (read["LinkStatus"]).ToString();
                                    model.ReturnMsg = (read["ReturnMsg"]).ToString();
                                    model.CNReturnMsg = (read["CNReturnMsg"]).ToString();

                                    modelList.Add(model);
                                }
                            }
                        }
                        catch (Exception)
                        {
                            throw;
                        }
                        finally
                        {
                            cmd.Dispose();
                            conn.Close();
                        }
                    }
                }
            }
            return modelList;
        }

        /// <summary>
        /// 根据机台代码、治具二维码获取FX_SNFixture 集合
        /// </summary>
        /// <param name="Machine_ID"></param>
        /// <returns></returns>
        public IList<FX_SNFixtureModel> GetFX_SNFixtureByMachineID(string Machine_ID)
        {
            var modelList = new List<FX_SNFixtureModel>();
            if (!string.IsNullOrWhiteSpace(Machine_ID))
            {
                //根据机台编码获取白班8:00~20:00或晚班20:00~8:00的数据，还有未扫到二维码的数据（即Fixture = ''）
                var sqlStr = string.Format(@"select * from FX_SNFixture A  where 
	            LastUpdated >= case when getdate() < dateadd(hour,8,CONVERT(varchar(100), GETDATE(), 23)) then dateadd(hour,-4,CONVERT(varchar(100), GETDATE(), 23)) else dateadd(hour,8,CONVERT(varchar(100), GETDATE(), 23)) end
 	            and LastUpdated = (select max(LastUpdated) from FX_SNFixture B where A.Fixture=B.Fixture)
	            and Machine = '{0}'
                union
                select * from FX_SNFixture A  where 
	            LastUpdated >= case when getdate() < dateadd(hour,8,CONVERT(varchar(100), GETDATE(), 23)) then dateadd(hour,-4,CONVERT(varchar(100), GETDATE(), 23)) else dateadd(hour,8,CONVERT(varchar(100), GETDATE(), 23)) end
 	            and (Fixture = '' or Fixture is null)
	            and Machine = '{0}'", Machine_ID);
                using (SqlConnection conn = new SqlConnection(connStr))
                {
                    using (SqlCommand cmd = new SqlCommand(sqlStr.ToString(), conn))
                    {
                        try
                        {
                            conn.Open();
                            using (var read = cmd.ExecuteReader())
                            {
                                while (read.Read())//读取每行数据
                                {
                                    var model = new FX_SNFixtureModel();
                                    //获取字段信息
                                    model.ID = Convert.ToInt32(read["ID"]);
                                    model.Customer = (read["Customer"]).ToString();
                                    model.Line = (read["Line"]).ToString();
                                    model.Machine = (read["Machine"]).ToString();
                                    model.Station = (read["Station"]).ToString();
                                    model.SN = (read["SN"]).ToString();
                                    model.BG = (read["BG"]).ToString();
                                    model.Fixture = (read["Fixture"]).ToString();
                                    model.LastUpdated = Convert.ToDateTime(read["LastUpdated"]);
                                    model.UserID = (read["UserID"]).ToString();
                                    model.LinkStatus = (read["LinkStatus"]).ToString();
                                    model.ReturnMsg = (read["ReturnMsg"]).ToString();
                                    model.CNReturnMsg = (read["CNReturnMsg"]).ToString();

                                    modelList.Add(model);
                                }
                            }
                        }
                        catch (Exception)
                        {
                            throw;
                        }
                        finally
                        {
                            cmd.Dispose();
                            conn.Close();
                        }
                    }
                }
            }
            return modelList;
        }
        public IList<FX_SNFixtureModel> GetFX_SNFixtureNotInCurerntMechine(string Machine_ID, string BarCode)
        {
            var modelList = new List<FX_SNFixtureModel>();
            if (!string.IsNullOrWhiteSpace(Machine_ID) && !string.IsNullOrWhiteSpace(BarCode))
            {
                var sqlStr = string.Format(@"select * from FX_SNFixture A  where 
	            LastUpdated >= case when getdate() < dateadd(hour,8,CONVERT(varchar(100), GETDATE(), 23)) then dateadd(hour,-4,CONVERT(varchar(100), GETDATE(), 23)) else dateadd(hour,8,CONVERT(varchar(100), GETDATE(), 23)) end
 	            and LastUpdated = (select max(LastUpdated) from FX_SNFixture B where A.Fixture=B.Fixture) 
	            and Machine != '{0}' and Fixture = '{1}'", Machine_ID, BarCode);    //治具在别个机台使用
                using (SqlConnection conn = new SqlConnection(connStr))
                {
                    using (SqlCommand cmd = new SqlCommand(sqlStr.ToString(), conn))
                    {
                        try
                        {
                            conn.Open();
                            using (var read = cmd.ExecuteReader())
                            {
                                while (read.Read())//读取每行数据
                                {
                                    var model = new FX_SNFixtureModel();
                                    //获取字段信息
                                    model.ID = Convert.ToInt32(read["ID"]);
                                    model.Customer = (read["Customer"]).ToString();
                                    model.Line = (read["Line"]).ToString();
                                    model.Machine = (read["Machine"]).ToString();
                                    model.Station = (read["Station"]).ToString();
                                    model.SN = (read["SN"]).ToString();
                                    model.BG = (read["BG"]).ToString();
                                    model.Fixture = (read["Fixture"]).ToString();
                                    model.LastUpdated = Convert.ToDateTime(read["LastUpdated"]);
                                    model.UserID = (read["UserID"]).ToString();
                                    model.LinkStatus = (read["LinkStatus"]).ToString();
                                    model.ReturnMsg = (read["ReturnMsg"]).ToString();
                                    model.CNReturnMsg = (read["CNReturnMsg"]).ToString();

                                    modelList.Add(model);
                                }
                            }
                        }
                        catch (Exception)
                        {
                            throw;
                        }
                        finally
                        {
                            cmd.Dispose();
                            conn.Close();
                        }
                    }
                }
            }
            return modelList;
        }


        /// <summary>
        /// 组装数据
        /// </summary>
        /// <param name="Fixtures"></param>
        /// <returns></returns>
        public IQueryable<FixtureDTO> SetFixtureDTO(List<FixtureDTO> Fixtures)
        {
            List<System_Organization> system_Organizations = DataContext.System_Organization.ToList();
            List<Vendor_Info> vendor_Infos = DataContext.Vendor_Info.ToList();
            List<FixtureStatuDTO> fixtureStatuDTOs = GetFixtureStatuDTO();
            foreach (var item in Fixtures)
            {
                // 设置厂区                            
                var system_OrganizationPlant = system_Organizations.Where(o => o.Organization_UID == item.Plant_Organization_UID).FirstOrDefault();
                if (system_OrganizationPlant != null)
                {
                    item.PlantName = system_OrganizationPlant.Organization_Name;
                }
                //设置OP
                var system_OrganizationOP = system_Organizations.Where(o => o.Organization_UID == item.BG_Organization_UID).FirstOrDefault();
                if (system_OrganizationOP != null)
                {
                    item.OPName = system_OrganizationOP.Organization_Name;
                }
                //设置功能厂
                if (item.FunPlant_Organization_UID != null && item.FunPlant_Organization_UID != 0)
                {
                    var system_OrganizationFunPlant = system_Organizations.Where(o => o.Organization_UID == item.FunPlant_Organization_UID).FirstOrDefault();
                    if (system_OrganizationFunPlant != null)
                    {
                        item.FunPlantName = system_OrganizationFunPlant.Organization_Name;
                    }
                }
                //设置厂商
                if (item.Vendor_Info_UID != null && item.Vendor_Info_UID != 0)
                {
                    var vendor_Info = vendor_Infos.Where(o => o.Vendor_Info_UID == item.Vendor_Info_UID).FirstOrDefault();
                    if (vendor_Info != null)
                    {
                        item.Vendor_Name = vendor_Info.Vendor_Name;
                    }
                }
                var status = fixtureStatuDTOs.Where(o => o.Status == item.Status).FirstOrDefault();
                if (status != null)
                {
                    item.StatuName = status.StatuName;
                }

            }
            return Fixtures.AsQueryable();
        }

        /// <summary>
        /// 根据ID获取当前的治具资料
        /// </summary>
        /// <param name="fixture_UID"></param>
        /// <returns></returns>
        public FixtureDTO QueryFixtureByUid(int fixture_UID)
        {

            var query = from fixture in DataContext.Fixture_M
                        select new FixtureDTO
                        {
                            Fixture_M_UID = fixture.Fixture_M_UID,
                            Plant_Organization_UID = fixture.Plant_Organization_UID,
                            BG_Organization_UID = fixture.BG_Organization_UID,
                            FunPlant_Organization_UID = fixture.FunPlant_Organization_UID,
                            Fixture_NO = fixture.Fixture_NO,
                            Version = fixture.Version,
                            Fixture_Seq = fixture.Fixture_Seq,
                            Fixture_Unique_ID = fixture.Fixture_Unique_ID,
                            Fixture_Name = fixture.Fixture_Name,
                            Project_UID = fixture.Project_UID,
                            Fixture_Machine_UID = fixture.Fixture_Machine_UID,
                            Vendor_Info_UID = fixture.Vendor_Info_UID,
                            Production_Line_UID = fixture.Production_Line_UID,
                            Status = fixture.Status,
                            ShortCode = fixture.ShortCode,
                            TwoD_Barcode = fixture.TwoD_Barcode,
                            Created_Date = fixture.Created_Date,
                            Created_UID = fixture.Created_UID,
                            Modified_Date = fixture.Modified_Date,
                            Modified_UID = fixture.Modified_UID,
                            Equipment_No = fixture.Fixture_Machine.Equipment_No,
                            Machine_Type = fixture.Fixture_Machine.Machine_Type,
                            Machine_ID = fixture.Fixture_Machine.Machine_ID,
                            Line_Name = fixture.Production_Line.Line_Name,
                            Workshop = fixture.Production_Line.Workshop.Workshop_Name,
                            Workshop_UID = fixture.Production_Line.Workshop_UID,
                            Workstation = fixture.Production_Line.WorkStation.WorkStation_Name,
                            Workstation_UID = fixture.Production_Line.Workstation_UID,
                            Process_Info = fixture.Production_Line.Process_Info.Process_Name,
                            Process_Info_UID = fixture.Production_Line.Process_Info_UID,
                            Project = fixture.System_Project.Project_Name,
                            Process_ID = fixture.Production_Line.Process_Info.Process_ID,
                            WorkStation_ID = fixture.Production_Line.WorkStation.WorkStation_ID,
                            Line_ID = fixture.Production_Line.Line_ID,
                            Createder = fixture.System_Users.User_Name,
                            Modifieder = fixture.System_Users1.User_Name
                        };
            query = query.Where(m => m.Fixture_M_UID == fixture_UID);
            var fixtures = SetListFixtureDTO(query.ToList());
            if (fixtures.Count > 0)
            {
                return fixtures.FirstOrDefault();
            }
            else
            {
                return null;
            }
        }
        public List<FixtureDTO> FixtureList(FixtureDTO searchModel)
        {
            var query = from fixture in DataContext.Fixture_M
                        select new FixtureDTO
                        {
                            Fixture_M_UID = fixture.Fixture_M_UID,
                            Plant_Organization_UID = fixture.Plant_Organization_UID,
                            BG_Organization_UID = fixture.BG_Organization_UID,
                            FunPlant_Organization_UID = fixture.FunPlant_Organization_UID,
                            Fixture_NO = fixture.Fixture_NO,
                            Version = fixture.Version,
                            Fixture_Seq = fixture.Fixture_Seq,
                            Fixture_Unique_ID = fixture.Fixture_Unique_ID,
                            Fixture_Name = fixture.Fixture_Name,
                            Project_UID = fixture.Project_UID,
                            Fixture_Machine_UID = fixture.Fixture_Machine_UID,
                            Vendor_Info_UID = fixture.Vendor_Info_UID,
                            Vendor_ID = fixture.Vendor_Info.Vendor_ID,
                            Production_Line_UID = fixture.Production_Line_UID,
                            Status = fixture.Status,
                            ShortCode = fixture.ShortCode,
                            TwoD_Barcode = fixture.TwoD_Barcode,
                            Created_Date = fixture.Created_Date,
                            Created_UID = fixture.Created_UID,
                            Modified_Date = fixture.Modified_Date,
                            Modified_UID = fixture.Modified_UID,
                            Equipment_No = fixture.Fixture_Machine.Equipment_No,
                            Machine_Type = fixture.Fixture_Machine.Machine_Type,
                            Machine_ID = fixture.Fixture_Machine.Machine_ID,
                            Line_Name = fixture.Production_Line.Line_Name,
                            Line_ID = fixture.Production_Line.Line_ID,
                            Workshop = fixture.Production_Line.Workshop.Workshop_Name,
                            Workshop_UID = fixture.Production_Line.Workshop_UID,
                            Workstation = fixture.Production_Line.WorkStation.WorkStation_Name,
                            Workstation_UID = fixture.Production_Line.Workstation_UID,
                            Process_Info = fixture.Production_Line.Process_Info.Process_Name,
                            Process_Info_UID = fixture.Production_Line.Process_Info_UID,
                            Project = fixture.System_Project.Project_Name,
                            Project_ID = fixture.System_Project.Project_Code,
                            Workshop_ID = fixture.Production_Line.Workshop.Workshop_ID,
                            Createder = fixture.System_Users.User_Name,
                            Modifieder = fixture.System_Users1.User_Name,
                        };
            if (searchModel.Plant_Organization_UID != 0)
                query = query.Where(m => m.Plant_Organization_UID == searchModel.Plant_Organization_UID);
            if (searchModel.BG_Organization_UID != 0)
                query = query.Where(m => m.BG_Organization_UID == searchModel.BG_Organization_UID);
            if (searchModel.FunPlant_Organization_UID != null && searchModel.FunPlant_Organization_UID != 0)
                query = query.Where(m => m.FunPlant_Organization_UID == searchModel.FunPlant_Organization_UID);
            if (!string.IsNullOrWhiteSpace(searchModel.Fixture_NO))
                query = query.Where(m => m.Fixture_NO.Contains(searchModel.Fixture_NO));
            if (!string.IsNullOrWhiteSpace(searchModel.Version))
                query = query.Where(m => m.Version.Contains(searchModel.Version));
            if (!string.IsNullOrWhiteSpace(searchModel.Fixture_Seq))
                query = query.Where(m => m.Fixture_Seq.Contains(searchModel.Fixture_Seq));
            if (!string.IsNullOrWhiteSpace(searchModel.Fixture_Unique_ID))
                query = query.Where(m => m.Fixture_Unique_ID == searchModel.Fixture_Unique_ID);
            if (!string.IsNullOrWhiteSpace(searchModel.Fixture_Name))
                query = query.Where(m => m.Fixture_Name.Contains(searchModel.Fixture_Name));
            if (searchModel.Project_UID != null && searchModel.Project_UID != 0)
                query = query.Where(m => m.Project_UID == searchModel.Project_UID);
            if (searchModel.Fixture_Machine_UID != null && searchModel.Fixture_Machine_UID != 0)
                query = query.Where(m => m.Fixture_Machine_UID == searchModel.Fixture_Machine_UID);
            if (searchModel.Vendor_Info_UID != null && searchModel.Vendor_Info_UID != 0)
                query = query.Where(m => m.Vendor_Info_UID == searchModel.Vendor_Info_UID);
            if (searchModel.Production_Line_UID != null && searchModel.Production_Line_UID != 0)
                query = query.Where(m => m.Production_Line_UID == searchModel.Production_Line_UID);
            if (searchModel.Process_Info_UID != null && searchModel.Process_Info_UID != 0)
                query = query.Where(m => m.Process_Info_UID == searchModel.Process_Info_UID);
            if (searchModel.Workshop_UID != null && searchModel.Workshop_UID != 0)
                query = query.Where(m => m.Workshop_UID == searchModel.Workshop_UID);
            if (searchModel.Workstation_UID != null && searchModel.Workstation_UID != 0)
                query = query.Where(m => m.Workstation_UID == searchModel.Workstation_UID);
            if (searchModel.Status != 0)
                query = query.Where(m => m.Status == searchModel.Status);
            if (!string.IsNullOrWhiteSpace(searchModel.ShortCode))
                query = query.Where(m => m.ShortCode.Contains(searchModel.ShortCode));
            if (!string.IsNullOrWhiteSpace(searchModel.TwoD_Barcode))
                query = query.Where(m => m.TwoD_Barcode.Contains(searchModel.TwoD_Barcode));
            if (searchModel.Created_Date != null)
            {
                DateTime nextTime = searchModel.Created_Date.Value.Date.AddDays(1);
                query = query.Where(m => m.Created_Date >= searchModel.Created_Date && m.Created_Date < nextTime);
            }
            if (!string.IsNullOrWhiteSpace(searchModel.Modifieder))
                query = query.Where(m => m.Modifieder.Contains(searchModel.Modifieder));
            if (searchModel.Modified_UID != 0)
                query = query.Where(m => m.Modified_UID == searchModel.Modified_UID);
            if (searchModel.End_Date_From != null)
                query = query.Where(m => m.Modified_Date >= searchModel.End_Date_From);
            if (searchModel.End_Date_To != null)
            {
                DateTime nextTime = searchModel.End_Date_To.Value.Date.AddDays(1);
                query = query.Where(m => m.Modified_Date < nextTime);
            }
       
            query = query.OrderByDescending(m => m.Modified_Date).ThenBy(m => m.Fixture_M_UID);

            query = SetFixtureDTO(query.ToList());
            return query.ToList();

        }


        public string BatchEnableFixturematerial(FixtureDTO searchModel, List<FixtureDTO> ListFixtureDTO, int status)
        {
            //2 更新数据
            //3 修改记录
            try
            {
                foreach (var item in ListFixtureDTO)
                {
                    string insertFixture_Resume = string.Format(@"INSERT INTO Fixture_Resume
                                                      (
	                                                     [Fixture_M_UID]
	                                                    ,[Data_Source]
	                                                    ,[Resume_Date]
	                                                    ,[Source_UID]
	                                                    ,[Source_NO]
	                                                    ,[Resume_Notes]
	                                                    ,[Modified_UID]
	                                                    ,Modified_Date)
                                                 VALUES ({0}, {1}, getdate(), {2}, N'{3}', N'{4}', {5},getdate())",
                                                         item.Fixture_M_UID,
                                                         "7",
                                                         item.Fixture_M_UID,
                                                         CreateSerialNumber(),
                                                         "治具主档状态异动-后台批次异动",
                                                         searchModel.AccountID
                                                       );
                    string updataFixture_M = string.Format(@"UPDATE [Fixture_M]
                                                         SET [Status] = {0},
                                                         [Modified_UID] = {1},
                                                         [Modified_Date] = getdate()
                                                        WHERE
	                                                      [Fixture_M_UID] = {2}", status, searchModel.AccountID, item.Fixture_M_UID
                                                           );

                    DataContext.Database.ExecuteSqlCommand(insertFixture_Resume);
                    DataContext.Database.ExecuteSqlCommand(updataFixture_M);
                }

                return "批量操作成功";

            }
            catch (Exception ex)
            {
                return "批量操作失败";
            }

        }

        public string CreateSerialNumber()
        {
            var querySql = @"SELECT 
                             'FM' + CONVERT(varchar(100), GETDATE(), 112) + '_' + RIGHT('0000' + CAST(COUNT(*) + 1 AS nvarchar(50)), 4) as SerialNumber
                              FROM[PDMS_Test].[dbo].[Fixture_Resume]
                            ";
            var time = DateTime.Now.ToString("yyyy-MM-dd");
            var querySql1 = $"WHERE Resume_Date> '{time} 00:00:00:000' AND Resume_Date< '{time} 23:59:59:999'  AND Data_Source = 7";
            querySql += querySql1;
            string SerialNumber = string.Empty;
            SerialNumber = DataContext.Database.SqlQuery<string>(querySql).First();
            return SerialNumber;
        }


        public List<FixtureDTO> FixtureList(int Plant_Organization_UID, int BG_Organization_UID, int FunPlant_Organization_UID)
        {

            var query = from fixture in DataContext.Fixture_M
                        select new FixtureDTO
                        {
                            Fixture_M_UID = fixture.Fixture_M_UID,
                            Plant_Organization_UID = fixture.Plant_Organization_UID,
                            BG_Organization_UID = fixture.BG_Organization_UID,
                            FunPlant_Organization_UID = fixture.FunPlant_Organization_UID,
                            Fixture_NO = fixture.Fixture_NO,
                            Version = fixture.Version,
                            Fixture_Seq = fixture.Fixture_Seq,
                            Fixture_Unique_ID = fixture.Fixture_Unique_ID,
                            Fixture_Name = fixture.Fixture_Name,
                            Project_UID = fixture.Project_UID,
                            Status = fixture.Status,
                            ShortCode = fixture.ShortCode,
                            TwoD_Barcode = fixture.TwoD_Barcode,
                            Created_Date = fixture.Created_Date,
                            Created_UID = fixture.Created_UID,
                            Modified_Date = fixture.Modified_Date,
                            Modified_UID = fixture.Modified_UID,
                            Createder = fixture.System_Users.User_Name,
                            Modifieder = fixture.System_Users1.User_Name
                        };
            if (Plant_Organization_UID != 0)
                query = query.Where(m => m.Plant_Organization_UID == Plant_Organization_UID);
            if (BG_Organization_UID != 0)
                query = query.Where(m => m.BG_Organization_UID == BG_Organization_UID);
            if (FunPlant_Organization_UID != 0)
                query = query.Where(m => m.FunPlant_Organization_UID == FunPlant_Organization_UID);
            return query.ToList();
        }
        public Fixture_M GetFixtureByUid(FixtureDTO searchModel)
        {
            var query = DataContext.Fixture_M.Where(o => o.Fixture_Unique_ID == searchModel.Fixture_Unique_ID);
            if (searchModel.Plant_Organization_UID != 0)
                query = query.Where(m => m.Plant_Organization_UID == searchModel.Plant_Organization_UID);
            if (searchModel.BG_Organization_UID != 0)
                query = query.Where(m => m.BG_Organization_UID == searchModel.BG_Organization_UID);
            if (searchModel.FunPlant_Organization_UID != null && searchModel.FunPlant_Organization_UID != 0)
                query = query.Where(m => m.FunPlant_Organization_UID == searchModel.FunPlant_Organization_UID);
            if (!string.IsNullOrWhiteSpace(searchModel.Fixture_NO))
                query = query.Where(m => m.Fixture_NO.Contains(searchModel.Fixture_NO));
            if (!string.IsNullOrWhiteSpace(searchModel.Version))
                query = query.Where(m => m.Version.Contains(searchModel.Version));
            if (!string.IsNullOrWhiteSpace(searchModel.Fixture_Seq))
                query = query.Where(m => m.Fixture_Seq.Contains(searchModel.Fixture_Seq));
            if (!string.IsNullOrWhiteSpace(searchModel.Fixture_Unique_ID))
                query = query.Where(m => m.Fixture_Unique_ID == searchModel.Fixture_Unique_ID);
            if (!string.IsNullOrWhiteSpace(searchModel.Fixture_Name))
                query = query.Where(m => m.Fixture_Name.Contains(searchModel.Fixture_Name));
            if (searchModel.Project_UID != null && searchModel.Project_UID != 0)
                query = query.Where(m => m.Project_UID == searchModel.Project_UID);
            if (searchModel.Fixture_Machine_UID != null && searchModel.Fixture_Machine_UID != 0)
                query = query.Where(m => m.Fixture_Machine_UID == searchModel.Fixture_Machine_UID);
            if (searchModel.Vendor_Info_UID != null && searchModel.Vendor_Info_UID != 0)
                query = query.Where(m => m.Vendor_Info_UID == searchModel.Vendor_Info_UID);
            if (searchModel.Production_Line_UID != null && searchModel.Production_Line_UID != 0)
                query = query.Where(m => m.Production_Line_UID == searchModel.Production_Line_UID);
            if (searchModel.Status != 0)
                query = query.Where(m => m.Status == searchModel.Status);
            if (!string.IsNullOrWhiteSpace(searchModel.ShortCode))
                query = query.Where(m => m.ShortCode.Contains(searchModel.ShortCode));
            if (!string.IsNullOrWhiteSpace(searchModel.TwoD_Barcode))
                query = query.Where(m => m.TwoD_Barcode.Contains(searchModel.TwoD_Barcode));
            if (searchModel.Modified_UID != 0)
                query = query.Where(m => m.Modified_UID == searchModel.Modified_UID);
            return query.FirstOrDefault();
        }
        /// <summary>
        /// 设置加载厂区，OP，功能厂，厂商
        /// </summary>
        /// <param name="Fixtures"></param>
        /// <returns></returns>
        public List<FixtureDTO> SetListFixtureDTO(List<FixtureDTO> Fixtures)
        {
            List<System_Organization> system_Organizations = DataContext.System_Organization.ToList();
            List<Vendor_Info> vendor_Infos = DataContext.Vendor_Info.ToList();
            List<FixtureStatuDTO> fixtureStatuDTOs = GetFixtureStatuDTO();
            foreach (var item in Fixtures)
            {
                // 设置厂区                            
                var system_OrganizationPlant = system_Organizations.Where(o => o.Organization_UID == item.Plant_Organization_UID).FirstOrDefault();
                if (system_OrganizationPlant != null)
                {
                    item.PlantName = system_OrganizationPlant.Organization_Name;
                }
                //设置OP
                var system_OrganizationOP = system_Organizations.Where(o => o.Organization_UID == item.BG_Organization_UID).FirstOrDefault();
                if (system_OrganizationOP != null)
                {
                    item.OPName = system_OrganizationOP.Organization_Name;
                }
                //设置功能厂
                if (item.FunPlant_Organization_UID != null && item.FunPlant_Organization_UID != 0)
                {
                    var system_OrganizationFunPlant = system_Organizations.Where(o => o.Organization_UID == item.FunPlant_Organization_UID).FirstOrDefault();
                    if (system_OrganizationFunPlant != null)
                    {
                        item.FunPlantName = system_OrganizationFunPlant.Organization_Name;
                    }
                }
                //设置厂商
                if (item.Vendor_Info_UID != null && item.Vendor_Info_UID != 0)
                {
                    var vendor_Info = vendor_Infos.Where(o => o.Vendor_Info_UID == item.Vendor_Info_UID).FirstOrDefault();
                    if (vendor_Info != null)
                    {
                        item.Vendor_Name = vendor_Info.Vendor_Name;
                    }
                }

                var status = fixtureStatuDTOs.Where(o => o.Status == item.Status).FirstOrDefault();
                if (status != null)
                {
                    item.StatuName = status.StatuName;
                }
            }
            return Fixtures;
        }
        public List<FixtureStatuDTO> GetFixtureStatuDTO()
        {
            ///治具狀態(1:使用中In - PRD; 2:未使用Non - PRD; 3.維修中In - Repair; 4.報廢Scrap; 5:返供應商維修RTV; 6:保養逾時Over - Due Maintenance)
            List<FixtureStatuDTO> fixtureStatuDTOs = new List<FixtureStatuDTO>();
            List<Enumeration> enumerationItems = DataContext.Enumeration.Where(o => o.Enum_Type == "Fixture_Status").ToList();
            // fixtureStatuDTOs.Add(new FixtureStatuDTO() { Status = 0, StatuName = "" });
            foreach (var item in enumerationItems)
            {
                FixtureStatuDTO fixtureStatuDTO = new FixtureStatuDTO();
                fixtureStatuDTO.StatuName = item.Enum_Value;
                fixtureStatuDTO.Status = item.Enum_UID;
                fixtureStatuDTOs.Add(fixtureStatuDTO);
            }
            return fixtureStatuDTOs;
        }

        public string Updatefixture_MAPI(List<FixtureDTO> dtolist)
        {
            try
            {
                if (dtolist != null && dtolist.Count > 0)
                {
                    using (var trans = DataContext.Database.BeginTransaction())
                    {
                        //全插操作
                        StringBuilder sb = new StringBuilder();
                        foreach (var item in dtolist)
                        {
                            string funPlant_Organization_UID = "null";
                            // dynamic funPlant_Organization_UID = DBNull.Value;
                            if (item.FunPlant_Organization_UID != null)
                            {
                                funPlant_Organization_UID = item.FunPlant_Organization_UID.ToString();
                            }
                            string project_UID = "null";
                            // dynamic project_UID= DBNull.Value;
                            if (item.Project_UID != 0)
                            {
                                project_UID = item.Project_UID.ToString();
                            }

                            // dynamic production_Line_UID = DBNull.Value;
                            string production_Line_UID = "null";
                            if (item.Production_Line_UID != 0)
                            {
                                production_Line_UID = item.Production_Line_UID.ToString();
                            }
                            // dynamic fixture_Machine_UID = DBNull.Value;
                            string fixture_Machine_UID = "null";
                            if (item.Fixture_Machine_UID != 0)
                            {
                                fixture_Machine_UID = item.Fixture_Machine_UID.ToString();
                            }
                            // dynamic vendor_Info_UID = DBNull.Value;
                            string vendor_Info_UID = "null";
                            if (item.Vendor_Info_UID != 0)
                            {
                                vendor_Info_UID = item.Vendor_Info_UID.ToString();
                            }

                            //var insertSql = string.Format(@"UPDATE Fixture_M
                            //                   SET Plant_Organization_UID = {0}
                            //                      ,BG_Organization_UID = {1}
                            //                      ,FunPlant_Organization_UID = {2}
                            //                      ,Fixture_NO = N'{3}'
                            //                      ,Version = N'{4}'
                            //                      ,Fixture_Seq = N'{5}'
                            //                      ,Fixture_Unique_ID = N'{6}'
                            //                      ,Fixture_Name = N'{7}'
                            //                      ,Project_UID = {8}
                            //                      ,Fixture_Machine_UID = {9}
                            //                      ,Vendor_Info_UID = {10}
                            //                      ,Production_Line_UID = {11}
                            //                      ,Status = {12}
                            //                      ,ShortCode = N'{13}'
                            //                      ,TwoD_Barcode = N'{14}'
                            //                      ,Modified_UID = {15}
                            //                      ,Modified_Date = N'{16}'
                            //                       WHERE Fixture_M_UID = {17}",
                            //                       item.Plant_Organization_UID,
                            //                       item.BG_Organization_UID,
                            //                       funPlant_Organization_UID, // item.FunPlant_Organization_UID== null ? DBNull.Value : item.FunPlant_Organization_UID,
                            //                       item.Fixture_NO,
                            //                       item.Version,
                            //                       item.Fixture_Seq,
                            //                       item.Fixture_Unique_ID,
                            //                       item.Fixture_Name,
                            //                       project_UID,//  item.Project_UID==0? DBNull.Value : item.Project_UID,
                            //                       fixture_Machine_UID,//item.Fixture_Machine_UID == 0 ? DBNull.Value : item.Fixture_Machine_UID,
                            //                       vendor_Info_UID,
                            //                       production_Line_UID,//item.Production_Line_UID == 0 ? DBNull.Value : item.Production_Line_UID,
                            //                       item.Status,
                            //                       item.ShortCode,
                            //                       item.TwoD_Barcode,
                            //                       item.Modified_UID,
                            //                       DateTime.Now.ToString(FormatConstants.DateTimeFormatString),
                            //                       item.Fixture_M_UID
                            //                      );


                            var insertSql = string.Format(@"UPDATE Fixture_M
                                               SET Plant_Organization_UID = {0}
                                                  ,BG_Organization_UID = {1}
                                                  ,FunPlant_Organization_UID = {2}
                                                  ,Fixture_NO = N'{3}'
                                                  ,Version = N'{4}'
                                                  ,Fixture_Seq = N'{5}'
                                                  ,Fixture_Unique_ID = N'{6}'
                                                  ,Fixture_Name = N'{7}'
                                                  ,Project_UID = {8}
                                                  ,Fixture_Machine_UID = {9}
                                                  ,Vendor_Info_UID = {10}
                                                  ,Production_Line_UID = {11}
                                                  ,ShortCode = N'{12}'
                                                  ,TwoD_Barcode = N'{13}'
                                                  ,Modified_UID = {14}
                                                  ,Modified_Date = N'{15}'
                                                   WHERE Fixture_M_UID = {16}",
                       item.Plant_Organization_UID,
                       item.BG_Organization_UID,
                       funPlant_Organization_UID, // item.FunPlant_Organization_UID== null ? DBNull.Value : item.FunPlant_Organization_UID,
                       item.Fixture_NO,
                       item.Version,
                       item.Fixture_Seq,
                       item.Fixture_Unique_ID,
                       item.Fixture_Name,
                       project_UID,//  item.Project_UID==0? DBNull.Value : item.Project_UID,
                       fixture_Machine_UID,//item.Fixture_Machine_UID == 0 ? DBNull.Value : item.Fixture_Machine_UID,
                       vendor_Info_UID,
                       production_Line_UID,//item.Production_Line_UID == 0 ? DBNull.Value : item.Production_Line_UID,
                       item.ShortCode,
                       item.TwoD_Barcode,
                       item.Modified_UID,
                       DateTime.Now.ToString(FormatConstants.DateTimeFormatString),
                       item.Fixture_M_UID
                      );
                            sb.AppendLine(insertSql);

                        }
                        string sql = sb.ToString();
                        DataContext.Database.ExecuteSqlCommand(sb.ToString());
                        trans.Commit();
                    }
                }
                return "更新成功";

            }
            catch (Exception e)
            {
                return e.Message;
            }
        }
        /// <summary>
        /// 获取供应商信息
        /// </summary>
        /// <param name="Plant_Organization_UID"></param>
        /// <param name="BG_Organization_UID"></param>
        /// <returns></returns>
        public List<Vendor_InfoDTO> GetVendor_InfoList(int Plant_Organization_UID, int BG_Organization_UID)
        {
            var query = from vendor_Info in DataContext.Vendor_Info
                        select new Vendor_InfoDTO
                        {
                            Vendor_Info_UID = vendor_Info.Vendor_Info_UID,
                            Plant_Organization_UID = vendor_Info.Plant_Organization_UID,
                            BG_Organization_UID = vendor_Info.BG_Organization_UID,
                            Vendor_ID = vendor_Info.Vendor_ID,
                            Vendor_Name = vendor_Info.Vendor_Name,
                            Is_Enable = vendor_Info.Is_Enable
                        };
            query = query.Where(m => m.Is_Enable == true);
            if (Plant_Organization_UID != 0)
                query = query.Where(m => m.Plant_Organization_UID == Plant_Organization_UID);
            if (BG_Organization_UID != 0)
                query = query.Where(m => m.BG_Organization_UID == BG_Organization_UID);
            return query.ToList();
        }
        /// <summary>
        /// 根据厂区 ，OP 功能厂获取 用户
        /// </summary>
        /// <param name="Plant_Organization_UID"></param>
        /// <param name="BG_Organization_UID"></param>
        /// <param name="FunPlant_Organization_UID"></param>
        /// <returns></returns>
        public List<FixtureSystemUserDTO> GetFixtureSystemUser(int Plant_Organization_UID, int BG_Organization_UID, int FunPlant_Organization_UID)
        {
            var query = from system_UserOrg in DataContext.System_UserOrg
                        select new FixtureSystemUserDTO
                        {

                            Plant_Organization_UID = system_UserOrg.Plant_OrganizationUID,
                            BG_Organization_UID = system_UserOrg.OPType_OrganizationUID,
                            FunPlant_Organization_UID = system_UserOrg.Funplant_OrganizationUID,
                            Account_UID = system_UserOrg.Account_UID,
                            User_NTID = system_UserOrg.System_Users.User_NTID,
                            User_Name = system_UserOrg.System_Users.User_Name,
                            User_NTID_Name = system_UserOrg.System_Users.User_NTID + "_" + system_UserOrg.System_Users.User_Name,
                            Enable_Flag = system_UserOrg.System_Users.Enable_Flag
                        };
            query = query.Where(m => m.Enable_Flag == true);
            if (Plant_Organization_UID != 0)
                query = query.Where(m => m.Plant_Organization_UID == Plant_Organization_UID);
            if (BG_Organization_UID != 0)
                query = query.Where(m => m.BG_Organization_UID == BG_Organization_UID);
            if (FunPlant_Organization_UID != 0)
                query = query.Where(m => m.FunPlant_Organization_UID == FunPlant_Organization_UID);
            return query.ToList();

        }
        /// <summary>
        /// 根据功能厂，OP，厂区获取产线
        /// </summary>
        /// <param name="Plant_Organization_UID"></param>
        /// <param name="BG_Organization_UID"></param>
        /// <param name="FunPlant_Organization_UID"></param>
        /// <returns></returns>
        public List<Production_LineDTO> GetProductionLineDTOList(int Plant_Organization_UID, int BG_Organization_UID, int FunPlant_Organization_UID)
        {
            var query = from production_Line in DataContext.Production_Line
                        select new Production_LineDTO
                        {
                            Production_Line_UID = production_Line.Production_Line_UID,
                            Plant_Organization_UID = production_Line.Plant_Organization_UID,
                            BG_Organization_UID = production_Line.BG_Organization_UID,
                            FunPlant_Organization_UID = production_Line.FunPlant_Organization_UID,
                            Line_ID = production_Line.Line_ID,
                            Line_Name = production_Line.Line_Name,
                            Line_Desc = production_Line.Line_Desc,
                            Workshop_UID = production_Line.Workshop_UID,
                            Workstation_UID = production_Line.Workstation_UID,
                            Project_UID = production_Line.Project_UID,
                            Process_Info_UID = production_Line.Process_Info_UID,
                            Workshop = production_Line.Workshop.Workshop_Name,
                            Workstation = production_Line.WorkStation.WorkStation_Name,
                            Project = production_Line.System_Project.Project_Name,
                            Process_Info = production_Line.Process_Info.Process_Name,
                            Is_Enable = production_Line.Is_Enable
                        };
            query = query.Where(m => m.Is_Enable == true);
            if (Plant_Organization_UID != 0)
                query = query.Where(m => m.Plant_Organization_UID == Plant_Organization_UID);
            if (BG_Organization_UID != 0)
                query = query.Where(m => m.BG_Organization_UID == BG_Organization_UID);
            if (FunPlant_Organization_UID != 0)
                query = query.Where(m => m.FunPlant_Organization_UID == FunPlant_Organization_UID);
            return query.ToList();
        }
        public List<WorkshopDTO> GetWorkshopList(int Plant_Organization_UID, int BG_Organization_UID, int FunPlant_Organization_UID)
        {
            var query = from production_Line in DataContext.Workshop
                        select new WorkshopDTO
                        {
                            Workshop_UID = production_Line.Workshop_UID,
                            Plant_Organization_UID = production_Line.Plant_Organization_UID,
                            BG_Organization_UID = production_Line.BG_Organization_UID,
                            FunPlant_Organization_UID = production_Line.FunPlant_Organization_UID,
                            Workshop_ID = production_Line.Workshop_ID,
                            Building_Name = production_Line.Building_Name,
                            Floor_Name = production_Line.Floor_Name,
                            Workshop_Name = production_Line.Workshop_Name,
                            Is_Enable = production_Line.Is_Enable
                        };
            query = query.Where(m => m.Is_Enable == true);
            if (Plant_Organization_UID != 0)
                query = query.Where(m => m.Plant_Organization_UID == Plant_Organization_UID);
            if (BG_Organization_UID != 0)
                query = query.Where(m => m.BG_Organization_UID == BG_Organization_UID);
            if (FunPlant_Organization_UID != 0)
                query = query.Where(m => m.FunPlant_Organization_UID == FunPlant_Organization_UID);
            return query.ToList();

        }
        public List<WorkStationDTO> GetWorkstationList(int Plant_Organization_UID, int BG_Organization_UID, int FunPlant_Organization_UID)
        {

            var query = from production_Line in DataContext.WorkStation
                        select new WorkStationDTO
                        {
                            WorkStation_UID = production_Line.WorkStation_UID,
                            WorkStation_ID = production_Line.WorkStation_ID,
                            WorkStation_Name = production_Line.WorkStation_Name,
                            WorkStation_Desc = production_Line.WorkStation_Desc,
                            Plant_Organization_UID = production_Line.Plant_Organization_UID,
                            BG_Organization_UID = production_Line.BG_Organization_UID,
                            FunPlant_Organization_UID = production_Line.FunPlant_Organization_UID,
                            Is_Enable = production_Line.Is_Enable,
                            Project_UID = production_Line.Project_UID,
                            Project_Name = production_Line.System_Project.Project_Name,
                            Process_Info_UID = production_Line.Process_Info_UID,
                            Process_Name = production_Line.Process_Info.Process_Name
                        };
            query = query.Where(m => m.Is_Enable == true);
            if (Plant_Organization_UID != 0)
                query = query.Where(m => m.Plant_Organization_UID == Plant_Organization_UID);
            if (BG_Organization_UID != 0)
                query = query.Where(m => m.BG_Organization_UID == BG_Organization_UID);
            if (FunPlant_Organization_UID != 0)
                query = query.Where(m => m.FunPlant_Organization_UID == FunPlant_Organization_UID);
            return query.ToList();
        }
        public List<Process_InfoDTO> GetProcess_InfoList(int Plant_Organization_UID, int BG_Organization_UID, int FunPlant_Organization_UID)
        {
            var query = from production_Line in DataContext.Process_Info
                        select new Process_InfoDTO
                        {
                            Process_Info_UID = production_Line.Process_Info_UID,
                            Plant_Organization_UID = production_Line.Plant_Organization_UID,
                            BG_Organization_UID = production_Line.BG_Organization_UID,
                            FunPlant_Organization_UID = production_Line.FunPlant_Organization_UID,
                            Process_ID = production_Line.Process_ID,
                            Process_Name = production_Line.Process_Name,
                            Process_Desc = production_Line.Process_Desc,
                            Is_Enable = production_Line.Is_Enable
                        };
            query = query.Where(m => m.Is_Enable == true);
            if (Plant_Organization_UID != 0)
                query = query.Where(m => m.Plant_Organization_UID == Plant_Organization_UID);
            if (BG_Organization_UID != 0)
                query = query.Where(m => m.BG_Organization_UID == BG_Organization_UID);
            if (FunPlant_Organization_UID != 0)
                query = query.Where(m => m.FunPlant_Organization_UID == FunPlant_Organization_UID);
            return query.ToList();
        }
        public List<Fixture_DefectCodeDTO> GetDefectCodeList(int Plant_Organization_UID, int BG_Organization_UID, int FunPlant_Organization_UID)
        {
            var query = from defectCode in DataContext.Fixture_DefectCode
                        where defectCode.Is_Enable.Equals(true)
                        select new Fixture_DefectCodeDTO
                        {
                            Fixture_Defect_UID = defectCode.Fixture_Defect_UID,
                            Plant_Organization_UID = defectCode.Plant_Organization_UID,
                            BG_Organization_UID = defectCode.BG_Organization_UID,
                            FunPlant_Organization_UID = defectCode.FunPlant_Organization_UID,
                            DefectCode_ID = defectCode.DefectCode_ID,
                            DefectCode_Name = defectCode.DefectCode_Name,
                            Is_Enable = defectCode.Is_Enable
                        };
            if (Plant_Organization_UID != 0)
                query = query.Where(m => m.Plant_Organization_UID == Plant_Organization_UID);
            if (BG_Organization_UID != 0)
                query = query.Where(m => m.BG_Organization_UID == BG_Organization_UID);
            if (FunPlant_Organization_UID != 0)
                query = query.Where(m => m.FunPlant_Organization_UID == FunPlant_Organization_UID);
            return query.ToList();
        }
        public List<Fixture_RepairSolutionDTO> GeRepairSolutionList(int Plant_Organization_UID, int BG_Organization_UID, int FunPlant_Organization_UID)
        {
            var query = from repairSolution in DataContext.Fixture_RepairSolution
                        where repairSolution.Is_Enable.Equals(true)
                        select new Fixture_RepairSolutionDTO
                        {
                            Fixture_RepairSolution_UID = repairSolution.Fixture_RepairSolution_UID,
                            Plant_Organization_UID = repairSolution.Plant_Organization_UID,
                            BG_Organization_UID = repairSolution.BG_Organization_UID,
                            FunPlant_Organization_UID = repairSolution.FunPlant_Organization_UID,
                            RepairSolution_ID = repairSolution.RepairSolution_ID,
                            RepairSolution_Name = repairSolution.RepairSolution_Name,
                            Is_Enable = repairSolution.Is_Enable
                        };
            if (Plant_Organization_UID != 0)
                query = query.Where(m => m.Plant_Organization_UID == Plant_Organization_UID);
            if (BG_Organization_UID != 0)
                query = query.Where(m => m.BG_Organization_UID == BG_Organization_UID);
            if (FunPlant_Organization_UID != 0)
                query = query.Where(m => m.FunPlant_Organization_UID == FunPlant_Organization_UID);
            return query.ToList();
        }
        public List<SystemProjectDTO> GetProjectList(int Plant_Organization_UID, int BG_Organization_UID, int FunPlant_Organization_UID)
        {

            var query = from production_Line in DataContext.System_Project
                        select new SystemProjectDTO
                        {

                        };

            return query.ToList();
        }
        /// <summary>
        /// 获取产线数据
        /// </summary>
        /// <param name="Production_Line_UID"></param>
        /// <returns></returns>
        public Production_LineDTO GetProductionLineDTO(int Production_Line_UID)
        {
            var query = from production_Line in DataContext.Production_Line
                        select new Production_LineDTO
                        {
                            Production_Line_UID = production_Line.Production_Line_UID,
                            Plant_Organization_UID = production_Line.Plant_Organization_UID,
                            BG_Organization_UID = production_Line.BG_Organization_UID,
                            FunPlant_Organization_UID = production_Line.FunPlant_Organization_UID,
                            Line_ID = production_Line.Line_ID,
                            Line_Name = production_Line.Line_Name,
                            Line_Desc = production_Line.Line_Desc,
                            Workshop_UID = production_Line.Workshop_UID,
                            Workstation_UID = production_Line.Workstation_UID,
                            Project_UID = production_Line.Project_UID,
                            Process_Info_UID = production_Line.Process_Info_UID,
                            Workshop = production_Line.Workshop.Workshop_Name,
                            Workstation = production_Line.WorkStation.WorkStation_Name,
                            Project = production_Line.System_Project.Project_Name,
                            Process_Info = production_Line.Process_Info.Process_Name,
                            Is_Enable = production_Line.Is_Enable
                        };

            query = query.Where(m => m.Production_Line_UID == Production_Line_UID);
            var production_LineDTOs = query.ToList();
            if (production_LineDTOs.Count > 0)
            {
                return production_LineDTOs.FirstOrDefault();
            }
            else
            {
                return null;
            }

        }
        public List<FixtureMachineDTO> GetFixtureMachineDTOList(int Plant_Organization_UID, int BG_Organization_UID, int FunPlant_Organization_UID, int Production_Line_UID)
        {
            var query = from fixture_Machine in DataContext.Fixture_Machine
                        select new FixtureMachineDTO
                        {
                            Fixture_Machine_UID = fixture_Machine.Fixture_Machine_UID,
                            Plant_Organization_UID = fixture_Machine.Plant_Organization_UID,
                            BG_Organization_UID = fixture_Machine.BG_Organization_UID,
                            FunPlant_Organization_UID = fixture_Machine.FunPlant_Organization_UID,
                            Machine_ID = fixture_Machine.Machine_ID,
                            Equipment_No = fixture_Machine.Equipment_No,
                            Machine_Name = fixture_Machine.Machine_Name,
                            Machine_Desc = fixture_Machine.Machine_Desc,
                            Machine_Type = fixture_Machine.Machine_Type,
                            Production_Line_UID = fixture_Machine.Production_Line_UID,
                            Is_Enable = fixture_Machine.Is_Enable,
                            Created_UID = fixture_Machine.Created_UID,
                            Machine_IDandName = fixture_Machine.Machine_ID + "_" + fixture_Machine.Machine_Name

                        };
            query = query.Where(m => m.Is_Enable == true);
            if (Plant_Organization_UID != 0)
                query = query.Where(m => m.Plant_Organization_UID == Plant_Organization_UID);
            if (BG_Organization_UID != 0)
                query = query.Where(m => m.BG_Organization_UID == BG_Organization_UID);
            if (FunPlant_Organization_UID != 0)
                query = query.Where(m => m.FunPlant_Organization_UID == FunPlant_Organization_UID);
            if (Production_Line_UID != 0)
                query = query.Where(m => m.Production_Line_UID == Production_Line_UID);
            return query.ToList();
        }
        /// <summary>
        /// 根据治具机台ID获取治具机台数据
        /// </summary>
        /// <param name="Fixture_Machine_UID"></param>
        /// <returns></returns>
        public FixtureMachineDTO GetFixtureMachineByUid(int Fixture_Machine_UID)
        {

            var query = from fixture_Machine in DataContext.Fixture_Machine
                        select new FixtureMachineDTO
                        {
                            Fixture_Machine_UID = fixture_Machine.Fixture_Machine_UID,
                            Plant_Organization_UID = fixture_Machine.Plant_Organization_UID,
                            BG_Organization_UID = fixture_Machine.BG_Organization_UID,
                            FunPlant_Organization_UID = fixture_Machine.FunPlant_Organization_UID,
                            Machine_ID = fixture_Machine.Machine_ID,
                            Equipment_No = fixture_Machine.Equipment_No,
                            Machine_Name = fixture_Machine.Machine_Name,
                            Machine_Desc = fixture_Machine.Machine_Desc,
                            Machine_Type = fixture_Machine.Machine_Type,
                            Production_Line_UID = fixture_Machine.Production_Line_UID,
                            Is_Enable = fixture_Machine.Is_Enable,
                            Created_UID = fixture_Machine.Created_UID

                        };
            query = query.Where(m => m.Fixture_Machine_UID == Fixture_Machine_UID);
            var fixture_MachineDTOs = query.ToList();
            if (fixture_MachineDTOs.Count > 0)
            {
                return fixture_MachineDTOs.FirstOrDefault();
            }
            else
            {
                return null;
            }

        }
        /// <summary>
        /// 删除治具记录
        /// </summary>
        /// <param name="Fixture_M_UID"></param>
        /// <returns></returns>
        public string DeleteByUid(int Fixture_M_UID)
        {
            try
            {
                var fixture_M = DataContext.Fixture_M.Where(o => o.Fixture_M_UID == Fixture_M_UID).FirstOrDefault();
                if (fixture_M.Fixture_Maintenance_Record.Count > 0 || fixture_M.Fixture_Repair_D.Count > 0 ||
                    fixture_M.Fixture_Resume.Count > 0 || fixture_M.Fixture_Totake_D.Count > 0)
                {
                    return "此数据在使用中,请删除相关联数据,再删除,谢谢!";
                }
                string sql = "delete  Fixture_M  where Fixture_M_UID={0}";
                sql = string.Format(sql, Fixture_M_UID);
                int result = DataContext.Database.ExecuteSqlCommand(sql);
                if (result == 1)
                    return "删除成功！";
                else
                    return "删除治具记录失败！";
            }
            catch (Exception e)
            {
                return "删除治具记录失败:" + e.Message;
            }
        }
        /// <summary>
        /// 导出治具数据
        /// </summary>
        /// <param name="Fixture_M_UIDs"></param>
        /// <returns></returns>
        public List<FixtureDTO> DoExportFixtureReprot(string Fixture_M_UIDs)
        {
            Fixture_M_UIDs = "," + Fixture_M_UIDs + ",";
            var query = from fixture in DataContext.Fixture_M
                        select new FixtureDTO
                        {
                            Fixture_M_UID = fixture.Fixture_M_UID,
                            Plant_Organization_UID = fixture.Plant_Organization_UID,
                            BG_Organization_UID = fixture.BG_Organization_UID,
                            FunPlant_Organization_UID = fixture.FunPlant_Organization_UID,
                            Fixture_NO = fixture.Fixture_NO,
                            Version = fixture.Version,
                            Fixture_Seq = fixture.Fixture_Seq,
                            Fixture_Unique_ID = fixture.Fixture_Unique_ID,
                            Fixture_Name = fixture.Fixture_Name,
                            Project_UID = fixture.Project_UID,
                            Fixture_Machine_UID = fixture.Fixture_Machine_UID,
                            Vendor_Info_UID = fixture.Vendor_Info_UID,
                            Vendor_ID = fixture.Vendor_Info.Vendor_ID,
                            Production_Line_UID = fixture.Production_Line_UID,
                            Status = fixture.Status,
                            ShortCode = fixture.ShortCode,
                            TwoD_Barcode = fixture.TwoD_Barcode,
                            Created_Date = fixture.Created_Date,
                            Created_UID = fixture.Created_UID,
                            Modified_Date = fixture.Modified_Date,
                            Modified_UID = fixture.Modified_UID,
                            Equipment_No = fixture.Fixture_Machine.Equipment_No,
                            Machine_Type = fixture.Fixture_Machine.Machine_Type,
                            Machine_ID = fixture.Fixture_Machine.Machine_ID,
                            Line_Name = fixture.Production_Line.Line_Name,
                            Line_ID = fixture.Production_Line.Line_ID,
                            Workshop = fixture.Production_Line.Workshop.Workshop_Name,
                            Workshop_UID = fixture.Production_Line.Workshop_UID,
                            Workstation = fixture.Production_Line.WorkStation.WorkStation_Name,
                            Workstation_UID = fixture.Production_Line.Workstation_UID,
                            Process_Info = fixture.Production_Line.Process_Info.Process_Name,
                            Process_Info_UID = fixture.Production_Line.Process_Info_UID,
                            Project = fixture.System_Project.Project_Name,
                            Project_ID = fixture.System_Project.Project_Code,
                            Workshop_ID = fixture.Production_Line.Workshop.Workshop_ID,
                            Createder = fixture.System_Users.User_Name,
                            Modifieder = fixture.System_Users1.User_Name,

                        };
            query = query.Where(m => Fixture_M_UIDs.Contains("," + m.Fixture_M_UID + ","));
            var fixtures = SetListFixtureDTO(query.ToList());
            return fixtures;

        }
        /// <summary>
        /// 获取序号
        /// </summary>
        /// <param name="searchModel"></param>
        /// <returns></returns>
        public int GetFixtureCount(FixtureDTO searchModel)
        {
            var query = from fixture in DataContext.Fixture_M
                        select new FixtureDTO
                        {
                            Fixture_M_UID = fixture.Fixture_M_UID,
                            Plant_Organization_UID = fixture.Plant_Organization_UID,
                            BG_Organization_UID = fixture.BG_Organization_UID,
                            FunPlant_Organization_UID = fixture.FunPlant_Organization_UID,
                            Fixture_NO = fixture.Fixture_NO,
                            Version = fixture.Version,
                            Fixture_Seq = fixture.Fixture_Seq,
                            Fixture_Unique_ID = fixture.Fixture_Unique_ID,
                            Fixture_Name = fixture.Fixture_Name,
                            Created_Date = fixture.Created_Date
                        };
            if (searchModel.Plant_Organization_UID != 0)
                query = query.Where(m => m.Plant_Organization_UID == searchModel.Plant_Organization_UID);
            if (searchModel.BG_Organization_UID != 0)
                query = query.Where(m => m.BG_Organization_UID == searchModel.BG_Organization_UID);
            if (searchModel.FunPlant_Organization_UID != null && searchModel.FunPlant_Organization_UID != 0)
                query = query.Where(m => m.FunPlant_Organization_UID == searchModel.FunPlant_Organization_UID);
            var dateTime = DateTime.Now.Date;
            query = query.Where(m => m.Created_Date >= dateTime);
            DateTime nextTime = dateTime.Date.AddDays(1);
            query = query.Where(m => m.Created_Date < nextTime);
            return query.Count();
        }
        #region 治具履历查询-----------------------Add by Rock 2017-10-03 -----------Start
        public PagedListModel<FixtureResumeSearchVM> FixtureResumeSearchVM(FixtureResumeSearchVM searchVM, Page page, out int totalcount)
        {


            var linq = from A in DataContext.Fixture_Resume
                       join B in DataContext.Fixture_M
                       on A.Fixture_M_UID equals B.Fixture_M_UID

                       join C in DataContext.Production_Line
                       on B.Production_Line_UID equals C.Production_Line_UID into BCTemp
                       from BC in BCTemp.DefaultIfEmpty()

                       join D in DataContext.Process_Info
                       on BC.Process_Info_UID equals D.Process_Info_UID
                       join E in DataContext.WorkStation
                       on BC.Workstation_UID equals E.WorkStation_UID
                       join F in DataContext.Fixture_Machine
                       on B.Fixture_Machine_UID equals F.Fixture_Machine_UID into BFTemp
                       from BF in BFTemp.DefaultIfEmpty()
                       join G in DataContext.Vendor_Info
                       on B.Vendor_Info_UID equals G.Vendor_Info_UID into BGTemp
                       from BG in BGTemp.DefaultIfEmpty()
                       join H in DataContext.System_Organization
                       on B.Plant_Organization_UID equals H.Organization_UID
                       join I in DataContext.System_Organization
                       on B.BG_Organization_UID equals I.Organization_UID
                       join J in DataContext.System_Organization
                       on B.FunPlant_Organization_UID equals J.Organization_UID into BJTemp
                       from BJ in BJTemp.DefaultIfEmpty()
                       join M in DataContext.System_Users
                       on A.Modified_UID equals M.Account_UID
                       //where  B.FunPlant_Organization_UID == null
                       select new FixtureResumeSearchVM
                       {
                           Plant_Organization_UID = H.Organization_UID,
                           BG_Organization_UID = I.Organization_UID,
                           FunPlant_Organization_UID = BJ.Organization_UID,
                           PlantName = H.Organization_Name,
                           OpType_Name = I.Organization_Name,
                           Func_Name = BJ.Organization_Name,
                           Fixture_M_UID = B.Fixture_M_UID,
                           Fixture_Unique_ID = B.Fixture_Unique_ID,
                           Fixture_NO = B.Fixture_NO,
                           Version = B.Version,
                           Status = B.Status,
                           Resume_Notes = A.Resume_Notes,
                           //FixStatus_Name = B.Status == 1 ? "使用中In-PRD" :
                           //                  B.Status == 2 ? "未使用Non-PRD" :
                           //                  B.Status == 3 ? "维修中In-Repair" :
                           //                  B.Status == 4 ? "报废Scrap" :
                           //                  B.Status == 5 ? "返供应商维修RTV" :
                           //                  B.Status == 6 ? "保养逾时Over-Due Maintenance" : "",
                           Fixture_Name = B.Fixture_Name,
                           ShortCode = B.ShortCode,
                           Process_Info_UID = D.Process_Info_UID,
                           Process_Name = D.Process_Name,
                           WorkStation_UID = E.WorkStation_UID,
                           WorkStation_Name = E.WorkStation_Name,
                           Equipment_No = BF.Equipment_No,
                           Vendor_Name = BG.Vendor_Name,
                           Source_NO = A.Source_NO,
                           Production_Line_UID = BC.Production_Line_UID,
                           Line_Name = BC.Line_Name,
                           Modified_UID = A.Modified_UID,
                           User_Name = M.User_Name,
                           Modified_Date = A.Modified_Date,
                           Fixture_Resume_UID = A.Fixture_Resume_UID
                       };


            //若查询日期，则Union 7天前的历史数据
            if (searchVM.End_Date_From != null || searchVM.End_Date_To != null)
            {
                linq = linq.Union(
                            from A in DataContext.Fixture_Resume_History
                            join B in DataContext.Fixture_M
                            on A.Fixture_M_UID equals B.Fixture_M_UID

                            join C in DataContext.Production_Line
                            on B.Production_Line_UID equals C.Production_Line_UID into BCTemp
                            from BC in BCTemp.DefaultIfEmpty()

                            join D in DataContext.Process_Info
                            on BC.Process_Info_UID equals D.Process_Info_UID
                            join E in DataContext.WorkStation
                            on BC.Workstation_UID equals E.WorkStation_UID
                            join F in DataContext.Fixture_Machine
                            on B.Fixture_Machine_UID equals F.Fixture_Machine_UID into BFTemp
                            from BF in BFTemp.DefaultIfEmpty()
                            join G in DataContext.Vendor_Info
                            on B.Vendor_Info_UID equals G.Vendor_Info_UID into BGTemp
                            from BG in BGTemp.DefaultIfEmpty()
                            join H in DataContext.System_Organization
                            on B.Plant_Organization_UID equals H.Organization_UID
                            join I in DataContext.System_Organization
                            on B.BG_Organization_UID equals I.Organization_UID
                            join J in DataContext.System_Organization
                            on B.FunPlant_Organization_UID equals J.Organization_UID into BJTemp
                            from BJ in BJTemp.DefaultIfEmpty()
                            join M in DataContext.System_Users
                            on A.Modified_UID equals M.Account_UID
                            //where  B.FunPlant_Organization_UID == null
                            select new FixtureResumeSearchVM
                            {
                                Plant_Organization_UID = H.Organization_UID,
                                BG_Organization_UID = I.Organization_UID,
                                FunPlant_Organization_UID = BJ.Organization_UID,
                                PlantName = H.Organization_Name,
                                OpType_Name = I.Organization_Name,
                                Func_Name = BJ.Organization_Name,
                                Fixture_M_UID = B.Fixture_M_UID,
                                Fixture_Unique_ID = B.Fixture_Unique_ID,
                                Fixture_NO = B.Fixture_NO,
                                Version = B.Version,
                                Status = B.Status,
                                Resume_Notes = A.Resume_Notes,
                                //FixStatus_Name = B.Status == 1 ? "使用中In-PRD" :
                                //                  B.Status == 2 ? "未使用Non-PRD" :
                                //                  B.Status == 3 ? "维修中In-Repair" :
                                //                  B.Status == 4 ? "报废Scrap" :
                                //                  B.Status == 5 ? "返供应商维修RTV" :
                                //                  B.Status == 6 ? "保养逾时Over-Due Maintenance" : "",
                                Fixture_Name = B.Fixture_Name,
                                ShortCode = B.ShortCode,
                                Process_Info_UID = D.Process_Info_UID,
                                Process_Name = D.Process_Name,
                                WorkStation_UID = E.WorkStation_UID,
                                WorkStation_Name = E.WorkStation_Name,
                                Equipment_No = BF.Equipment_No,
                                Vendor_Name = BG.Vendor_Name,
                                Source_NO = A.Source_NO,
                                Production_Line_UID = BC.Production_Line_UID,
                                Line_Name = BC.Line_Name,
                                Modified_UID = A.Modified_UID,
                                User_Name = M.User_Name,
                                Modified_Date = A.Modified_Date,
                                Fixture_Resume_UID = A.Fixture_Resume_UID
                            }
  );
            }
            if (searchVM.Plant_Organization_UID != 0)
            {
                linq = linq.Where(m => m.Plant_Organization_UID.Equals(searchVM.Plant_Organization_UID));
            }
            if (searchVM.BG_Organization_UID != 0)
            {
                linq = linq.Where(m => m.BG_Organization_UID.Equals(searchVM.BG_Organization_UID));
            }
            if (searchVM.FunPlant_Organization_UID != null && searchVM.FunPlant_Organization_UID != 0)
            {
                linq = linq.Where(m => m.FunPlant_Organization_UID.Value.Equals(searchVM.FunPlant_Organization_UID.Value));
            }
            if (!string.IsNullOrWhiteSpace(searchVM.Fixture_Unique_ID))
            {
                linq = linq.Where(m => m.Fixture_Unique_ID.Contains(searchVM.Fixture_Unique_ID));
            }
            if (searchVM.Process_Info_UID != 0)
            {
                linq = linq.Where(m => m.Process_Info_UID.Equals(searchVM.Process_Info_UID));
            }
            if (searchVM.WorkStation_UID != 0)
            {
                linq = linq.Where(m => m.WorkStation_UID.Equals(searchVM.WorkStation_UID));
            }
            if (!string.IsNullOrWhiteSpace(searchVM.Equipment_No))
            {
                linq = linq.Where(m => m.Equipment_No.Contains(searchVM.Equipment_No));
            }
            if (!string.IsNullOrWhiteSpace(searchVM.Fixture_NO))
            {
                linq = linq.Where(m => m.Fixture_NO.Contains(searchVM.Fixture_NO));
            }
            if (searchVM.Status != 0)
            {
                linq = linq.Where(m => m.Status.Equals(searchVM.Status));
                if (searchVM.Status == 555)
                {
                    linq = linq.Where(m => m.Source_NO.Contains("OP_"));
                }
            }
            if (!string.IsNullOrWhiteSpace(searchVM.Version))
            {
                linq = linq.Where(m => m.Version.Contains(searchVM.Version));
            }
            if (!string.IsNullOrWhiteSpace(searchVM.Vendor_Name))
            {
                linq = linq.Where(m => m.Vendor_Name.Contains(searchVM.Vendor_Name));
            }
            if (!string.IsNullOrWhiteSpace(searchVM.Source_NO))
            {
                linq = linq.Where(m => m.Source_NO.Contains(searchVM.Source_NO));
            }
            if (searchVM.Production_Line_UID != 0)
            {
                linq = linq.Where(m => m.Production_Line_UID.Equals(searchVM.Production_Line_UID));
            }
            if (!string.IsNullOrWhiteSpace(searchVM.ShortCode))
            {
                linq = linq.Where(m => m.ShortCode.Contains(searchVM.ShortCode));
            }
            if (!string.IsNullOrWhiteSpace(searchVM.Fixture_Name))
            {
                linq = linq.Where(m => m.Fixture_Name.Contains(searchVM.Fixture_Name));
            }
            if (searchVM.End_Date_From != null)
                linq = linq.Where(m => m.Modified_Date >= searchVM.End_Date_From);
            if (searchVM.End_Date_To != null)
            {
                DateTime nextTime = searchVM.End_Date_To.Value.Date.AddDays(1);
                linq = linq.Where(m => m.Modified_Date < nextTime);
            }
            totalcount = linq.Count();
            var iquery = linq.OrderByDescending(m => m.Modified_Date).ThenBy(m => m.Fixture_NO).GetPage(page);
            var pagelist = new PagedListModel<FixtureResumeSearchVM>(totalcount, iquery);
            return pagelist;
        }
        public ViewResumeByUID QueryFixtureResumeByUID(int Fixture_Resume_UID, int Fixture_M_UID)
        {
            var resumeItem = DataContext.Fixture_Resume.FirstOrDefault(m => m.Fixture_Resume_UID == Fixture_Resume_UID);
            if (resumeItem == null)
            {
                var resumeHistoryItem = DataContext.Fixture_Resume_History.FirstOrDefault(m => m.Fixture_Resume_UID == Fixture_Resume_UID);
                resumeItem = AutoMapper.Mapper.Map<Fixture_Resume>(resumeHistoryItem);
            }
            var flag = resumeItem.Source_NO.Substring(0, 1);
            var item = ViewResumeSql(flag, resumeItem.Source_NO, Fixture_M_UID);
            //var item = DataContext.Database.SqlQuery<ViewResumeByUID>(sql).FirstOrDefault();
            var sql2 = ViewResumeSqlByCustomer(Fixture_M_UID);
            var customer = DataContext.Database.SqlQuery<string>(sql2).First();
            item.Customer = customer;
            //获取解决对策和维修原因
            List<DefectRepairSolution> drList = new List<DefectRepairSolution>();
            var sql3 = ViewResumeSqlByRepair(item.Fixture_Repair_D_UID);
            drList = DataContext.Database.SqlQuery<DefectRepairSolution>(sql3).ToList();
            item.DRList = drList;
            return item;
        }
        private ViewResumeByUID ViewResumeSql(string flag, string sourceNo, int Fixture_M_UID)
        {
            ViewResumeByUID item = new ViewResumeByUID();
            string sql = string.Empty;
            if (flag == "M")
            {
                sql = @"
                        SELECT A.Fixture_NO,B.Maintenance_Person_Number,B.Maintenance_Person_Name,
                        B.Maintenance_Date,N.User_Name AS ConfirmorName,B.Confirm_Date,
                        D.Cycle_ID,D.Cycle_Interval, D.Cycle_Unit,
                        CASE Maintenance_Type 
                        WHEN 'D' THEN N'日常保养' 
                        WHEN 'P' THEN N'周期保养' 
                        END AS CycleValue,
                        O.User_Name AS UpdateName,
                        B.Modified_Date
                        FROM dbo.Fixture_M A
                        JOIN (SELECT* from dbo.Fixture_Maintenance_Record UNION  
						     SELECT * from dbo.Fixture_Maintenance_Record_History) B
                        ON B.Fixture_M_UID = A.Fixture_M_UID
                        JOIN dbo.Fixture_Maintenance_Profile C
                        ON C.Fixture_Maintenance_Profile_UID = B.Fixture_Maintenance_Profile_UID
                        JOIN dbo.Maintenance_Plan D
                        ON D.Maintenance_Plan_UID = C.Maintenance_Plan_UID
                        LEFT JOIN dbo.System_Users N
                        ON N.Account_UID = B.Confirmor_UID
                        JOIN dbo.System_Users O
                        ON O.Account_UID = B.Modified_UID
                        WHERE B.Maintenance_Record_NO = N'{0}' AND B.Fixture_M_UID = {1}
                        ";
                sql = string.Format(sql, sourceNo, Fixture_M_UID);
                item = DataContext.Database.SqlQuery<ViewResumeByUID>(sql).FirstOrDefault();
            }
            else if (flag == "B")
            {
                sql = @"SELECT A.Shiper_UID,A.Totake_Number,A.Totake_Name AS TotakeName,A.Ship_Date AS Totake_Date
                        FROM dbo.Fixture_Totake_M A
                        JOIN dbo.System_Users F
                        ON F.Account_UID = A.Created_UID
                        WHERE A.Totake_NO = '{0}'";
                sql = string.Format(sql, sourceNo);
                item = DataContext.Database.SqlQuery<ViewResumeByUID>(sql).FirstOrDefault();
                string sql2 = @"
                                SELECT TOP 1 A.Fixture_Repair_M_UID,C.User_Name AS SendName, A.Created_Date AS SendDate, B.Status, B.Fixture_Repair_D_UID  
                                FROM dbo.Fixture_Repair_M A
                                JOIN dbo.Fixture_Repair_D B
                                ON B.Fixture_Repair_M_UID = A.Fixture_Repair_M_UID
                                JOIN dbo.System_Users C
                                ON C.Account_UID = A.Created_UID
                                WHERE B.Fixture_M_UID = {0}
                                ORDER BY B.Modified_Date DESC";
                sql2 = string.Format(sql2, Fixture_M_UID);
                var sql2Item = DataContext.Database.SqlQuery<ViewResumeByUID>(sql2).FirstOrDefault();
                if (sql2Item != null)
                {
                    item.SendDate = sql2Item.SendDate;
                    item.SendName = sql2Item.SendName;
                    item.SentOut_UID = sql2Item.SentOut_UID;
                    item.Status = sql2Item.Status;
                    item.Fixture_Repair_D_UID = sql2Item.Fixture_Repair_D_UID;
                    switch (item.Status)
                    {
                        case StructConstants.FixtureStatus.StatusOne:
                            item.StatusName = "使用中In-PRD";
                            break;
                        case StructConstants.FixtureStatus.StatusTwo:
                            item.StatusName = "未使用Non-PRD";
                            break;
                        case StructConstants.FixtureStatus.StatusThree:
                            item.StatusName = "维修中In-Repair";
                            break;
                        case StructConstants.FixtureStatus.StatusFour:
                            item.StatusName = "报废Scrap";
                            break;
                        case StructConstants.FixtureStatus.StatusFive:
                            item.StatusName = "返供应商维修RTV";
                            break;
                        case StructConstants.FixtureStatus.StatusSix:
                            item.StatusName = "保养逾时Over-Due Maintenance";
                            break;
                    }
                }
            }
            else if (flag == "R")
            {
                sql = @"
                        SELECT  A.SentOut_Number, A.SentOut_Name AS SendName, A.Created_Date AS SendDate,B.Fixture_Repair_D_UID,B.Fixture_M_UID
                        FROM dbo.Fixture_Repair_M A
                        JOIN dbo.Fixture_Repair_D B
                        ON B.Fixture_Repair_M_UID = A.Fixture_Repair_M_UID
                        JOIN dbo.Fixture_Repair_D_Defect C
                        ON C.Fixture_Repair_D_UID = B.Fixture_Repair_D_UID
                        WHERE Repair_NO = '{0}' ";
                sql = string.Format(sql, sourceNo);
                item = DataContext.Database.SqlQuery<ViewResumeByUID>(sql).FirstOrDefault();
            }
            return item;
        }
        private string ViewResumeSqlByCustomer(int Fixture_M_UID)
        {
            string sql = string.Empty;
            sql = @";
                        WITH one AS
                        (
                        SELECT B.BU_D_UID, B.BU_D_Name FROM dbo.System_BU_M A
                        JOIN dbo.System_BU_D B
                        ON B.BU_M_UID = A.BU_M_UID
                        ),
                        two AS 
                        (
                        SELECT one.BU_D_Name FROM dbo.Fixture_M A
                        JOIN dbo.System_Project B
                        ON B.Project_UID = A.Project_UID
                        JOIN one
                        ON one.BU_D_UID = B.BU_D_UID
                        WHERE A.Fixture_M_UID = {0}
                        )
                        SELECT * FROM two
                        ";
            sql = string.Format(sql, Fixture_M_UID);
            return sql;
        }
        private string ViewResumeSqlByRepair(int Fixture_Repair_D_UID)
        {
            string sql = string.Empty;
            sql = @"
                    SELECT B.DefectCode_Name,C.RepairSolution_Name 
                    FROM dbo.Fixture_Repair_D_Defect A
                    JOIN dbo.Fixture_DefectCode B
                    ON B.Fixture_Defect_UID = A.Defect_Code_UID
                    JOIN dbo.Fixture_RepairSolution C
                    ON C.Fixture_RepairSolution_UID = A.Solution_UID
                    WHERE Fixture_Repair_D_UID = {0}";
            sql = string.Format(sql, Fixture_Repair_D_UID);
            return sql;
        }

        public List<FixtureResumeSearchVM> DoAllExportFixtureResumeReprot(FixtureResumeSearchVM searchVM)
        {
            var linq =
                           from A in DataContext.Fixture_Resume
                           join B in DataContext.Fixture_M
                           on A.Fixture_M_UID equals B.Fixture_M_UID

                           join C in DataContext.Production_Line
                           on B.Production_Line_UID equals C.Production_Line_UID into BCTemp
                           from BC in BCTemp.DefaultIfEmpty()

                           join D in DataContext.Process_Info
                           on BC.Process_Info_UID equals D.Process_Info_UID
                           join E in DataContext.WorkStation
                           on BC.Workstation_UID equals E.WorkStation_UID
                           join F in DataContext.Fixture_Machine
                           on B.Fixture_Machine_UID equals F.Fixture_Machine_UID into BFTemp
                           from BF in BFTemp.DefaultIfEmpty()
                           join G in DataContext.Vendor_Info
                           on B.Vendor_Info_UID equals G.Vendor_Info_UID into BGTemp
                           from BG in BGTemp.DefaultIfEmpty()
                           join H in DataContext.System_Organization
                           on B.Plant_Organization_UID equals H.Organization_UID
                           join I in DataContext.System_Organization
                           on B.BG_Organization_UID equals I.Organization_UID
                           join J in DataContext.System_Organization
                           on B.FunPlant_Organization_UID equals J.Organization_UID into BJTemp
                           from BJ in BJTemp.DefaultIfEmpty()
                           join M in DataContext.System_Users
                           on A.Modified_UID equals M.Account_UID
                           select new FixtureResumeSearchVM
                           {
                               Plant_Organization_UID = H.Organization_UID,
                               BG_Organization_UID = I.Organization_UID,
                               FunPlant_Organization_UID = BJ.Organization_UID,
                               PlantName = H.Organization_Name,
                               OpType_Name = I.Organization_Name,
                               Func_Name = BJ.Organization_Name,
                               Fixture_M_UID = B.Fixture_M_UID,
                               Fixture_Unique_ID = B.Fixture_Unique_ID,
                               Fixture_NO = B.Fixture_NO,
                               Version = B.Version,
                               Status = B.Status,
                               Resume_Notes = A.Resume_Notes,
                               Fixture_Name = B.Fixture_Name,
                               ShortCode = B.ShortCode,
                               Process_Info_UID = D.Process_Info_UID,
                               Process_Name = D.Process_Name,
                               WorkStation_UID = E.WorkStation_UID,
                               WorkStation_Name = E.WorkStation_Name,
                               Equipment_No = BF.Equipment_No,
                               Vendor_Name = BG.Vendor_Name,
                               Source_NO = A.Source_NO,
                               Production_Line_UID = BC.Production_Line_UID,
                               Line_Name = BC.Line_Name,
                               Modified_UID = A.Modified_UID,
                               User_Name = M.User_Name,
                               Modified_Date = A.Modified_Date,
                               Fixture_Resume_UID = A.Fixture_Resume_UID
                           };
            if (searchVM.End_Date_From != null || searchVM.End_Date_To != null)
            {
                linq =
                        (from A in DataContext.Fixture_Resume
                         join B in DataContext.Fixture_M
                         on A.Fixture_M_UID equals B.Fixture_M_UID

                         join C in DataContext.Production_Line
                         on B.Production_Line_UID equals C.Production_Line_UID into BCTemp
                         from BC in BCTemp.DefaultIfEmpty()

                         join D in DataContext.Process_Info
                         on BC.Process_Info_UID equals D.Process_Info_UID
                         join E in DataContext.WorkStation
                         on BC.Workstation_UID equals E.WorkStation_UID
                         join F in DataContext.Fixture_Machine
                         on B.Fixture_Machine_UID equals F.Fixture_Machine_UID into BFTemp
                         from BF in BFTemp.DefaultIfEmpty()
                         join G in DataContext.Vendor_Info
                         on B.Vendor_Info_UID equals G.Vendor_Info_UID into BGTemp
                         from BG in BGTemp.DefaultIfEmpty()
                         join H in DataContext.System_Organization
                         on B.Plant_Organization_UID equals H.Organization_UID
                         join I in DataContext.System_Organization
                         on B.BG_Organization_UID equals I.Organization_UID
                         join J in DataContext.System_Organization
                         on B.FunPlant_Organization_UID equals J.Organization_UID into BJTemp
                         from BJ in BJTemp.DefaultIfEmpty()
                         join M in DataContext.System_Users
                         on A.Modified_UID equals M.Account_UID

                         select new FixtureResumeSearchVM
                         {
                             Plant_Organization_UID = H.Organization_UID,
                             BG_Organization_UID = I.Organization_UID,
                             FunPlant_Organization_UID = BJ.Organization_UID,
                             PlantName = H.Organization_Name,
                             OpType_Name = I.Organization_Name,
                             Func_Name = BJ.Organization_Name,
                             Fixture_M_UID = B.Fixture_M_UID,
                             Fixture_Unique_ID = B.Fixture_Unique_ID,
                             Fixture_NO = B.Fixture_NO,
                             Version = B.Version,
                             Status = B.Status,
                             Resume_Notes = A.Resume_Notes,
                             Fixture_Name = B.Fixture_Name,
                             ShortCode = B.ShortCode,
                             Process_Info_UID = D.Process_Info_UID,
                             Process_Name = D.Process_Name,
                             WorkStation_UID = E.WorkStation_UID,
                             WorkStation_Name = E.WorkStation_Name,
                             Equipment_No = BF.Equipment_No,
                             Vendor_Name = BG.Vendor_Name,
                             Source_NO = A.Source_NO,
                             Production_Line_UID = BC.Production_Line_UID,
                             Line_Name = BC.Line_Name,
                             Modified_UID = A.Modified_UID,
                             User_Name = M.User_Name,
                             Modified_Date = A.Modified_Date,
                             Fixture_Resume_UID = A.Fixture_Resume_UID
                         }).Union(


                            from A in DataContext.Fixture_Resume_History
                            join B in DataContext.Fixture_M
                            on A.Fixture_M_UID equals B.Fixture_M_UID

                            join C in DataContext.Production_Line
                            on B.Production_Line_UID equals C.Production_Line_UID into BCTemp
                            from BC in BCTemp.DefaultIfEmpty()

                            join D in DataContext.Process_Info
                            on BC.Process_Info_UID equals D.Process_Info_UID
                            join E in DataContext.WorkStation
                            on BC.Workstation_UID equals E.WorkStation_UID
                            join F in DataContext.Fixture_Machine
                            on B.Fixture_Machine_UID equals F.Fixture_Machine_UID into BFTemp
                            from BF in BFTemp.DefaultIfEmpty()
                            join G in DataContext.Vendor_Info
                            on B.Vendor_Info_UID equals G.Vendor_Info_UID into BGTemp
                            from BG in BGTemp.DefaultIfEmpty()
                            join H in DataContext.System_Organization
                            on B.Plant_Organization_UID equals H.Organization_UID
                            join I in DataContext.System_Organization
                            on B.BG_Organization_UID equals I.Organization_UID
                            join J in DataContext.System_Organization
                            on B.FunPlant_Organization_UID equals J.Organization_UID into BJTemp
                            from BJ in BJTemp.DefaultIfEmpty()
                            join M in DataContext.System_Users
                            on A.Modified_UID equals M.Account_UID
                            select new FixtureResumeSearchVM
                            {
                                Plant_Organization_UID = H.Organization_UID,
                                BG_Organization_UID = I.Organization_UID,
                                FunPlant_Organization_UID = BJ.Organization_UID,
                                PlantName = H.Organization_Name,
                                OpType_Name = I.Organization_Name,
                                Func_Name = BJ.Organization_Name,
                                Fixture_M_UID = B.Fixture_M_UID,
                                Fixture_Unique_ID = B.Fixture_Unique_ID,
                                Fixture_NO = B.Fixture_NO,
                                Version = B.Version,
                                Status = B.Status,
                                Resume_Notes = A.Resume_Notes,
                                Fixture_Name = B.Fixture_Name,
                                ShortCode = B.ShortCode,
                                Process_Info_UID = D.Process_Info_UID,
                                Process_Name = D.Process_Name,
                                WorkStation_UID = E.WorkStation_UID,
                                WorkStation_Name = E.WorkStation_Name,
                                Equipment_No = BF.Equipment_No,
                                Vendor_Name = BG.Vendor_Name,
                                Source_NO = A.Source_NO,
                                Production_Line_UID = BC.Production_Line_UID,
                                Line_Name = BC.Line_Name,
                                Modified_UID = A.Modified_UID,
                                User_Name = M.User_Name,
                                Modified_Date = A.Modified_Date,
                                Fixture_Resume_UID = A.Fixture_Resume_UID
                            }
             );
            }
            if (searchVM.Plant_Organization_UID != 0)
            {
                linq = linq.Where(m => m.Plant_Organization_UID.Equals(searchVM.Plant_Organization_UID));
            }
            if (searchVM.BG_Organization_UID != 0)
            {
                linq = linq.Where(m => m.BG_Organization_UID.Equals(searchVM.BG_Organization_UID));
            }
            if (searchVM.FunPlant_Organization_UID != null && searchVM.FunPlant_Organization_UID != 0)
            {
                linq = linq.Where(m => m.FunPlant_Organization_UID.Value.Equals(searchVM.FunPlant_Organization_UID.Value));
            }
            if (!string.IsNullOrWhiteSpace(searchVM.Fixture_Unique_ID))
            {
                linq = linq.Where(m => m.Fixture_Unique_ID.Contains(searchVM.Fixture_Unique_ID));
            }
            if (searchVM.Process_Info_UID != 0)
            {
                linq = linq.Where(m => m.Process_Info_UID.Equals(searchVM.Process_Info_UID));
            }
            if (searchVM.WorkStation_UID != 0)
            {
                linq = linq.Where(m => m.WorkStation_UID.Equals(searchVM.WorkStation_UID));
            }
            if (!string.IsNullOrWhiteSpace(searchVM.Equipment_No))
            {
                linq = linq.Where(m => m.Equipment_No.Contains(searchVM.Equipment_No));
            }
            if (!string.IsNullOrWhiteSpace(searchVM.Fixture_NO))
            {
                linq = linq.Where(m => m.Fixture_NO.Contains(searchVM.Fixture_NO));
            }
            if (searchVM.Status != 0)
            {
                linq = linq.Where(m => m.Status.Equals(searchVM.Status));
                if (searchVM.Status == 555)
                {
                    linq = linq.Where(m => m.Source_NO.Contains("OP_"));
                }
            }
            if (!string.IsNullOrWhiteSpace(searchVM.Version))
            {
                linq = linq.Where(m => m.Version.Contains(searchVM.Version));
            }
            if (!string.IsNullOrWhiteSpace(searchVM.Vendor_Name))
            {
                linq = linq.Where(m => m.Vendor_Name.Contains(searchVM.Vendor_Name));
            }
            if (!string.IsNullOrWhiteSpace(searchVM.Source_NO))
            {
                linq = linq.Where(m => m.Source_NO.Contains(searchVM.Source_NO));
            }
            if (searchVM.Production_Line_UID != 0)
            {
                linq = linq.Where(m => m.Production_Line_UID.Equals(searchVM.Production_Line_UID));
            }
            if (!string.IsNullOrWhiteSpace(searchVM.ShortCode))
            {
                linq = linq.Where(m => m.ShortCode.Contains(searchVM.ShortCode));
            }
            if (!string.IsNullOrWhiteSpace(searchVM.Fixture_Name))
            {
                linq = linq.Where(m => m.Fixture_Name.Contains(searchVM.Fixture_Name));
            }
            if (searchVM.End_Date_From != null)
                linq = linq.Where(m => m.Modified_Date >= searchVM.End_Date_From);
            if (searchVM.End_Date_To != null)
            {
                DateTime nextTime = searchVM.End_Date_To.Value.Date.AddDays(1);
                linq = linq.Where(m => m.Modified_Date < nextTime);
            }
            linq = linq.Take(10000);
            var iquery = linq.OrderByDescending(m => m.Modified_Date).ThenBy(m => m.Fixture_NO);
            return iquery.ToList();
        }
        public List<FixtureResumeSearchVM> ExportFixtureResumeByUID(string uids)
        {
            //var linq =
            //from A in DataContext.Fixture_Resume
            //join B in DataContext.Fixture_M
            //on A.Fixture_M_UID equals B.Fixture_M_UID
            //join C in DataContext.Production_Line
            //on B.Production_Line_UID equals C.Production_Line_UID into BCTemp
            //from BC in BCTemp.DefaultIfEmpty()
            //join D in DataContext.Process_Info
            //on BC.Process_Info_UID equals D.Process_Info_UID
            //join E in DataContext.WorkStation
            //on BC.Workstation_UID equals E.WorkStation_UID
            //join F in DataContext.Fixture_Machine
            //on B.Fixture_Machine_UID equals F.Fixture_Machine_UID into BFTemp
            //from BF in BFTemp.DefaultIfEmpty()
            //join G in DataContext.Vendor_Info
            //on B.Vendor_Info_UID equals G.Vendor_Info_UID into BGTemp
            //from BG in BGTemp.DefaultIfEmpty()
            //join H in DataContext.System_Organization
            //on B.Plant_Organization_UID equals H.Organization_UID
            //join I in DataContext.System_Organization
            //on B.BG_Organization_UID equals I.Organization_UID
            //join J in DataContext.System_Organization
            //on B.FunPlant_Organization_UID equals J.Organization_UID into BJTemp
            //from BJ in BJTemp.DefaultIfEmpty()
            //join M in DataContext.System_Users
            //on A.Modified_UID equals M.Account_UID
            //select new FixtureResumeSearchVM
            //{
            //    Plant_Organization_UID = H.Organization_UID,
            //    BG_Organization_UID = I.Organization_UID,
            //    FunPlant_Organization_UID = BJ.Organization_UID,
            //    PlantName = H.Organization_Name,
            //    OpType_Name = I.Organization_Name,
            //    Func_Name = BJ.Organization_Name,
            //    Fixture_M_UID = B.Fixture_M_UID,
            //    Fixture_Unique_ID = B.Fixture_Unique_ID,
            //    Fixture_NO = B.Fixture_NO,
            //    Version = B.Version,
            //    Status = B.Status,
            //    Resume_Notes = A.Resume_Notes,
            //    Fixture_Name = B.Fixture_Name,
            //    ShortCode = B.ShortCode,
            //    Process_Name = D.Process_Name,
            //    WorkStation_Name = E.WorkStation_Name,
            //    Equipment_No = BF.Equipment_No,
            //    Vendor_Name = BG.Vendor_Name,
            //    Source_NO = A.Source_NO,
            //    Line_Name = BC.Line_Name,
            //    Modified_UID = A.Modified_UID,
            //    User_Name = M.User_Name,
            //    Modified_Date = A.Modified_Date,
            //    Fixture_Resume_UID = A.Fixture_Resume_UID
            //};
            var linq =
             (from A in DataContext.Fixture_Resume
              join B in DataContext.Fixture_M
              on A.Fixture_M_UID equals B.Fixture_M_UID

              join C in DataContext.Production_Line
              on B.Production_Line_UID equals C.Production_Line_UID into BCTemp
              from BC in BCTemp.DefaultIfEmpty()

              join D in DataContext.Process_Info
              on BC.Process_Info_UID equals D.Process_Info_UID
              join E in DataContext.WorkStation
              on BC.Workstation_UID equals E.WorkStation_UID
              join F in DataContext.Fixture_Machine
              on B.Fixture_Machine_UID equals F.Fixture_Machine_UID into BFTemp
              from BF in BFTemp.DefaultIfEmpty()
              join G in DataContext.Vendor_Info
              on B.Vendor_Info_UID equals G.Vendor_Info_UID into BGTemp
              from BG in BGTemp.DefaultIfEmpty()
              join H in DataContext.System_Organization
              on B.Plant_Organization_UID equals H.Organization_UID
              join I in DataContext.System_Organization
              on B.BG_Organization_UID equals I.Organization_UID
              join J in DataContext.System_Organization
              on B.FunPlant_Organization_UID equals J.Organization_UID into BJTemp
              from BJ in BJTemp.DefaultIfEmpty()
              join M in DataContext.System_Users
              on A.Modified_UID equals M.Account_UID

              select new FixtureResumeSearchVM
              {
                  Plant_Organization_UID = H.Organization_UID,
                  BG_Organization_UID = I.Organization_UID,
                  FunPlant_Organization_UID = BJ.Organization_UID,
                  PlantName = H.Organization_Name,
                  OpType_Name = I.Organization_Name,
                  Func_Name = BJ.Organization_Name,
                  Fixture_M_UID = B.Fixture_M_UID,
                  Fixture_Unique_ID = B.Fixture_Unique_ID,
                  Fixture_NO = B.Fixture_NO,
                  Version = B.Version,
                  Status = B.Status,
                  Resume_Notes = A.Resume_Notes,
                  Fixture_Name = B.Fixture_Name,
                  ShortCode = B.ShortCode,
                  Process_Info_UID = D.Process_Info_UID,
                  Process_Name = D.Process_Name,
                  WorkStation_UID = E.WorkStation_UID,
                  WorkStation_Name = E.WorkStation_Name,
                  Equipment_No = BF.Equipment_No,
                  Vendor_Name = BG.Vendor_Name,
                  Source_NO = A.Source_NO,
                  Production_Line_UID = BC.Production_Line_UID,
                  Line_Name = BC.Line_Name,
                  Modified_UID = A.Modified_UID,
                  User_Name = M.User_Name,
                  Modified_Date = A.Modified_Date,
                  Fixture_Resume_UID = A.Fixture_Resume_UID
              }).Union(
                 from A in DataContext.Fixture_Resume_History
                 join B in DataContext.Fixture_M
                 on A.Fixture_M_UID equals B.Fixture_M_UID
                 join C in DataContext.Production_Line
                 on B.Production_Line_UID equals C.Production_Line_UID into BCTemp
                 from BC in BCTemp.DefaultIfEmpty()
                 join D in DataContext.Process_Info
                 on BC.Process_Info_UID equals D.Process_Info_UID
                 join E in DataContext.WorkStation
                 on BC.Workstation_UID equals E.WorkStation_UID
                 join F in DataContext.Fixture_Machine
                 on B.Fixture_Machine_UID equals F.Fixture_Machine_UID into BFTemp
                 from BF in BFTemp.DefaultIfEmpty()
                 join G in DataContext.Vendor_Info
                 on B.Vendor_Info_UID equals G.Vendor_Info_UID into BGTemp
                 from BG in BGTemp.DefaultIfEmpty()
                 join H in DataContext.System_Organization
                 on B.Plant_Organization_UID equals H.Organization_UID
                 join I in DataContext.System_Organization
                 on B.BG_Organization_UID equals I.Organization_UID
                 join J in DataContext.System_Organization
                 on B.FunPlant_Organization_UID equals J.Organization_UID into BJTemp
                 from BJ in BJTemp.DefaultIfEmpty()
                 join M in DataContext.System_Users
                 on A.Modified_UID equals M.Account_UID
                 select new FixtureResumeSearchVM
                 {
                     Plant_Organization_UID = H.Organization_UID,
                     BG_Organization_UID = I.Organization_UID,
                     FunPlant_Organization_UID = BJ.Organization_UID,
                     PlantName = H.Organization_Name,
                     OpType_Name = I.Organization_Name,
                     Func_Name = BJ.Organization_Name,
                     Fixture_M_UID = B.Fixture_M_UID,
                     Fixture_Unique_ID = B.Fixture_Unique_ID,
                     Fixture_NO = B.Fixture_NO,
                     Version = B.Version,
                     Status = B.Status,
                     Resume_Notes = A.Resume_Notes,
                     Fixture_Name = B.Fixture_Name,
                     ShortCode = B.ShortCode,
                     Process_Info_UID = D.Process_Info_UID,
                     Process_Name = D.Process_Name,
                     WorkStation_UID = E.WorkStation_UID,
                     WorkStation_Name = E.WorkStation_Name,
                     Equipment_No = BF.Equipment_No,
                     Vendor_Name = BG.Vendor_Name,
                     Source_NO = A.Source_NO,
                     Production_Line_UID = BC.Production_Line_UID,
                     Line_Name = BC.Line_Name,
                     Modified_UID = A.Modified_UID,
                     User_Name = M.User_Name,
                     Modified_Date = A.Modified_Date,
                     Fixture_Resume_UID = A.Fixture_Resume_UID
                 }
              );
            var strIdList = uids.Split(',').ToList();
            var intIdList = strIdList.Select<string, int>(x => Convert.ToInt32(x));
            var list = linq.Where(m => intIdList.Contains(m.Fixture_Resume_UID)).ToList();
            return list;
        }
        #endregion 治具履历查询-----------------------Add by Rock 2017-10-03 -----------end
        #region 国庆长假第七天_未保养治具查询 Add by Rock 2017-10-07-------------------------Start
        public Dictionary<string, string> GetMaintenanceStatus(string Maintenance_Type)
        {
            string sql = @"
                            SELECT (Maintenance_Type + '_' + Cycle_ID + '_' + CAST(Cycle_Interval AS NVARCHAR(5)) + '_' + Cycle_Unit) AS Cycle_UID, 
                            Cycle_ID + '-' + CAST(Cycle_Interval AS NVARCHAR(5)) + '' + Cycle_Unit + '-' +
                            CASE Maintenance_Type WHEN 'D' THEN N'日常保养' WHEN 'P' THEN N'周期保养' END AS CycleValue
                            FROM dbo.Maintenance_Plan {0}
                            
                            GROUP BY Maintenance_Type, Cycle_ID,Cycle_Interval,Cycle_Unit";
            if (string.IsNullOrEmpty(Maintenance_Type))
            {
                sql = string.Format(sql, string.Empty);
            }
            else
            {
                var whereVlue = string.Format(" WHERE Maintenance_Type = '{0}'", Maintenance_Type);
                sql = string.Format(sql, whereVlue);
            }

            var list = DataContext.Database.SqlQuery<MainTenanceStatus>(sql).ToList();

            Dictionary<string, string> dirList = new Dictionary<string, string>();
            foreach (var item in list)
            {
                dirList.Add(item.Cycle_UID, item.CycleValue);
            }
            return dirList;
        }
        public List<NotMaintenanceSearchVM> QueryFixtureNotMaintained(NotMaintenanceSearchVM model, Page page, out int totalcount)
        {
            List<string> timeList = new List<string>();
            while (model.QueryDate <= model.QueryDate_To)
            {
                timeList.Add(model.QueryDate.ToString("yyyyMMdd"));
                model.QueryDate = model.QueryDate.AddDays(1);
            }
            string dateStr = model.QueryDate.ToString("yyyyMMdd");
            string dataFormat = model.QueryDate.ToString(FormatConstants.DateTimeFormatStringByDate);

            var linq =
                    from A in DataContext.Fixture_Maintenance_Profile

                    join B in DataContext.Maintenance_Plan
                    on A.Maintenance_Plan_UID equals B.Maintenance_Plan_UID

                    join C in DataContext.Fixture_M
                    on A.Fixture_NO equals C.Fixture_NO

                    join D in DataContext.Production_Line
                    on C.Production_Line_UID equals D.Production_Line_UID into CDTemp
                    from CD in CDTemp.DefaultIfEmpty()

                    join E in DataContext.WorkStation
                    on CD.Workstation_UID equals E.WorkStation_UID

                    join F in DataContext.Process_Info
                    on CD.Process_Info_UID equals F.Process_Info_UID

                    join G in DataContext.Fixture_Machine
                    on C.Fixture_Machine_UID equals G.Fixture_Machine_UID

                    join H in DataContext.System_Organization
                    on A.Plant_Organization_UID equals H.Organization_UID

                    join I in DataContext.System_Organization
                    on A.BG_Organization_UID equals I.Organization_UID

                    join J in DataContext.System_Organization
                    on A.FunPlant_Organization_UID equals J.Organization_UID into BJTemp
                    from BJ in BJTemp.DefaultIfEmpty()

                    join K in DataContext.System_Project
                    on C.Project_UID equals K.Project_UID into KCTemp
                    from KC in KCTemp.DefaultIfEmpty()

                    join L in DataContext.System_BU_D
                    on KC.BU_D_UID equals L.BU_D_UID into LLTemp
                    from LL in LLTemp.DefaultIfEmpty()

                    join M in DataContext.Vendor_Info
                    on C.Vendor_Info_UID equals M.Vendor_Info_UID into MCTemp
                    from MC in MCTemp.DefaultIfEmpty()

                    join N in DataContext.Enumeration
                    on C.Status equals N.Enum_UID



                    where B.Maintenance_Type == "D"
                    select new NotMaintenanceSearchVM
                    {
                        Plant_Organization_UID = A.Plant_Organization_UID,
                        BG_Organization_UID = A.BG_Organization_UID,
                        FunPlant_Organization_UID = A.FunPlant_Organization_UID,
                        Fixture_Maintenance_Profile_UID = A.Fixture_Maintenance_Profile_UID,
                        PlantName = H.Organization_Name,
                        OpType_Name = I.Organization_Name,
                        Func_Name = BJ.Organization_Name,
                        BU_Name = LL.BU_D_Name,
                        Vendor_Name = MC.Vendor_Name,
                        StatusName = N.Enum_Value,
                        MaintenanceType = B.Maintenance_Type,
                        Cycle_ID = B.Cycle_ID,
                        Cycle_Interval = B.Cycle_Interval,
                        Cycle_Unit = B.Cycle_Unit,
                        Process_Info_UID = CD.Process_Info_UID,
                        Process_Name = F.Process_Name,
                        Machine_Type = G.Machine_Type,
                        WorkStation_UID = E.WorkStation_UID,
                        WorkStation_Name = E.WorkStation_Name,
                        Production_Line_UID = CD.Production_Line_UID,
                        Line_Name = CD.Line_Name,
                        Equipment_No = G.Equipment_No,
                        Fixture_M_UID = C.Fixture_M_UID,
                        Maintenance_Plan_UID = B.Maintenance_Plan_UID,
                        Fixture_Name = C.Fixture_Name,
                        Fixture_NO = C.Fixture_NO,
                        Version = C.Version,
                        ShortCode = C.ShortCode,
                        Fixture_Seq = C.Fixture_Seq,
                        Fixture_Unique_ID = C.Fixture_Unique_ID,
                        CycleValue = B.Cycle_ID + "_" + B.Cycle_Interval + B.Cycle_Unit + "_" + (B.Maintenance_Type == "D" ? "日常保养" : "周期保养"),
                        TwoD_Barcode = C.TwoD_Barcode,
                        DataFormat = dataFormat
                    };

            if (model.Plant_Organization_UID != 0)
            {
                linq = linq.Where(m => m.Plant_Organization_UID.Equals(model.Plant_Organization_UID));
            }
            if (model.BG_Organization_UID != 0)
            {
                linq = linq.Where(m => m.BG_Organization_UID.Equals(model.BG_Organization_UID));
            }
            if (model.FunPlant_Organization_UID != null && model.FunPlant_Organization_UID != 0)
            {
                linq = linq.Where(m => m.FunPlant_Organization_UID.Value.Equals(model.FunPlant_Organization_UID.Value));
            }
            if (!string.IsNullOrWhiteSpace(model.ProjectName))
            {
                linq = linq.Where(m => m.ProjectName.Contains(model.ProjectName));
            }
            if (model.Process_Info_UID != 0)
            {
                linq = linq.Where(m => m.Process_Info_UID.Equals(model.Process_Info_UID));
            }
            if (model.WorkStation_UID != 0)
            {
                linq = linq.Where(m => m.WorkStation_UID.Equals(model.WorkStation_UID));
            }
            if (model.Production_Line_UID != 0)
            {
                linq = linq.Where(m => m.Production_Line_UID.Equals(model.Production_Line_UID));
            }
            if (!string.IsNullOrEmpty(model.MaintenanceType))
            {
                var strList = model.MaintenanceType.Split('_').ToList();
                string Maintenance_Type = strList[0];
                string Cycle_ID = strList[1];
                int Cycle_Interval = Convert.ToInt32(strList[2]);
                string Cycle_Unit = strList[3];
                linq = linq.Where(m => m.MaintenanceType == Maintenance_Type && m.Cycle_ID == Cycle_ID
                && m.Cycle_Interval == Cycle_Interval && m.Cycle_Unit == Cycle_Unit);
            }


            var listOne = linq.ToList();

            var Profile_UIDList = listOne.Select(m => m.Fixture_Maintenance_Profile_UID).Distinct().ToList();

            //var listaa = DataContext.Fixture_Maintenance_Record.Where(m => Profile_UIDList.Contains(m.Fixture_Maintenance_Profile_UID)
            //&& m.Maintenance_Status == 1 && m.Maintenance_Record_NO.Contains(dateStr)).ToList();


            var listaa = DataContext.Fixture_Maintenance_Record.Where(m => Profile_UIDList.Contains(m.Fixture_Maintenance_Profile_UID)
            && m.Maintenance_Status == 1).ToList();

            List<Fixture_Maintenance_Record> recordList = new List<Fixture_Maintenance_Record>();
            foreach (var timeStr in timeList)
            {
                var aList = listaa.Where(m => m.Maintenance_Record_NO.Contains(timeStr)).ToList();
                recordList.AddRange(aList);
            }

            var idList = recordList.Select(m => m.Fixture_M_UID).Distinct().ToList();

            //var idList = listaa.Select(m => m.Fixture_M_UID).Distinct().ToList();


            //var allListId = listOne.Select(m => m.Fixture_M_UID).ToList().Except(idList).ToList();

            //var allList = listOne.Where(m => allListId.Contains(m.Fixture_M_UID)).ToList();

            var allList = listOne.Where(m => idList.Contains(m.Fixture_M_UID)).ToList();

            totalcount = allList.Count();
            return allList.Skip(page.Skip).Take(page.PageSize).ToList();
        }
        public List<NotMaintenanceSearchVM> ExportFixtureNotMaintainedByUID(string uids, string hidDate)
        {
            List<NotMaintenanceSearchVM> list = new List<NotMaintenanceSearchVM>();
            var idList = uids.Split(',').ToList();
            foreach (var idItem in idList)
            {
                var ids = idItem.Split('_').ToList();
                var Fixture_M_UID = Convert.ToInt32(ids[0]);
                var Maintenance_Plan_UID = Convert.ToInt32(ids[1]);

                var linq =
                    from C in DataContext.Fixture_M

                    join D in DataContext.Production_Line
                    on C.Production_Line_UID equals D.Production_Line_UID into CDTemp
                    from CD in CDTemp.DefaultIfEmpty()

                    join E in DataContext.WorkStation
                    on CD.Workstation_UID equals E.WorkStation_UID

                    join F in DataContext.Process_Info
                    on CD.Process_Info_UID equals F.Process_Info_UID

                    join G in DataContext.Fixture_Machine
                    on C.Fixture_Machine_UID equals G.Fixture_Machine_UID

                    join H in DataContext.System_Organization
                    on C.Plant_Organization_UID equals H.Organization_UID

                    join I in DataContext.System_Organization
                    on C.BG_Organization_UID equals I.Organization_UID

                    join J in DataContext.System_Organization
                    on C.FunPlant_Organization_UID equals J.Organization_UID into BJTemp
                    from BJ in BJTemp.DefaultIfEmpty()
                    where C.Fixture_M_UID == Fixture_M_UID
                    select new NotMaintenanceSearchVM
                    {
                        Plant_Organization_UID = C.Plant_Organization_UID,
                        BG_Organization_UID = C.BG_Organization_UID,
                        FunPlant_Organization_UID = C.FunPlant_Organization_UID,
                        PlantName = H.Organization_Name,
                        OpType_Name = I.Organization_Name,
                        Func_Name = BJ.Organization_Name,
                        Process_Info_UID = CD.Process_Info_UID,
                        Process_Name = F.Process_Name,
                        WorkStation_UID = E.WorkStation_UID,
                        WorkStation_Name = E.WorkStation_Name,
                        Production_Line_UID = CD.Production_Line_UID,
                        Line_Name = CD.Line_Name,
                        Equipment_No = G.Equipment_No,
                        Fixture_M_UID = C.Fixture_M_UID,
                        Fixture_Name = C.Fixture_Name,
                        Fixture_NO = C.Fixture_NO,
                        ShortCode = C.ShortCode,
                        DataFormat = hidDate
                    };
                NotMaintenanceSearchVM vmItem = new NotMaintenanceSearchVM();
                vmItem = linq.First();

                var linq2 = from B in DataContext.Maintenance_Plan
                            where B.Maintenance_Plan_UID == Maintenance_Plan_UID
                            select new NotMaintenanceSearchVM
                            {
                                Maintenance_Plan_UID = Maintenance_Plan_UID,
                                CycleValue = B.Cycle_ID + "_" + B.Cycle_Interval + B.Cycle_Unit + "_" + (B.Maintenance_Type == "D" ? "日常保养" : "周期保养")
                            };
                var linqItem2 = linq2.First();

                vmItem.Maintenance_Plan_UID = linqItem2.Maintenance_Plan_UID;
                vmItem.CycleValue = linqItem2.CycleValue;

                list.Add(vmItem);
            }
            return list;
        }

        public List<NotMaintenanceSearchVM> DoAllExportFixtureNotMaintainedReprot(NotMaintenanceSearchVM model)
        {

            string dateStr = model.QueryDate.ToString("yyyyMMdd");
            string dataFormat = model.QueryDate.ToString(FormatConstants.DateTimeFormatStringByDate);

            var linq =
                    from A in DataContext.Fixture_Maintenance_Profile

                    join B in DataContext.Maintenance_Plan
                    on A.Maintenance_Plan_UID equals B.Maintenance_Plan_UID

                    join C in DataContext.Fixture_M
                    on A.Fixture_NO equals C.Fixture_NO

                    join D in DataContext.Production_Line
                    on C.Production_Line_UID equals D.Production_Line_UID into CDTemp
                    from CD in CDTemp.DefaultIfEmpty()

                    join E in DataContext.WorkStation
                    on CD.Workstation_UID equals E.WorkStation_UID

                    join F in DataContext.Process_Info
                    on CD.Process_Info_UID equals F.Process_Info_UID

                    join G in DataContext.Fixture_Machine
                    on C.Fixture_Machine_UID equals G.Fixture_Machine_UID

                    join H in DataContext.System_Organization
                    on A.Plant_Organization_UID equals H.Organization_UID

                    join I in DataContext.System_Organization
                    on A.BG_Organization_UID equals I.Organization_UID

                    join J in DataContext.System_Organization
                    on A.FunPlant_Organization_UID equals J.Organization_UID into BJTemp
                    from BJ in BJTemp.DefaultIfEmpty()

                    join K in DataContext.System_Project
                    on C.Project_UID equals K.Project_UID into KCTemp
                    from KC in KCTemp.DefaultIfEmpty()

                    join L in DataContext.System_BU_D
                    on KC.BU_D_UID equals L.BU_D_UID into LLTemp
                    from LL in LLTemp.DefaultIfEmpty()

                    join M in DataContext.Vendor_Info
                    on C.Vendor_Info_UID equals M.Vendor_Info_UID into MCTemp
                    from MC in MCTemp.DefaultIfEmpty()

                    join N in DataContext.Enumeration
                    on C.Status equals N.Enum_UID



                    where B.Maintenance_Type == "D"
                    select new NotMaintenanceSearchVM
                    {
                        Plant_Organization_UID = A.Plant_Organization_UID,
                        BG_Organization_UID = A.BG_Organization_UID,
                        FunPlant_Organization_UID = A.FunPlant_Organization_UID,
                        Fixture_Maintenance_Profile_UID = A.Fixture_Maintenance_Profile_UID,
                        PlantName = H.Organization_Name,
                        OpType_Name = I.Organization_Name,
                        Func_Name = BJ.Organization_Name,
                        BU_Name = LL.BU_D_Name,
                        Vendor_Name = MC.Vendor_Name,
                        StatusName = N.Enum_Value,
                        MaintenanceType = B.Maintenance_Type,
                        Cycle_ID = B.Cycle_ID,
                        Cycle_Interval = B.Cycle_Interval,
                        Cycle_Unit = B.Cycle_Unit,
                        Process_Info_UID = CD.Process_Info_UID,
                        Process_Name = F.Process_Name,
                        Machine_Type = G.Machine_Type,
                        WorkStation_UID = E.WorkStation_UID,
                        WorkStation_Name = E.WorkStation_Name,
                        Production_Line_UID = CD.Production_Line_UID,
                        Line_Name = CD.Line_Name,
                        Equipment_No = G.Equipment_No,
                        Fixture_M_UID = C.Fixture_M_UID,
                        Maintenance_Plan_UID = B.Maintenance_Plan_UID,
                        Fixture_Name = C.Fixture_Name,
                        Fixture_NO = C.Fixture_NO,
                        Version = C.Version,
                        ShortCode = C.ShortCode,
                        Fixture_Seq = C.Fixture_Seq,
                        Fixture_Unique_ID = C.Fixture_Unique_ID,
                        CycleValue = B.Cycle_ID + "_" + B.Cycle_Interval + B.Cycle_Unit + "_" + (B.Maintenance_Type == "D" ? "日常保养" : "周期保养"),
                        TwoD_Barcode = C.TwoD_Barcode,
                        DataFormat = dataFormat
                    };

            if (model.Plant_Organization_UID != 0)
            {
                linq = linq.Where(m => m.Plant_Organization_UID.Equals(model.Plant_Organization_UID));
            }
            if (model.BG_Organization_UID != 0)
            {
                linq = linq.Where(m => m.BG_Organization_UID.Equals(model.BG_Organization_UID));
            }
            if (model.FunPlant_Organization_UID != null && model.FunPlant_Organization_UID != 0)
            {
                linq = linq.Where(m => m.FunPlant_Organization_UID.Value.Equals(model.FunPlant_Organization_UID.Value));
            }
            if (!string.IsNullOrWhiteSpace(model.ProjectName))
            {
                linq = linq.Where(m => m.ProjectName.Contains(model.ProjectName));
            }
            if (model.Process_Info_UID != 0)
            {
                linq = linq.Where(m => m.Process_Info_UID.Equals(model.Process_Info_UID));
            }
            if (model.WorkStation_UID != 0)
            {
                linq = linq.Where(m => m.WorkStation_UID.Equals(model.WorkStation_UID));
            }
            if (model.Production_Line_UID != 0)
            {
                linq = linq.Where(m => m.Production_Line_UID.Equals(model.Production_Line_UID));
            }
            if (!string.IsNullOrEmpty(model.MaintenanceType))
            {
                var strList = model.MaintenanceType.Split('_').ToList();
                string Maintenance_Type = strList[0];
                string Cycle_ID = strList[1];
                int Cycle_Interval = Convert.ToInt32(strList[2]);
                string Cycle_Unit = strList[3];
                linq = linq.Where(m => m.MaintenanceType == Maintenance_Type && m.Cycle_ID == Cycle_ID
                && m.Cycle_Interval == Cycle_Interval && m.Cycle_Unit == Cycle_Unit);
            }

            var listOne = linq.ToList();
            var Profile_UIDList = listOne.Select(m => m.Fixture_Maintenance_Profile_UID).Distinct().ToList();
            var listaa = DataContext.Fixture_Maintenance_Record.Where(m => Profile_UIDList.Contains(m.Fixture_Maintenance_Profile_UID)
            && m.Maintenance_Status == 1 && m.Maintenance_Record_NO.Contains(dateStr)).ToList();
            var idList = listaa.Select(m => m.Fixture_M_UID).Distinct().ToList();
            var allListId = listOne.Select(m => m.Fixture_M_UID).ToList().Except(idList).ToList();
            var allList = listOne.Where(m => allListId.Contains(m.Fixture_M_UID)).ToList();
            return allList;

        }

        #endregion 国庆长假第七天_未保养治具查询 Add by Rock 2017-10-07-------------------------End
        #region 治具维修次数查询 Add by Rock 2017-10-30-------------------------------Start
        public List<ReportByRepair> QueryReportByRepair(ReportByRepair model, Page page, out int totalcount)
        {
            var linq = from A in DataContext.Fixture_Repair_M
                       join B in DataContext.Fixture_Repair_D
                       on A.Fixture_Repair_M_UID equals B.Fixture_Repair_M_UID

                       join C in DataContext.Fixture_Repair_D_Defect
                       on B.Fixture_Repair_D_UID equals C.Fixture_Repair_D_UID

                       join D in DataContext.Fixture_DefectCode
                       on C.Defect_Code_UID equals D.Fixture_Defect_UID

                       join E in DataContext.Fixture_M
                       on B.Fixture_M_UID equals E.Fixture_M_UID

                       join F in DataContext.Production_Line
                       on E.Production_Line_UID equals F.Production_Line_UID into EFTemp
                       from EF in EFTemp.DefaultIfEmpty()

                       join G in DataContext.WorkStation
                       on EF.Workstation_UID equals G.WorkStation_UID

                       join H in DataContext.System_Organization
                       on A.Plant_Organization_UID equals H.Organization_UID

                       join I in DataContext.System_Organization
                       on A.BG_Organization_UID equals I.Organization_UID

                       join J in DataContext.System_Organization
                       on A.FunPlant_Organization_UID equals J.Organization_UID into BJTemp
                       from BJ in BJTemp.DefaultIfEmpty()

                       select new ReportByRepair
                       {
                           Plant_Organization_UID = H.Organization_UID,
                           BG_Organization_UID = I.Organization_UID,
                           FunPlant_Organization_UID = BJ.Organization_UID,
                           PlantName = H.Organization_Name,
                           OpType_Name = I.Organization_Name,
                           Func_Name = BJ.Organization_Name,
                           Fixture_M_UID = B.Fixture_M_UID,
                           Fixture_NO = E.Fixture_NO,
                           WorkStation_UID = G.WorkStation_UID,
                           WorkStation_Name = G.WorkStation_Name,
                           Fixture_Defect_UID = D.Fixture_Defect_UID,
                           DefectCode_ID = D.DefectCode_ID,
                           DefectCode_Name = D.DefectCode_Name,
                           SentOut_Date = DbFunctions.TruncateTime(A.SentOut_Date)
                       };

            if (model.Plant_Organization_UID != 0)
            {
                linq = linq.Where(m => m.Plant_Organization_UID.Equals(model.Plant_Organization_UID));
            }
            if (model.BG_Organization_UID != 0)
            {
                linq = linq.Where(m => m.BG_Organization_UID.Equals(model.BG_Organization_UID));
            }
            if (model.FunPlant_Organization_UID != null && model.FunPlant_Organization_UID != 0)
            {
                linq = linq.Where(m => m.FunPlant_Organization_UID.Value.Equals(model.FunPlant_Organization_UID.Value));
            }
            if (model.WorkStation_UID != 0)
            {
                linq = linq.Where(m => m.WorkStation_UID.Equals(model.WorkStation_UID));
            }

            linq = linq.Where(m => SqlFunctions.DateDiff("dd", m.SentOut_Date, model.StartDate) <= 0);
            linq = linq.Where(m => SqlFunctions.DateDiff("dd", m.SentOut_Date, model.EndDate) >= 0);

            var list = linq.OrderBy(m => m.Fixture_NO).ToList();

            List<ReportByRepair> repairList = new List<ReportByRepair>();
            if (model.QueryType == 1)
            {
                var g1 = list.GroupBy(m => new { m.PlantName, m.OpType_Name, m.Func_Name, m.Fixture_NO, m.DefectCode_ID, m.DefectCode_Name, m.WorkStation_Name }).ToList();
                foreach (var g1Item in g1)
                {
                    repairList.Add(new ReportByRepair
                    {
                        PlantName = g1Item.Key.PlantName,
                        OpType_Name = g1Item.Key.OpType_Name,
                        Func_Name = g1Item.Key.Func_Name,
                        Fixture_NO = g1Item.Key.Fixture_NO,
                        DefectCode_ID = g1Item.Key.DefectCode_ID,
                        DefectCode_Name = g1Item.Key.DefectCode_Name,
                        WorkStation_Name = g1Item.Key.WorkStation_Name,
                        TotalCount = g1Item.Count()
                    });
                }
                totalcount = repairList.Count();
                return repairList.Skip(page.Skip).Take(page.PageSize).ToList();
            }
            else
            {
                var g2 = list.GroupBy(m => new { m.PlantName, m.OpType_Name, m.Func_Name, m.Fixture_NO, m.WorkStation_Name, m.SentOut_Date }).ToList();
                foreach (var g2Item in g2)
                {
                    repairList.Add(new ReportByRepair
                    {
                        PlantName = g2Item.Key.PlantName,
                        OpType_Name = g2Item.Key.OpType_Name,
                        Func_Name = g2Item.Key.Func_Name,
                        Fixture_NO = g2Item.Key.Fixture_NO,
                        WorkStation_Name = g2Item.Key.WorkStation_Name,
                        SentOut_Date = g2Item.Key.SentOut_Date,
                        TotalCount = g2Item.Count()
                    });
                }
                totalcount = repairList.Count();
                return repairList.Skip(page.Skip).Take(page.PageSize).ToList();
            }
        }
        public List<ReportByRepair> ExportReportByRepair(ReportByRepair model)
        {
            var linq = from A in DataContext.Fixture_Repair_M
                       join B in DataContext.Fixture_Repair_D
                       on A.Fixture_Repair_M_UID equals B.Fixture_Repair_M_UID

                       join C in DataContext.Fixture_Repair_D_Defect
                       on B.Fixture_Repair_D_UID equals C.Fixture_Repair_D_UID

                       join D in DataContext.Fixture_DefectCode
                       on C.Defect_Code_UID equals D.Fixture_Defect_UID

                       join E in DataContext.Fixture_M
                       on B.Fixture_M_UID equals E.Fixture_M_UID

                       join F in DataContext.Production_Line
                       on E.Production_Line_UID equals F.Production_Line_UID into EFTemp
                       from EF in EFTemp.DefaultIfEmpty()

                       join G in DataContext.WorkStation
                       on EF.Workstation_UID equals G.WorkStation_UID

                       join H in DataContext.System_Organization
                       on A.Plant_Organization_UID equals H.Organization_UID

                       join I in DataContext.System_Organization
                       on A.BG_Organization_UID equals I.Organization_UID

                       join J in DataContext.System_Organization
                       on A.FunPlant_Organization_UID equals J.Organization_UID into BJTemp
                       from BJ in BJTemp.DefaultIfEmpty()

                       select new ReportByRepair
                       {
                           Plant_Organization_UID = H.Organization_UID,
                           BG_Organization_UID = I.Organization_UID,
                           FunPlant_Organization_UID = BJ.Organization_UID,
                           PlantName = H.Organization_Name,
                           OpType_Name = I.Organization_Name,
                           Func_Name = BJ.Organization_Name,
                           Fixture_M_UID = B.Fixture_M_UID,
                           Fixture_NO = E.Fixture_NO,
                           WorkStation_UID = G.WorkStation_UID,
                           WorkStation_Name = G.WorkStation_Name,
                           Fixture_Defect_UID = D.Fixture_Defect_UID,
                           DefectCode_ID = D.DefectCode_ID,
                           DefectCode_Name = D.DefectCode_Name,
                           SentOut_Date = DbFunctions.TruncateTime(A.SentOut_Date)
                       };

            if (model.Plant_Organization_UID != 0)
            {
                linq = linq.Where(m => m.Plant_Organization_UID.Equals(model.Plant_Organization_UID));
            }
            if (model.BG_Organization_UID != 0)
            {
                linq = linq.Where(m => m.BG_Organization_UID.Equals(model.BG_Organization_UID));
            }
            if (model.FunPlant_Organization_UID != null && model.FunPlant_Organization_UID != 0)
            {
                linq = linq.Where(m => m.FunPlant_Organization_UID.Value.Equals(model.FunPlant_Organization_UID.Value));
            }
            if (model.WorkStation_UID != 0)
            {
                linq = linq.Where(m => m.WorkStation_UID.Equals(model.WorkStation_UID));
            }

            linq = linq.Where(m => SqlFunctions.DateDiff("dd", m.SentOut_Date, model.StartDate) <= 0);
            linq = linq.Where(m => SqlFunctions.DateDiff("dd", m.SentOut_Date, model.EndDate) >= 0);

            var list = linq.OrderBy(m => m.Fixture_NO).ToList();

            List<ReportByRepair> repairList = new List<ReportByRepair>();
            //生成第一个model的数据
            var g1 = list.GroupBy(m => new { m.PlantName, m.OpType_Name, m.Func_Name, m.Fixture_NO, m.DefectCode_ID, m.DefectCode_Name, m.WorkStation_Name }).ToList();
            foreach (var g1Item in g1)
            {
                repairList.Add(new ReportByRepair
                {
                    PlantName = g1Item.Key.PlantName,
                    OpType_Name = g1Item.Key.OpType_Name,
                    Func_Name = g1Item.Key.Func_Name,
                    Fixture_NO = g1Item.Key.Fixture_NO,
                    DefectCode_ID = g1Item.Key.DefectCode_ID,
                    DefectCode_Name = g1Item.Key.DefectCode_Name,
                    WorkStation_Name = g1Item.Key.WorkStation_Name,
                    TotalCount = g1Item.Count(),
                    sheetCount = 1
                });
            }

            //生成第二个model的数据
            var g2 = list.GroupBy(m => new { m.PlantName, m.OpType_Name, m.Func_Name, m.Fixture_NO, m.WorkStation_Name, m.SentOut_Date }).ToList();
            foreach (var g2Item in g2)
            {
                repairList.Add(new ReportByRepair
                {
                    PlantName = g2Item.Key.PlantName,
                    OpType_Name = g2Item.Key.OpType_Name,
                    Func_Name = g2Item.Key.Func_Name,
                    Fixture_NO = g2Item.Key.Fixture_NO,
                    WorkStation_Name = g2Item.Key.WorkStation_Name,
                    SentOut_Date = g2Item.Key.SentOut_Date,
                    TotalCount = g2Item.Count(),
                    sheetCount = 2
                });
            }
            return repairList;
        }
        #endregion Add by Rock 2017-10-30-------------------------------end
        #region 报表-治具维修次查询(维修人) Add by Rock 2017-11-02------------------------ Start
        public List<ReportByRepair> QueryReportByRepairPerson(ReportByRepair model, Page page, out int totalcount, bool isExport)
        {
            var linq = from A in DataContext.Fixture_Repair_M
                       join B in DataContext.Fixture_Repair_D
                       on A.Fixture_Repair_M_UID equals B.Fixture_Repair_M_UID

                       join C in DataContext.Fixture_Repair_D_Defect
                       on B.Fixture_Repair_D_UID equals C.Fixture_Repair_D_UID

                       join D in DataContext.Fixture_DefectCode
                       on C.Defect_Code_UID equals D.Fixture_Defect_UID

                       join E in DataContext.Fixture_M
                       on B.Fixture_M_UID equals E.Fixture_M_UID

                       join F in DataContext.Production_Line
                       on E.Production_Line_UID equals F.Production_Line_UID into EFTemp
                       from EF in EFTemp.DefaultIfEmpty()

                       join G in DataContext.WorkStation
                       on EF.Workstation_UID equals G.WorkStation_UID

                       join BB in DataContext.System_Users
                       on B.Repair_Staff_UID equals BB.Account_UID

                       join H in DataContext.System_Organization
                       on A.Plant_Organization_UID equals H.Organization_UID

                       join I in DataContext.System_Organization
                       on A.BG_Organization_UID equals I.Organization_UID

                       join J in DataContext.System_Organization
                       on A.FunPlant_Organization_UID equals J.Organization_UID into BJTemp
                       from BJ in BJTemp.DefaultIfEmpty()

                       select new ReportByRepair
                       {
                           Plant_Organization_UID = H.Organization_UID,
                           BG_Organization_UID = I.Organization_UID,
                           FunPlant_Organization_UID = BJ.Organization_UID,
                           PlantName = H.Organization_Name,
                           OpType_Name = I.Organization_Name,
                           Func_Name = BJ.Organization_Name,
                           Fixture_M_UID = B.Fixture_M_UID,
                           Fixture_NO = E.Fixture_NO,
                           WorkStation_UID = G.WorkStation_UID,
                           WorkStation_Name = G.WorkStation_Name,
                           Fixture_Defect_UID = D.Fixture_Defect_UID,
                           DefectCode_ID = D.DefectCode_ID,
                           DefectCode_Name = D.DefectCode_Name,
                           Repair_Staff_UID = B.Repair_Staff_UID,
                           SentOut_Date = A.SentOut_Date,
                           RepairName = BB.User_Name
                       };

            if (model.Plant_Organization_UID != 0)
            {
                linq = linq.Where(m => m.Plant_Organization_UID.Equals(model.Plant_Organization_UID));
            }
            if (model.BG_Organization_UID != 0)
            {
                linq = linq.Where(m => m.BG_Organization_UID.Equals(model.BG_Organization_UID));
            }
            if (model.FunPlant_Organization_UID != null && model.FunPlant_Organization_UID != 0)
            {
                linq = linq.Where(m => m.FunPlant_Organization_UID.Value.Equals(model.FunPlant_Organization_UID.Value));
            }
            if (model.WorkStation_UID != 0)
            {
                linq = linq.Where(m => m.WorkStation_UID.Equals(model.WorkStation_UID));
            }

            linq = linq.Where(m => SqlFunctions.DateDiff("dd", m.SentOut_Date, model.StartDate) <= 0);
            linq = linq.Where(m => SqlFunctions.DateDiff("dd", m.SentOut_Date, model.EndDate) >= 0);

            var list = linq.ToList();

            List<ReportByRepair> repairList = new List<ReportByRepair>();

            var g1 = list.GroupBy(m => new { m.PlantName, m.OpType_Name, m.Func_Name, m.DefectCode_ID, m.DefectCode_Name, m.WorkStation_Name, m.RepairName }).ToList();
            foreach (var g1Item in g1)
            {
                repairList.Add(new ReportByRepair
                {
                    PlantName = g1Item.Key.PlantName,
                    OpType_Name = g1Item.Key.OpType_Name,
                    Func_Name = g1Item.Key.Func_Name,
                    DefectCode_ID = g1Item.Key.DefectCode_ID,
                    DefectCode_Name = g1Item.Key.DefectCode_Name,
                    WorkStation_Name = g1Item.Key.WorkStation_Name,
                    RepairName = g1Item.Key.RepairName,
                    TotalCount = g1Item.Count()
                });
            }
            totalcount = repairList.Count();
            if (isExport) //如果是导出数据则导出所有数据
            {
                return repairList;
            }
            else //如果是查询则按条件分页
            {
                return repairList.Skip(page.Skip).Take(page.PageSize).ToList();
            }

        }
        public List<ReportByRepair> ExportReportByRepairPersonValid(ReportByRepair model)
        {
            int count = 0;
            return QueryReportByRepairPerson(model, null, out count, true);
        }
        #endregion 报表-治具维修次查询(维修人) Add by Rock 2017-11-02------------------------ End

        #region 报表-日治具维修次数报表 Add by Rock 2017-11-06------------------------ Start
        public List<ReportByRepair> QueryReportByPage(ReportByRepair model, Page page, out int totalcount, bool isExport)
        {
            var linq = from A in DataContext.Fixture_Repair_M
                       join B in DataContext.Fixture_Repair_D
                       on A.Fixture_Repair_M_UID equals B.Fixture_Repair_M_UID

                       join E in DataContext.Fixture_M
                       on B.Fixture_M_UID equals E.Fixture_M_UID

                       join BB in DataContext.System_Users
                       on B.Repair_Staff_UID equals BB.Account_UID

                       join H in DataContext.System_Organization
                       on A.Plant_Organization_UID equals H.Organization_UID

                       join I in DataContext.System_Organization
                       on A.BG_Organization_UID equals I.Organization_UID

                       join J in DataContext.System_Organization
                       on A.FunPlant_Organization_UID equals J.Organization_UID into BJTemp
                       from BJ in BJTemp.DefaultIfEmpty()

                       select new ReportByRepair
                       {
                           Plant_Organization_UID = H.Organization_UID,
                           BG_Organization_UID = I.Organization_UID,
                           FunPlant_Organization_UID = BJ.Organization_UID,
                           PlantName = H.Organization_Name,
                           OpType_Name = I.Organization_Name,
                           Func_Name = BJ.Organization_Name,
                           Fixture_M_UID = B.Fixture_M_UID,
                           Fixture_NO = E.Fixture_NO,
                           Version = E.Version,
                           Repair_Staff_UID = B.Repair_Staff_UID,
                           SentOut_Date = A.SentOut_Date,
                           RepairName = BB.User_Name
                       };

            if (model.Plant_Organization_UID != 0)
            {
                linq = linq.Where(m => m.Plant_Organization_UID.Equals(model.Plant_Organization_UID));
            }
            if (model.BG_Organization_UID != 0)
            {
                linq = linq.Where(m => m.BG_Organization_UID.Equals(model.BG_Organization_UID));
            }
            if (model.FunPlant_Organization_UID != null && model.FunPlant_Organization_UID != 0)
            {
                linq = linq.Where(m => m.FunPlant_Organization_UID.Value.Equals(model.FunPlant_Organization_UID.Value));
            }


            linq = linq.Where(m => SqlFunctions.DateDiff("dd", m.SentOut_Date, model.StartDate) <= 0);
            linq = linq.Where(m => SqlFunctions.DateDiff("dd", m.SentOut_Date, model.EndDate) >= 0);

            if (model.DisplayCount != 0)
            {
                linq = linq.Take(model.DisplayCount);
            }

            var list = linq.ToList();

            List<ReportByRepair> repairList = new List<ReportByRepair>();

            var g1 = list.GroupBy(m => new { m.PlantName, m.OpType_Name, m.Func_Name, m.Fixture_NO, m.Version }).ToList();

            foreach (var g1Item in g1)
            {
                repairList.Add(new ReportByRepair
                {
                    PlantName = g1Item.Key.PlantName,
                    OpType_Name = g1Item.Key.OpType_Name,
                    Func_Name = g1Item.Key.Func_Name,
                    Fixture_NO = g1Item.Key.Fixture_NO,
                    Version = g1Item.Key.Version,
                    TotalCount = g1Item.Count()
                });
            }
            totalcount = repairList.Count();
            if (isExport) //如果是导出数据则导出所有数据
            {
                return repairList;
            }
            else //如果是查询则按条件分页
            {
                return repairList.Skip(page.Skip).Take(page.PageSize).ToList();
            }
        }
        public List<ReportByRepair> ExportReportByPageValid(ReportByRepair model)
        {
            int count = 0;
            return QueryReportByPage(model, null, out count, true);
        }
        #endregion 报表-日治具维修次数报表 Add by Rock 2017-11-06------------------------ End

        #region 报表-治具维修次数查询（明细） Add by Rock 2017-11-24------------------------ Start

        public List<ReportByRepair> QueryFixtureReportByDetail(ReportByRepair model, Page page, out int totalcount, bool isExport)
        {
            var linq = from A in DataContext.Fixture_Repair_M
                       join B in DataContext.Fixture_Repair_D
                       on A.Fixture_Repair_M_UID equals B.Fixture_Repair_M_UID

                       join C in DataContext.Fixture_Repair_D_Defect
                       on B.Fixture_Repair_D_UID equals C.Fixture_Repair_D_UID

                       join D in DataContext.Fixture_DefectCode
                       on C.Defect_Code_UID equals D.Fixture_Defect_UID

                       join E in DataContext.Fixture_M
                       on B.Fixture_M_UID equals E.Fixture_M_UID

                       join F in DataContext.Production_Line
                       on E.Production_Line_UID equals F.Production_Line_UID into EFTemp
                       from EF in EFTemp.DefaultIfEmpty()

                       join G in DataContext.WorkStation
                       on EF.Workstation_UID equals G.WorkStation_UID

                       join BB in DataContext.System_Users
                       on B.Repair_Staff_UID equals BB.Account_UID

                       join H in DataContext.System_Organization
                       on A.Plant_Organization_UID equals H.Organization_UID

                       join I in DataContext.System_Organization
                       on A.BG_Organization_UID equals I.Organization_UID

                       join J in DataContext.System_Organization
                       on A.FunPlant_Organization_UID equals J.Organization_UID into BJTemp
                       from BJ in BJTemp.DefaultIfEmpty()


                       select new ReportByRepair
                       {
                           Plant_Organization_UID = H.Organization_UID,
                           BG_Organization_UID = I.Organization_UID,
                           FunPlant_Organization_UID = BJ.Organization_UID,
                           PlantName = H.Organization_Name,
                           OpType_Name = I.Organization_Name,
                           Func_Name = BJ.Organization_Name,
                           WorkStation_UID = G.WorkStation_UID,
                           WorkStation_Name = G.WorkStation_Name,
                           WorkStation_Is_Enable = G.Is_Enable,
                           Production_Line_Is_Enable = EF.Is_Enable,
                           Fixture_M_UID = B.Fixture_M_UID,
                           Fixture_NO = E.Fixture_NO,
                           Fixture_Unique_ID = E.Fixture_Unique_ID,
                           Fixture_Defect_UID = D.Fixture_Defect_UID,
                           DefectCode_ID = D.DefectCode_ID,
                           DefectCode_Name = D.DefectCode_Name,
                           Repair_Staff_UID = B.Repair_Staff_UID,
                           RepairName = BB.User_Name,
                           SentOut_Date = DbFunctions.TruncateTime(A.SentOut_Date)

                       };

            linq = linq.Where(m => m.Production_Line_Is_Enable == true && m.WorkStation_Is_Enable == true);
            if (model.Plant_Organization_UID != 0)
            {
                linq = linq.Where(m => m.Plant_Organization_UID.Equals(model.Plant_Organization_UID));
            }
            if (model.BG_Organization_UID != 0)
            {
                linq = linq.Where(m => m.BG_Organization_UID.Equals(model.BG_Organization_UID));
            }
            if (model.FunPlant_Organization_UID != null && model.FunPlant_Organization_UID != 0)
            {
                linq = linq.Where(m => m.FunPlant_Organization_UID.Value.Equals(model.FunPlant_Organization_UID.Value));
            }
            if (model.WorkStation_UID != 0)
            {
                linq = linq.Where(m => m.WorkStation_UID.Equals(model.WorkStation_UID));
            }


            linq = linq.Where(m => SqlFunctions.DateDiff("dd", m.SentOut_Date, model.StartDate) <= 0);
            linq = linq.Where(m => SqlFunctions.DateDiff("dd", m.SentOut_Date, model.EndDate) >= 0);

            if (model.DisplayCount != 0)
            {
                linq = linq.Take(model.DisplayCount);
            }

            var list = linq.ToList();

            List<ReportByRepair> repairList = new List<ReportByRepair>();
            if (model.QueryType == 1)
            {
                var g1 = list.GroupBy(m => new { m.PlantName, m.OpType_Name, m.Func_Name, m.WorkStation_Name, m.Fixture_Unique_ID, m.RepairName }).ToList();
                foreach (var g1Item in g1)
                {
                    repairList.Add(new ReportByRepair
                    {
                        PlantName = g1Item.Key.PlantName,
                        OpType_Name = g1Item.Key.OpType_Name,
                        Func_Name = g1Item.Key.Func_Name,
                        WorkStation_Name = g1Item.Key.WorkStation_Name,
                        Fixture_Unique_ID = g1Item.Key.Fixture_Unique_ID,
                        RepairName = g1Item.Key.RepairName,
                        TotalCount = g1Item.Count()
                    });
                }
            }
            else
            {
                var g1 = list.GroupBy(m => new { m.PlantName, m.OpType_Name, m.Func_Name, m.WorkStation_Name, m.Fixture_Unique_ID, m.DefectCode_ID, m.DefectCode_Name }).ToList();
                foreach (var g1Item in g1)
                {
                    repairList.Add(new ReportByRepair
                    {
                        PlantName = g1Item.Key.PlantName,
                        OpType_Name = g1Item.Key.OpType_Name,
                        Func_Name = g1Item.Key.Func_Name,
                        WorkStation_Name = g1Item.Key.WorkStation_Name,
                        Fixture_Unique_ID = g1Item.Key.Fixture_Unique_ID,
                        DefectCode_ID = g1Item.Key.DefectCode_ID,
                        DefectCode_Name = g1Item.Key.DefectCode_Name,
                        TotalCount = g1Item.Count()
                    });
                }
            }

            totalcount = repairList.Count();
            if (isExport) //如果是导出数据则导出所有数据
            {
                return repairList;
            }
            else //如果是查询则按条件分页
            {
                return repairList.Skip(page.Skip).Take(page.PageSize).ToList();
            }
        }

        public List<ReportByRepair> ExportReportByDetailValid(ReportByRepair model)
        {
            int count = 0;
            return QueryFixtureReportByDetail(model, null, out count, true);
        }

        #endregion 报表-治具维修次数查询（明细） Add by Rock 2017-11-24------------------------ End

        #region 报表-治具间维修时间分析报表 Add by Rock 2017-11-24------------------------ Start
        public List<ReportByRepair> QueryFixtureReportByAnalisis(ReportByRepair model, Page page, out int totalcount, bool isExport)
        {
            var linq = from A in DataContext.Fixture_Repair_M

                       join B in DataContext.Fixture_Repair_D
                       on A.Fixture_Repair_M_UID equals B.Fixture_Repair_M_UID

                       join C in DataContext.Repair_Location
                       on A.Repair_Location_UID equals C.Repair_Location_UID

                       join E in DataContext.Fixture_M
                       on B.Fixture_M_UID equals E.Fixture_M_UID

                       join EE in DataContext.Enumeration
                       on new { Enum_Type = StructConstants.FixtureStatus.Fixture_Status, Enum_UID = E.Status } equals new { Enum_Type = EE.Enum_Type, Enum_UID = EE.Enum_UID }

                       join F in DataContext.Production_Line
                       on E.Production_Line_UID equals F.Production_Line_UID into EFTemp
                       from EF in EFTemp.DefaultIfEmpty()

                       join G in DataContext.WorkStation
                       on EF.Workstation_UID equals G.WorkStation_UID

                       join H in DataContext.System_Organization
                       on A.Plant_Organization_UID equals H.Organization_UID

                       join I in DataContext.System_Organization
                       on A.BG_Organization_UID equals I.Organization_UID

                       join J in DataContext.System_Organization
                       on A.FunPlant_Organization_UID equals J.Organization_UID into BJTemp
                       from BJ in BJTemp.DefaultIfEmpty()

                       select new ReportByRepair
                       {
                           Plant_Organization_UID = H.Organization_UID,
                           BG_Organization_UID = I.Organization_UID,
                           FunPlant_Organization_UID = BJ.Organization_UID,
                           PlantName = H.Organization_Name,
                           OpType_Name = I.Organization_Name,
                           Func_Name = BJ.Organization_Name,
                           WorkStation_UID = G.WorkStation_UID,
                           WorkStation_Name = G.WorkStation_Name,
                           WorkStation_Is_Enable = G.Is_Enable,
                           Repair_Is_Enable = C.Is_Enable,
                           Production_Line_Is_Enable = EF.Is_Enable,
                           Repair_Location_UID = C.Repair_Location_UID,
                           Repair_Location_Name = C.Repair_Location_Name,
                           Fixture_M_UID = B.Fixture_M_UID,
                           Fixture_NO = E.Fixture_NO,
                           Fixture_Unique_ID = E.Fixture_Unique_ID,
                           Fixture_Status = EE.Enum_Value,
                           SentOut_Date = A.SentOut_Date,
                           Completion_Date = B.Completion_Date,
                           TimeInterval = SqlFunctions.DateDiff("MINUTE", A.SentOut_Date, B.Completion_Date)
                       };

            linq = linq.Where(m => m.Production_Line_Is_Enable == true && m.WorkStation_Is_Enable == true
            && m.Repair_Is_Enable == true && m.Completion_Date != null);
            if (model.Plant_Organization_UID != 0)
            {
                linq = linq.Where(m => m.Plant_Organization_UID.Equals(model.Plant_Organization_UID));
            }
            if (model.BG_Organization_UID != 0)
            {
                linq = linq.Where(m => m.BG_Organization_UID.Equals(model.BG_Organization_UID));
            }
            if (model.FunPlant_Organization_UID != null && model.FunPlant_Organization_UID != 0)
            {
                linq = linq.Where(m => m.FunPlant_Organization_UID.Value.Equals(model.FunPlant_Organization_UID.Value));
            }
            if (model.WorkStation_UID != 0)
            {
                linq = linq.Where(m => m.WorkStation_UID.Equals(model.WorkStation_UID));
            }
            if (model.Repair_Location_UID != 0)
            {
                linq = linq.Where(m => m.Repair_Location_UID.Equals(model.Repair_Location_UID));
            }

            linq = linq.Where(m => SqlFunctions.DateDiff("dd", m.SentOut_Date, model.StartDate) <= 0);
            linq = linq.Where(m => SqlFunctions.DateDiff("dd", m.SentOut_Date, model.EndDate) >= 0);

            var list = linq.ToList();
            List<ReportByRepair> repairList = new List<ReportByRepair>();

            var gList = list.GroupBy(m => new { m.PlantName, m.OpType_Name, m.Func_Name, m.WorkStation_Name, m.Repair_Location_Name, m.Fixture_NO });
            foreach (var gItem in gList)
            {
                var aList = gItem.Where(m => m.TimeInterval < 30).ToList();
                var bList = gItem.Where(m => m.TimeInterval >= 30 && m.TimeInterval < 120).ToList();
                var cList = gItem.Where(m => m.TimeInterval >= 120 && m.TimeInterval < 240).ToList();
                var dList = gItem.Where(m => m.TimeInterval >= 240).ToList();

                ReportByRepair newItem = new ReportByRepair();
                newItem.PlantName = gItem.Key.PlantName;
                newItem.OpType_Name = gItem.Key.OpType_Name;
                newItem.Func_Name = gItem.Key.Func_Name;
                newItem.WorkStation_Name = gItem.Key.WorkStation_Name;
                newItem.Repair_Location_Name = gItem.Key.Repair_Location_Name;
                newItem.Fixture_NO = gItem.Key.Fixture_NO;

                if (aList.Count() > 0)
                {
                    newItem.LessHalfMinutes = aList.Count();
                }
                else
                {
                    newItem.LessHalfMinutes = 0;
                }

                if (bList.Count() > 0)
                {
                    newItem.LessTwoHour = bList.Count();
                }
                else
                {
                    newItem.LessTwoHour = 0;
                }

                if (cList.Count() > 0)
                {
                    newItem.LessFourHour = cList.Count();
                }
                else
                {
                    newItem.LessFourHour = 0;
                }

                if (dList.Count() > 0)
                {
                    newItem.OtherHour = dList.Count();
                }
                else
                {
                    newItem.OtherHour = 0;
                }

                repairList.Add(newItem);
            }

            totalcount = repairList.Count();
            if (isExport) //如果是导出数据则导出所有数据
            {
                return repairList;
            }
            else //如果是查询则按条件分页
            {
                return repairList.Skip(page.Skip).Take(page.PageSize).ToList();
            }
        }

        public List<ReportByRepair> ExportReportByAnalisisValid(ReportByRepair model)
        {
            int count = 0;
            return QueryFixtureReportByAnalisis(model, null, out count, true);
        }

        #endregion 报表-治具间维修时间分析报表 Add by Rock 2017-11-24------------------------ End

        #region 报表-治具数量查询按冶具状态 Add by Rock 2017-11-29------------------------ Start

        public List<ReportByRepair> QueryFixtureReportByStatus(ReportByRepair model, Page page, out int totalcount, bool isExport)
        {
            var linq = from E in DataContext.Fixture_M

                       join EE in DataContext.Enumeration
                       on new { Enum_Type = StructConstants.FixtureStatus.Fixture_Status, Enum_UID = E.Status } equals new { Enum_Type = EE.Enum_Type, Enum_UID = EE.Enum_UID }

                       join F in DataContext.Production_Line
                       on E.Production_Line_UID equals F.Production_Line_UID into EFTemp
                       from EF in EFTemp.DefaultIfEmpty()

                       join G in DataContext.WorkStation
                       on EF.Workstation_UID equals G.WorkStation_UID

                       join H in DataContext.System_Organization
                       on E.Plant_Organization_UID equals H.Organization_UID

                       join I in DataContext.System_Organization
                       on E.BG_Organization_UID equals I.Organization_UID

                       join J in DataContext.System_Organization
                       on E.FunPlant_Organization_UID equals J.Organization_UID into BJTemp
                       from BJ in BJTemp.DefaultIfEmpty()

                       select new ReportByRepair
                       {
                           Plant_Organization_UID = H.Organization_UID,
                           BG_Organization_UID = I.Organization_UID,
                           FunPlant_Organization_UID = BJ.Organization_UID,
                           PlantName = H.Organization_Name,
                           OpType_Name = I.Organization_Name,
                           Func_Name = BJ.Organization_Name,
                           WorkStation_UID = G.WorkStation_UID,
                           WorkStation_Name = G.WorkStation_Name,
                           WorkStation_Is_Enable = G.Is_Enable,
                           Production_Line_Is_Enable = EF.Is_Enable,
                           Fixture_M_UID = E.Fixture_M_UID,
                           Fixture_NO = E.Fixture_NO,
                           Fixture_Unique_ID = E.Fixture_Unique_ID,
                           Fixture_Status = EE.Enum_Value
                       };
            linq = linq.Where(m => m.Production_Line_Is_Enable == true && m.WorkStation_Is_Enable == true);
            if (model.Plant_Organization_UID != 0)
            {
                linq = linq.Where(m => m.Plant_Organization_UID.Equals(model.Plant_Organization_UID));
            }
            if (model.BG_Organization_UID != 0)
            {
                linq = linq.Where(m => m.BG_Organization_UID.Equals(model.BG_Organization_UID));
            }
            if (model.FunPlant_Organization_UID != null && model.FunPlant_Organization_UID != 0)
            {
                linq = linq.Where(m => m.FunPlant_Organization_UID.Value.Equals(model.FunPlant_Organization_UID.Value));
            }
            if (model.WorkStation_UID != 0)
            {
                linq = linq.Where(m => m.WorkStation_UID.Equals(model.WorkStation_UID));
            }
            if (!string.IsNullOrEmpty(model.Fixture_NO))
            {
                linq = linq.Where(m => m.Fixture_NO.Contains(model.Fixture_NO));
            }

            var list = linq.OrderBy(m => m.Fixture_NO).ToList();
            List<ReportByRepair> repairList = new List<ReportByRepair>();

            var gList = list.GroupBy(m => new { m.PlantName, m.OpType_Name, m.Func_Name, m.WorkStation_Name, m.Fixture_NO, m.Fixture_Status }).ToList();
            foreach (var gItem in gList)
            {
                repairList.Add(new ReportByRepair
                {
                    PlantName = gItem.Key.PlantName,
                    OpType_Name = gItem.Key.OpType_Name,
                    Func_Name = gItem.Key.Func_Name,
                    WorkStation_Name = gItem.Key.WorkStation_Name,
                    Fixture_NO = gItem.Key.Fixture_NO,
                    Fixture_Status = gItem.Key.Fixture_Status,
                    TotalCount = gItem.Count()
                });
            }

            totalcount = repairList.Count();
            if (isExport) //如果是导出数据则导出所有数据
            {
                return repairList;
            }
            else //如果是查询则按条件分页
            {
                return repairList.Skip(page.Skip).Take(page.PageSize).ToList();
            }
        }

        public List<string> GetFixtureNoList(int Plant_Organization_UID, int BG_Organization_UID, int FunPlant_Organization_UID)
        {
            var linq = from A in DataContext.Fixture_M
                       select new FixtureDTO
                       {
                           Plant_Organization_UID = A.Plant_Organization_UID,
                           BG_Organization_UID = A.BG_Organization_UID,
                           FunPlant_Organization_UID = A.FunPlant_Organization_UID,
                           Fixture_NO = A.Fixture_NO,
                       };
            if (Plant_Organization_UID != 0)
            {
                linq = linq.Where(m => m.Plant_Organization_UID.Equals(Plant_Organization_UID));
            }
            if (BG_Organization_UID != 0)
            {
                linq = linq.Where(m => m.BG_Organization_UID.Equals(BG_Organization_UID));
            }
            if (FunPlant_Organization_UID != 0)
            {
                linq = linq.Where(m => m.FunPlant_Organization_UID.Value.Equals(FunPlant_Organization_UID));
            }
            linq = linq.OrderBy(m => m.Fixture_NO);
            var list = linq.ToList().Select(m => m.Fixture_NO).Distinct().ToList();
            return list;
        }

        public List<ReportByRepair> ExportReportByStatusValid(ReportByRepair model)
        {
            int count = 0;
            return QueryFixtureReportByStatus(model, null, out count, true);

        }

        #endregion 报表-治具数量查询按冶具状态 Add by Rock 2017-11-29------------------------ End

        #region 报表-厂内治具状态分析表 Add by Rock 2017-12-03----------------Start
        public List<ReportByStatusAnalisis> QueryFixtureReportByStatusAnalisis(ReportByRepair model, Page page, out int totalcount, bool isExport)
        {
            var status = (from A in DataContext.Enumeration
                          where A.Enum_Type == StructConstants.FixtureStatus.Fixture_Status
                          select new
                          {
                              A.Enum_UID,
                              A.Enum_Name
                          }).ToList();

            var statusOne = status.Where(m => m.Enum_Name == "1").Select(m => m.Enum_UID).First();
            var statusTwo = status.Where(m => m.Enum_Name == "2").Select(m => m.Enum_UID).First();
            var statusThree = status.Where(m => m.Enum_Name == "3").Select(m => m.Enum_UID).First();
            var statusFour = status.Where(m => m.Enum_Name == "4").Select(m => m.Enum_UID).First();
            var statusFive = status.Where(m => m.Enum_Name == "5").Select(m => m.Enum_UID).First();
            var statusSix = status.Where(m => m.Enum_Name == "6").Select(m => m.Enum_UID).First();

            //获取工站的所有条件
            var workstationLinq = from G in DataContext.WorkStation

                                  join H in DataContext.System_Organization
                                  on G.Plant_Organization_UID equals H.Organization_UID

                                  join I in DataContext.System_Organization
                                  on G.BG_Organization_UID equals I.Organization_UID

                                  join J in DataContext.System_Organization
                                  on G.FunPlant_Organization_UID equals J.Organization_UID into BJTemp
                                  from BJ in BJTemp.DefaultIfEmpty()

                                  select new ReportByRepair
                                  {
                                      Plant_Organization_UID = H.Organization_UID,
                                      BG_Organization_UID = I.Organization_UID,
                                      FunPlant_Organization_UID = BJ.Organization_UID,
                                      PlantName = H.Organization_Name,
                                      OpType_Name = I.Organization_Name,
                                      Func_Name = BJ.Organization_Name,
                                      WorkStation_UID = G.WorkStation_UID,
                                      WorkStation_Name = G.WorkStation_Name,
                                  };

            if (model.Plant_Organization_UID != 0)
            {
                workstationLinq = workstationLinq.Where(m => m.Plant_Organization_UID.Equals(model.Plant_Organization_UID));
            }
            if (model.BG_Organization_UID != 0)
            {
                workstationLinq = workstationLinq.Where(m => m.BG_Organization_UID.Equals(model.BG_Organization_UID));
            }
            if (model.FunPlant_Organization_UID != null && model.FunPlant_Organization_UID != 0)
            {
                workstationLinq = workstationLinq.Where(m => m.FunPlant_Organization_UID.Value.Equals(model.FunPlant_Organization_UID.Value));
            }
            if (model.WorkStation_UID != 0)
            {
                workstationLinq = workstationLinq.Where(m => m.WorkStation_UID.Equals(model.WorkStation_UID));
            }


            var allWrokStationList = workstationLinq.ToList();
            if (allWrokStationList.Count() == 0)
            {
                totalcount = 0;
                return new List<ReportByStatusAnalisis>();
            }

            List<ReportByRepair> repairList = new List<ReportByRepair>();
            foreach (var item in allWrokStationList)
            {
                repairList.Add(new ReportByRepair
                {
                    Plant_Organization_UID = item.Plant_Organization_UID,
                    BG_Organization_UID = item.BG_Organization_UID,
                    FunPlant_Organization_UID = item.FunPlant_Organization_UID,
                    PlantName = item.PlantName,
                    OpType_Name = item.OpType_Name,
                    Func_Name = item.Func_Name,
                    WorkStation_UID = item.WorkStation_UID,
                    WorkStation_Name = item.WorkStation_Name,
                });
            }


            #region 获取治具的状态信息

            //var linq = from E in DataContext.Fixture_M

            //           join EE in DataContext.Enumeration
            //           on new { Enum_Type = StructConstants.FixtureStatus.Fixture_Status, Enum_UID = E.Status } equals new { Enum_Type = EE.Enum_Type, Enum_UID = EE.Enum_UID }

            //           join F in DataContext.Production_Line
            //           on E.Production_Line_UID equals F.Production_Line_UID into EFTemp
            //           from EF in EFTemp.DefaultIfEmpty()

            //           join G in DataContext.WorkStation
            //           on EF.Workstation_UID equals G.WorkStation_UID

            //           join H in DataContext.System_Organization
            //           on E.Plant_Organization_UID equals H.Organization_UID

            //           join I in DataContext.System_Organization
            //           on E.BG_Organization_UID equals I.Organization_UID

            //           join J in DataContext.System_Organization
            //           on E.FunPlant_Organization_UID equals J.Organization_UID into BJTemp
            //           from BJ in BJTemp.DefaultIfEmpty()

            //           select new ReportByRepair
            //           {
            //               Plant_Organization_UID = H.Organization_UID,
            //               BG_Organization_UID = I.Organization_UID,
            //               FunPlant_Organization_UID = BJ.Organization_UID,
            //               PlantName = H.Organization_Name,
            //               OpType_Name = I.Organization_Name,
            //               Func_Name = BJ.Organization_Name,
            //               WorkStation_UID = G.WorkStation_UID,
            //               WorkStation_Name = G.WorkStation_Name,
            //               WorkStation_Is_Enable = G.Is_Enable,
            //               Production_Line_Is_Enable = EF.Is_Enable,
            //               Fixture_M_UID = E.Fixture_M_UID,
            //               Fixture_NO = E.Fixture_NO,
            //               Fixture_Unique_ID = E.Fixture_Unique_ID,
            //               Fixture_Status_UID = EE.Enum_UID,
            //               Fixture_Status = EE.Enum_Value
            //           };
            var linq = from F in DataContext.Fixture_M select F;
            //linq = linq.Where(m => m.Production_Line_Is_Enable == true && m.WorkStation_Is_Enable == true);
            if (model.Plant_Organization_UID != 0)
            {
                linq = linq.Where(m => m.Plant_Organization_UID.Equals(model.Plant_Organization_UID));
            }
            if (model.BG_Organization_UID != 0)
            {
                linq = linq.Where(m => m.BG_Organization_UID.Equals(model.BG_Organization_UID));
            }
            if (model.FunPlant_Organization_UID != null && model.FunPlant_Organization_UID != 0)
            {
                linq = linq.Where(m => m.FunPlant_Organization_UID.Value.Equals(model.FunPlant_Organization_UID.Value));
            }
            if (model.WorkStation_UID != 0)
            {
                linq = linq.Where(m => m.Production_Line != null && m.Production_Line.Workstation_UID.Equals(model.WorkStation_UID) && m.Production_Line.Is_Enable == true && m.Production_Line.WorkStation.Is_Enable == true);
            }
            var dtoList = new List<ReportByRepair>();
            foreach (var item in linq)
            {
                var dto = new ReportByRepair();
                if (item.System_Organization != null)
                {
                    dto.Plant_Organization_UID = item.System_Organization.Organization_UID;
                    dto.PlantName = item.System_Organization.Organization_Name;
                }

                if (item.System_Organization1 != null)
                {
                    dto.BG_Organization_UID = item.System_Organization1.Organization_UID;
                    dto.OpType_Name = item.System_Organization1.Organization_Name;
                }

                if (item.System_Organization2 != null)
                {
                    dto.FunPlant_Organization_UID = item.System_Organization2.Organization_UID;
                    dto.Func_Name = item.System_Organization2.Organization_Name;

                }
                dto.Fixture_M_UID = item.Fixture_M_UID;
                dto.Fixture_NO = item.Fixture_NO;
                dto.Fixture_Unique_ID = item.Fixture_Unique_ID;
                dto.Fixture_Status_UID = item.Enumeration.Enum_UID;
                dto.Fixture_Status = item.Enumeration.Enum_Value;
                if (item.Production_Line != null)
                {
                    dto.WorkStation_UID = item.Production_Line.WorkStation.WorkStation_UID;
                    dto.WorkStation_Name = item.Production_Line.WorkStation.WorkStation_Name;
                    dto.WorkStation_Is_Enable = item.Production_Line.WorkStation.Is_Enable;
                    dto.Production_Line_Is_Enable = item.Production_Line.Is_Enable;
                }

                dtoList.Add(dto);
            }
            var list = dtoList;

            //总记录数,要放在WorkStation_UID的前面
            var fixTotalList = list;//linq.ToList();
            var fixTotalCount = fixTotalList.Count();

            #endregion

            #region 获取未保养已保养信息
            var profileLinq = from A in DataContext.Fixture_Maintenance_Profile
                              select new
                              {
                                  A.Fixture_Maintenance_Profile_UID,
                                  A.Plant_Organization_UID,
                                  A.BG_Organization_UID,
                                  A.FunPlant_Organization_UID
                              };
            if (model.Plant_Organization_UID != 0)
            {
                profileLinq = profileLinq.Where(m => m.Plant_Organization_UID.Equals(model.Plant_Organization_UID));
            }
            if (model.BG_Organization_UID != 0)
            {
                profileLinq = profileLinq.Where(m => m.BG_Organization_UID.Equals(model.BG_Organization_UID));
            }
            if (model.FunPlant_Organization_UID != null && model.FunPlant_Organization_UID != 0)
            {
                profileLinq = profileLinq.Where(m => m.FunPlant_Organization_UID.Value.Equals(model.FunPlant_Organization_UID.Value));
            }
            //这个数据算出来的是治具总数的已保养或未保养，各个工站的已保养未保养还没统计出来
            var profileUIDList = profileLinq.Select(m => m.Fixture_Maintenance_Profile_UID).ToList();

            string dateStr = DateTime.Now.ToString("yyyyMMdd");
            var maintenLinq = from A in DataContext.Fixture_Maintenance_Record

                              join E in DataContext.Fixture_M
                              on A.Fixture_M_UID equals E.Fixture_M_UID

                              join F in DataContext.Production_Line
                              on E.Production_Line_UID equals F.Production_Line_UID into EFTemp
                              from EF in EFTemp.DefaultIfEmpty()

                              join G in DataContext.WorkStation
                              on EF.Workstation_UID equals G.WorkStation_UID

                              where profileUIDList.Contains(A.Fixture_Maintenance_Profile_UID) && A.Maintenance_Record_NO.Contains(dateStr)
                              select new
                              {
                                  A.Fixture_Maintenance_Record_UID,
                                  A.Maintenance_Status,
                                  G.WorkStation_UID
                              };


            var recordList = maintenLinq.ToList();

            #endregion

            List<ReportByStatusAnalisis> analisisList = new List<ReportByStatusAnalisis>();
            analisisList = AutoMapper.Mapper.Map<List<ReportByStatusAnalisis>>(repairList);

            foreach (var item in analisisList)
            {
                //治具总数
                item.TotalCount = list.Where(m => m.WorkStation_UID == item.WorkStation_UID).Count();
                //使用中
                item.StatusOne = list.Where(m => m.WorkStation_UID == item.WorkStation_UID && m.Fixture_Status_UID == statusOne).Count();
                //未使用
                item.StatusTwo = list.Where(m => m.WorkStation_UID == item.WorkStation_UID && m.Fixture_Status_UID == statusTwo).Count();
                //维修中
                item.StatusThree = list.Where(m => m.WorkStation_UID == item.WorkStation_UID && m.Fixture_Status_UID == statusThree).Count();
                //报废
                item.StatusFour = list.Where(m => m.WorkStation_UID == item.WorkStation_UID && m.Fixture_Status_UID == statusFour).Count();
                //返供应商维修RTV
                item.StatusFive = list.Where(m => m.WorkStation_UID == item.WorkStation_UID && m.Fixture_Status_UID == statusFive).Count();
                //保养逾时
                item.StatusSix = list.Where(m => m.WorkStation_UID == item.WorkStation_UID && m.Fixture_Status_UID == statusSix).Count();
                //已保养
                item.StatusSeven = recordList.Where(m => m.Maintenance_Status == 1 && m.WorkStation_UID == item.WorkStation_UID).Count();
                //未保养
                item.StatusEight = recordList.Where(m => m.Maintenance_Status == 0 && m.WorkStation_UID == item.WorkStation_UID).Count();
            }

            //插入治具总数的统计并放在第一行
            var firstItem = repairList.First();
            analisisList.Insert(0, new ReportByStatusAnalisis
            {
                Plant_Organization_UID = firstItem.Plant_Organization_UID,
                BG_Organization_UID = firstItem.BG_Organization_UID,
                FunPlant_Organization_UID = firstItem.FunPlant_Organization_UID,
                PlantName = firstItem.PlantName,
                OpType_Name = firstItem.OpType_Name,
                Func_Name = firstItem.Func_Name,
                WorkStation_UID = 0,
                WorkStation_Name = "治具总数",
                TotalCount = fixTotalCount,
                //使用中
                StatusOne = fixTotalList.Where(m => m.Fixture_Status_UID == statusOne).Count(),
                //未使用
                StatusTwo = fixTotalList.Where(m => m.Fixture_Status_UID == statusTwo).Count(),
                //维修中
                StatusThree = fixTotalList.Where(m => m.Fixture_Status_UID == statusThree).Count(),
                //报废
                StatusFour = fixTotalList.Where(m => m.Fixture_Status_UID == statusFour).Count(),
                //返供应商维修RTV
                StatusFive = fixTotalList.Where(m => m.Fixture_Status_UID == statusFive).Count(),
                //保养逾时
                StatusSix = fixTotalList.Where(m => m.Fixture_Status_UID == statusSix).Count(),
                //已保养, Modified by Jay加上过滤条件 m.WorkStation_UID == model.WorkStation_UID
                StatusSeven = recordList.Where(m => m.Maintenance_Status == 1 && m.WorkStation_UID == model.WorkStation_UID).Count(),
                //未保养, Modified by Jay加上过滤条件 m.WorkStation_UID == model.WorkStation_UID
                StatusEight = recordList.Where(m => m.Maintenance_Status == 0 && m.WorkStation_UID == model.WorkStation_UID).Count()
            });

            //整理报表格式，输出指定格式
            List<ReportByStatusAnalisis> reportList = new List<ReportByStatusAnalisis>();
            foreach (var item in analisisList)
            {
                reportList.Add(item);

                reportList.Add(new ReportByStatusAnalisis
                {
                    Plant_Organization_UID = firstItem.Plant_Organization_UID,
                    BG_Organization_UID = firstItem.BG_Organization_UID,
                    FunPlant_Organization_UID = firstItem.FunPlant_Organization_UID,
                    PlantName = firstItem.PlantName,
                    OpType_Name = firstItem.OpType_Name,
                    Func_Name = firstItem.Func_Name,
                    WorkStation_UID = 0,
                    WorkStation_Name = string.Format("占{0}总数百分比", item.WorkStation_Name),
                    TotalCount = -1, //这个值在前端画面转换为-
                    //使用中
                    StatusOne = item.TotalCount == 0 ? 0 : Math.Round((item.StatusOne / item.TotalCount * 100), 2),
                    StatusTwo = item.TotalCount == 0 ? 0 : Math.Round((item.StatusTwo / item.TotalCount * 100), 2),
                    StatusThree = item.TotalCount == 0 ? 0 : Math.Round((item.StatusThree / item.TotalCount * 100), 2),
                    StatusFour = item.TotalCount == 0 ? 0 : Math.Round((item.StatusFour / item.TotalCount * 100), 2),
                    StatusFive = item.TotalCount == 0 ? 0 : Math.Round((item.StatusFive / item.TotalCount * 100), 2),
                    StatusSix = item.TotalCount == 0 ? 0 : Math.Round((item.StatusSix / item.TotalCount * 100), 2),
                    StatusSeven = item.TotalCount == 0 ? 0 : Math.Round((item.StatusSeven / item.TotalCount * 100), 2),
                    StatusEight = item.TotalCount == 0 ? 0 : Math.Round((item.StatusEight / item.TotalCount * 100), 2)
                });

            }



            totalcount = reportList.Count();
            if (isExport) //如果是导出数据则导出所有数据
            {
                return reportList;
            }
            else //如果是查询则按条件分页
            {
                return reportList.Skip(page.Skip).Take(page.PageSize).ToList();
            }
        }


        public List<ReportByStatusAnalisis> ExportReportByStatusAnalisisValid(ReportByRepair model)
        {
            int count = 0;
            return QueryFixtureReportByStatusAnalisis(model, null, out count, true);
        }

        #endregion 报表-厂内治具状态分析表 Add by Rock 2017-12-03----------------End


        #region 报表-FMT Dashboard Add by Rock 2017-12-11----------------Start
        public List<Batch_ReportByStatus> QueryFixtureReportByFMT(Batch_ReportByStatus model, Page page, out int totalcount, bool isExport, DateTime StartDate, DateTime EndDate)
        {
            //获取使用中的状态
            var statusUID = DataContext.Enumeration.Where(m => m.Enum_Type == StructConstants.FixtureStatus.Fixture_Status && m.Enum_Name == "1").Select(m => m.Enum_UID).First();

            var linq = from A in DataContext.Process_Info

                       join H in DataContext.System_Organization
                       on A.Plant_Organization_UID equals H.Organization_UID

                       join I in DataContext.System_Organization
                       on A.BG_Organization_UID equals I.Organization_UID

                       join J in DataContext.System_Organization
                       on A.FunPlant_Organization_UID equals J.Organization_UID into BJTemp
                       from BJ in BJTemp.DefaultIfEmpty()

                       select new Batch_ReportByStatus
                       {
                           Plant_Organization_UID = H.Organization_UID,
                           BG_Organization_UID = I.Organization_UID,
                           FunPlant_Organization_UID = BJ.Organization_UID,
                           PlantName = H.Organization_Name,
                           OpType_Name = I.Organization_Name,
                           Func_Name = BJ.Organization_Name,
                           Process_Info_UID = A.Process_Info_UID,
                           Process_Name = A.Process_Name
                       };

            if (model.Plant_Organization_UID != 0)
            {
                linq = linq.Where(m => m.Plant_Organization_UID.Equals(model.Plant_Organization_UID));
            }
            if (model.BG_Organization_UID != 0)
            {
                linq = linq.Where(m => m.BG_Organization_UID.Equals(model.BG_Organization_UID));
            }
            if (model.FunPlant_Organization_UID != null && model.FunPlant_Organization_UID != 0)
            {
                linq = linq.Where(m => m.FunPlant_Organization_UID.Value.Equals(model.FunPlant_Organization_UID.Value));
            }

            var list = linq.ToList();

            var processUIDList = list.Select(m => m.Process_Info_UID).ToList();

            //获取所有制程的治具总数放内存
            var totalSql = GetTotalInfo(processUIDList);
            var fixTotalList = DataContext.Database.SqlQuery<Batch_TotalFix>(totalSql).ToList();
            var fixTotalUIDList = fixTotalList.Select(m => m.Fixture_M_UID).ToList();

            //获取所有制程的当日新增放内存
            var NewCountList = GetNewInfo(fixTotalUIDList, StartDate, EndDate);

            //获取所有制程的当日报废数放内存
            var FreeCountList = GetFreeInfo(fixTotalUIDList, StartDate, EndDate);

            //获取该制程的当日送修数放内存
            var repairList = GetCurrentDayRepairList(fixTotalUIDList, StartDate, EndDate);

            //获取该制程的当日领用数
            var shipList = GetCurrentDayTotakeList(fixTotalUIDList, StartDate, EndDate);

            //获取该制程的当日待修数
            var waitRepair = GetCurrentDayNotRepairList(fixTotalUIDList, StartDate, EndDate);

            //获取该制程的当日应保养数，已保养数，未保养数
            var allMainten = GetCurrentDayHasMaintenList(fixTotalUIDList, StartDate, EndDate);

            //获取该制程的当日未保养数
            //var notMainten = GetCurrentDayNotMaintenList(fixTotalUIDList, model.StartDate.Value, model.EndDate.Value);

            foreach (var item in list)
            {
                //获取该制程的治具总数
                //var totalSql = GetTotalInfo(item.Process_Info_UID);
                var fixList = fixTotalList.Where(m => m.Plant_Organization_UID == item.Plant_Organization_UID && m.BG_Organization_UID == item.BG_Organization_UID && m.FunPlant_Organization_UID == item.FunPlant_Organization_UID
                && m.Process_Info_UID == item.Process_Info_UID).ToList();
                var fixMUIDList = fixList.Select(m => m.Fixture_M_UID).ToList();
                item.TotalCount = fixList.Count();

                //获取该制程的当日新增
                item.NewCount = NewCountList.Where(m => fixMUIDList.Contains(m.Fixture_M_UID)).Count();

                //获取该制程的当日报废
                item.FreeCount = FreeCountList.Where(m => fixMUIDList.Contains(m.Fixture_M_UID)).Count();

                //获取该制程的当日送修数
                item.SendRepairCount = repairList.Where(m => fixMUIDList.Contains(m.Fixture_M_UID)).Count();

                //获取该制程的当日领用数
                item.ShipCount = shipList.Where(m => fixMUIDList.Contains(m.Fixture_M_UID)).Count();

                //获取该制程的当日待修数
                item.WaitRepairCount = waitRepair.Where(m => fixMUIDList.Contains(m.Fixture_M_UID)).Count();

                //获取该制程的当日应保养数
                var distinctUIDList = allMainten.Where(m => fixMUIDList.Contains(m.Fixture_M_UID)).Select(m => m.Fixture_M_UID).Distinct().ToList();
                item.NeedMaintenCount = distinctUIDList.Count();

                //获取该制程的当日已保养数
                //allMainten里面包含了未保养和已保养
                item.HasMaintenCount = item.NeedMaintenCount;
                foreach (var disUID in distinctUIDList)
                {
                    var hasNotMain = allMainten.Where(m => m.Fixture_M_UID == disUID && m.Maintenance_Status == 0).Count();
                    if (hasNotMain > 0)
                    {
                        item.HasMaintenCount--;
                    }
                }

                //var hasMaintenList = allMainten.Where(m => fixMUIDList.Contains(m.Fixture_M_UID) && m.Maintenance_Status == 1).Select(m => m.Fixture_M_UID).ToList();
                ////取出二个班别都有数据的就是已保养的数据，这里只要数据>1就是已保养的
                //var bb = hasMaintenList.GroupBy(m => new { m }).Select(m => new {A = m, Count = m.Count() } ).ToList();
                //var aa = bb.Where(m => m.Count > 1).Count();
                //item.HasMaintenCount = aa;

                //获取该制程的当日未保养数
                item.NotMaintenCount = item.NeedMaintenCount - item.HasMaintenCount;
            }

            totalcount = list.Count();

            if (isExport)
            {
                return list;
            }
            else
            {
                list = list.OrderBy(m => m.NotMaintenCount).ToList();
                return list.Skip(page.Skip).Take(page.PageSize).ToList();
            }
        }

        public List<Batch_ReportByStatus> QueryQueryFixtureReportByFMTDetail(int Process_Info_UID, DateTime StartDate, DateTime EndDate, int SheetCount)
        {
            var statusUID = DataContext.Enumeration.Where(m => m.Enum_Type == StructConstants.FixtureStatus.Fixture_Status && m.Enum_Name == "1").Select(m => m.Enum_UID).First();

            var processName = DataContext.Process_Info.Where(m => m.Process_Info_UID == Process_Info_UID).Select(m => m.Process_Name).First();

            var linq = from A in DataContext.Fixture_M

                       join B in DataContext.Production_Line
                       on A.Production_Line_UID equals B.Production_Line_UID

                       join C in DataContext.WorkStation
                       on B.Workstation_UID equals C.WorkStation_UID

                       where C.Process_Info_UID == Process_Info_UID
                       select new { A.Plant_Organization_UID, A.BG_Organization_UID, A.FunPlant_Organization_UID, A.Fixture_M_UID, A.Status, B.Process_Info_UID, C.WorkStation_UID };

            var list = linq.ToList();
            var fixTotalUIDList = list.Select(m => m.Fixture_M_UID).ToList();

            //获取该制程的当日新增
            var NewCountList = GetNewInfo(fixTotalUIDList, StartDate, EndDate);

            //获取该制程的当日报废数
            var FreeCountList = GetFreeInfo(fixTotalUIDList, StartDate, EndDate);

            //获取该制程的当日送修数
            var repairList = GetCurrentDayRepairList(fixTotalUIDList, StartDate, EndDate);

            //获取该制程的当日领用数
            var shipList = GetCurrentDayTotakeList(fixTotalUIDList, StartDate, EndDate);

            //获取该制程的当日待修数
            var waitRepair = GetCurrentDayNotRepairList(fixTotalUIDList, StartDate, EndDate);

            //获取该制程的当日应保养数，已保养数，未保养数
            var allMainten = GetCurrentDayHasMaintenList(fixTotalUIDList, StartDate, EndDate);


            //获取该制程的当日未保养数
            //var notMainten = GetCurrentDayNotMaintenList(fixTotalUIDList, StartDate, EndDate);

            var workStationList = DataContext.WorkStation.Where(m => m.Process_Info_UID == Process_Info_UID).ToList();
            List<Batch_ReportByStatus> detailList = new List<Batch_ReportByStatus>();
            foreach (var workStationItem in workStationList)
            {
                var fixUIDList = list.Where(m => m.Plant_Organization_UID == workStationItem.Plant_Organization_UID && m.BG_Organization_UID == workStationItem.BG_Organization_UID && m.FunPlant_Organization_UID == workStationItem.FunPlant_Organization_UID
                && m.WorkStation_UID == workStationItem.WorkStation_UID).Select(m => m.Fixture_M_UID).ToList();

                Batch_ReportByStatus statusItem = new Batch_ReportByStatus();
                statusItem.SheetCount = SheetCount;
                statusItem.Process_Info_UID = Process_Info_UID;
                statusItem.Process_Name = processName;
                statusItem.WorkStation_UID = workStationItem.WorkStation_UID;
                statusItem.WorkStation_Name = workStationItem.WorkStation_Name;

                //获取该制程的治具总数
                var totalCount = fixUIDList.Count();
                statusItem.TotalCount = totalCount;

                //获取该制程的当日新增
                statusItem.NewCount = NewCountList.Where(m => fixUIDList.Contains(m.Fixture_M_UID)).Count();

                //获取该制程的当日报废
                statusItem.FreeCount = FreeCountList.Where(m => fixUIDList.Contains(m.Fixture_M_UID)).Count();

                //获取该制程的当日送修数
                statusItem.SendRepairCount = repairList.Where(m => fixUIDList.Contains(m.Fixture_M_UID)).Count();

                //获取该制程的当日领用数
                statusItem.ShipCount = shipList.Where(m => fixUIDList.Contains(m.Fixture_M_UID)).Count();

                //获取该制程的当日待修数
                statusItem.WaitRepairCount = waitRepair.Where(m => fixUIDList.Contains(m.Fixture_M_UID)).Count();

                //获取该制程的当日应保养数
                //statusItem.NeedMaintenCount = list.Where(m => m.WorkStation_UID == workStationItem.WorkStation_UID && m.Status == statusUID).Count();
                var distinctUIDList = allMainten.Where(m => fixUIDList.Contains(m.Fixture_M_UID)).Select(m => m.Fixture_M_UID).Distinct().ToList();
                statusItem.NeedMaintenCount = distinctUIDList.Count();

                //获取该制程的当日已保养数
                //allMainten里面包含了未保养和已保养
                statusItem.HasMaintenCount = statusItem.NeedMaintenCount;
                foreach (var disUID in distinctUIDList)
                {
                    var hasNotMain = allMainten.Where(m => m.Fixture_M_UID == disUID && m.Maintenance_Status == 0).Count();
                    if (hasNotMain > 0)
                    {
                        statusItem.HasMaintenCount--;
                    }
                }

                //var hasMaintenList = allMainten.Where(m => fixUIDList.Contains(m.Fixture_M_UID) && m.Maintenance_Status == 1).Select(m => m.Fixture_M_UID).ToList();
                //取出二个班别都有数据的就是已保养的数据，这里只要数据>1就是已保养的
                //var bb = hasMaintenList.GroupBy(m => new { m }).Select(m => new { A = m, Count = m.Count() }).ToList();
                //var aa = bb.Where(m => m.Count > 1).Count();
                //statusItem.HasMaintenCount = aa;
                //statusItem.HasMaintenCount = hasMainten.Where(m => fixUIDList.Contains(m.Fixture_M_UID)).Count();

                //获取该制程的当日未保养数
                statusItem.NotMaintenCount = statusItem.NeedMaintenCount - statusItem.HasMaintenCount;

                detailList.Add(statusItem);
            }
            detailList = detailList.OrderBy(m => m.NotMaintenCount).ToList();
            return detailList;
        }

        //获取该制程的治具总数
        private string GetTotalInfo(List<int> Process_Info_UIDList)
        {
            var uidStr = string.Join("','", Process_Info_UIDList);

            var sql = @"SELECT A.Plant_Organization_UID,A.BG_Organization_UID,A.FunPlant_Organization_UID, A.Fixture_M_UID,A.Status,B.Process_Info_UID 
                        FROM dbo.Fixture_M A
                        JOIN dbo.Production_Line B
                        ON B.Production_Line_UID = A.Production_Line_UID
                        WHERE A.Production_Line_UID IN 
                        (SELECT Production_Line_UID FROM dbo.Production_Line
                        WHERE Process_Info_UID in ('{0}')) ";
            sql = string.Format(sql, uidStr);
            return sql;
        }

        //获取该制程的当日新增数
        private List<Batch_FMTNew> GetNewInfo(List<int> fixTotalUIDList, DateTime dateStart, DateTime dateEnd)
        {
            //var uidStr = string.Join("','", Process_Info_UIDList);
            if (dateStart.AddDays(6) > DateTime.Now) //一周之内
            {
                var list = (from A in DataContext.Fixture_Resume
                            where A.Data_Source == "1" && A.Modified_Date >= dateStart
                            && A.Modified_Date <= dateEnd && fixTotalUIDList.Contains(A.Fixture_M_UID)
                            select new Batch_FMTNew
                            {
                                Fixture_Resume_UID = A.Fixture_Resume_UID,
                                Data_Source = A.Data_Source,
                                Fixture_M_UID = A.Fixture_M_UID,
                                Modified_Date = A.Modified_Date
                            }).ToList();
                return list;
            }
            else //有可能超过一周需要超history表
            {
                var linq = (from A in DataContext.Fixture_Resume
                            where A.Data_Source == "1" && A.Modified_Date >= dateStart
                            && A.Modified_Date <= dateEnd && fixTotalUIDList.Contains(A.Fixture_M_UID)
                            select new Batch_FMTNew
                            {
                                Fixture_Resume_UID = A.Fixture_Resume_UID,
                                Data_Source = A.Data_Source,
                                Fixture_M_UID = A.Fixture_M_UID,
                                Modified_Date = A.Modified_Date
                            }).Union
                            (
                            from A in DataContext.Fixture_Resume_History
                            select new Batch_FMTNew
                            {
                                Fixture_Resume_UID = A.Fixture_Resume_UID,
                                Data_Source = A.Data_Source,
                                Fixture_M_UID = A.Fixture_M_UID,
                                Modified_Date = A.Modified_Date
                            });
                linq = linq.Where(A => A.Data_Source == "1" && A.Modified_Date >= dateStart && A.Modified_Date <= dateEnd && fixTotalUIDList.Contains(A.Fixture_M_UID));
                var list = linq.ToList();
                return list;

            }
        }

        //获取该制程的当日报废数
        private List<Batch_FMTNew> GetFreeInfo(List<int> fixTotalUIDList, DateTime dateStart, DateTime dateEnd)
        {
            string freeStr = "报废Scrap";
            if (dateStart.AddDays(6) > DateTime.Now) //一周之内
            {
                var list = (from A in DataContext.Fixture_Resume
                            where A.Data_Source == "4" && A.Resume_Notes.Equals(freeStr) && A.Modified_Date >= dateStart
                            && A.Modified_Date <= dateEnd && fixTotalUIDList.Contains(A.Fixture_M_UID)
                            select new Batch_FMTNew
                            {
                                Fixture_Resume_UID = A.Fixture_Resume_UID,
                                Data_Source = A.Data_Source,
                                Fixture_M_UID = A.Fixture_M_UID,
                                Modified_Date = A.Modified_Date
                            }).ToList();
                return list;
            }
            else //有可能超过一周需要超history表
            {
                var linq = (from A in DataContext.Fixture_Resume
                            select new Batch_FMTNew
                            {
                                Fixture_Resume_UID = A.Fixture_Resume_UID,
                                Data_Source = A.Data_Source,
                                Fixture_M_UID = A.Fixture_M_UID,
                                Resume_Notes = A.Resume_Notes,
                                Modified_Date = A.Modified_Date
                            }).Union
                            (
                            from A in DataContext.Fixture_Resume_History
                            select new Batch_FMTNew
                            {
                                Fixture_Resume_UID = A.Fixture_Resume_UID,
                                Data_Source = A.Data_Source,
                                Fixture_M_UID = A.Fixture_M_UID,
                                Resume_Notes = A.Resume_Notes,
                                Modified_Date = A.Modified_Date
                            });
                linq = linq.Where(A => A.Data_Source == "4" && A.Resume_Notes.Equals(freeStr) && A.Modified_Date >= dateStart && A.Modified_Date <= dateEnd && fixTotalUIDList.Contains(A.Fixture_M_UID));
                var list = linq.ToList();
                return list;
            }
        }

        //获取该制程的当日送修数
        private List<Batch_FMTRepair> GetCurrentDayRepairList(List<int> fixTotalUIDList, DateTime StartDate, DateTime EndDate)
        {
            var repairList = (from A in DataContext.Fixture_Repair_M
                              join B in DataContext.Fixture_Repair_D
                              on A.Fixture_Repair_M_UID equals B.Fixture_Repair_M_UID
                              where fixTotalUIDList.Contains(B.Fixture_M_UID) && A.SentOut_Date >= StartDate
                              && A.SentOut_Date <= EndDate
                              select new Batch_FMTRepair
                              {
                                  Fixture_Repair_D_UID = B.Fixture_Repair_D_UID,
                                  Fixture_M_UID = B.Fixture_M_UID
                              }).ToList();

            return repairList;
        }

        //获取该制程的当日领用数
        private List<Batch_FMTTotake> GetCurrentDayTotakeList(List<int> fixTotalUIDList, DateTime StartDate, DateTime EndDate)
        {
            var shipList = (from A in DataContext.Fixture_Totake_M
                            join B in DataContext.Fixture_Totake_D
                            on A.Fixture_Totake_M_UID equals B.Fixture_Totake_M_UID
                            where fixTotalUIDList.Contains(B.Fixture_M_UID) && A.Ship_Date >= StartDate
                            && A.Ship_Date <= EndDate
                            select new Batch_FMTTotake
                            {
                                Fixture_Totake_D_UID = B.Fixture_Totake_D_UID,
                                Fixture_M_UID = B.Fixture_M_UID
                            }).ToList();
            return shipList;
        }

        //获取该制程的当日待修数
        private List<Batch_FMTRepair> GetCurrentDayNotRepairList(List<int> fixTotalUIDList, DateTime StartDate, DateTime EndDate)
        {
            var waitRepair = (from A in DataContext.Fixture_Repair_M
                              join B in DataContext.Fixture_Repair_D
                              on A.Fixture_Repair_M_UID equals B.Fixture_Repair_M_UID
                              where fixTotalUIDList.Contains(B.Fixture_M_UID) && A.SentOut_Date >= StartDate
                              && A.SentOut_Date <= EndDate && B.Completion_Date == null
                              select new Batch_FMTRepair
                              {
                                  Fixture_Repair_D_UID = B.Fixture_Repair_D_UID,
                                  Fixture_M_UID = B.Fixture_M_UID
                              }).ToList();

            return waitRepair;
        }

        //获取该制程的当日应保养数，已保养数，未保养数
        private List<Batch_FMTHasMainten> GetCurrentDayHasMaintenList(List<int> fixTotalUIDList, DateTime StartDate, DateTime EndDate)
        {
            if (StartDate.AddDays(6) > DateTime.Now) //一周之内
            {
                var hasMainten = (from A in DataContext.Fixture_Maintenance_Record
                                  where fixTotalUIDList.Contains(A.Fixture_M_UID)
                                  //&& A.Maintenance_Status == 1 这句话不能要，因为应保养数，已保养数，未保养数都需要统计
                                  //不能以Maintenance_Date为查询条件，治具保养记录创建后是未保养状态Maintenance_Date=null
                                  && A.Created_Date >= StartDate && A.Created_Date <= EndDate
                                  && A.Modified_UID != ConstConstants.AdminUID
                                  select new Batch_FMTHasMainten
                                  {
                                      Fixture_Maintenance_Record_UID = A.Fixture_Maintenance_Record_UID,
                                      Fixture_M_UID = A.Fixture_M_UID,
                                      Maintenance_Status = A.Maintenance_Status,
                                      Maintenance_Date = A.Maintenance_Date,
                                      Modified_UID = A.Modified_UID
                                  }).ToList();
                return hasMainten;
            }
            else //有可能超过一周需要超history表
            {
                var hasMainten = (from A in DataContext.Fixture_Maintenance_Record
                                  select new Batch_FMTHasMainten
                                  {
                                      Fixture_Maintenance_Record_UID = A.Fixture_Maintenance_Record_UID,
                                      Fixture_M_UID = A.Fixture_M_UID,
                                      Maintenance_Status = A.Maintenance_Status,
                                      Created_Date = A.Created_Date,
                                      Modified_UID = A.Modified_UID
                                  }).Union
                                  (
                                    from A in DataContext.Fixture_Maintenance_Record_History
                                    select new Batch_FMTHasMainten
                                    {
                                        Fixture_Maintenance_Record_UID = A.Fixture_Maintenance_Record_UID,
                                        Fixture_M_UID = A.Fixture_M_UID,
                                        Maintenance_Status = A.Maintenance_Status,
                                        Created_Date = A.Created_Date,
                                        Modified_UID = A.Modified_UID
                                    });

                hasMainten = hasMainten.Where(A => A.Created_Date >= StartDate && A.Created_Date <= EndDate
                && fixTotalUIDList.Contains(A.Fixture_M_UID) && A.Modified_UID != ConstConstants.AdminUID
                //A.Maintenance_Status == 1 这句话不能要，因为应保养数，已保养数，未保养数都需要统计
                );
                var list = hasMainten.ToList();
                return list;
            }
        }

        //获取该制程的当日未保养数
        private List<Batch_FMTHasMainten> GetCurrentDayNotMaintenList(List<int> fixTotalUIDList, DateTime StartDate, DateTime EndDate)
        {
            if (StartDate.AddDays(6) > DateTime.Now) //一周之内
            {
                var notMainten = (from A in DataContext.Fixture_Maintenance_Record
                                  where fixTotalUIDList.Contains(A.Fixture_M_UID) && A.Maintenance_Status == 0
                                  && A.Maintenance_Date >= StartDate && A.Maintenance_Date <= EndDate
                                  select new Batch_FMTHasMainten
                                  {
                                      Fixture_Maintenance_Record_UID = A.Fixture_Maintenance_Record_UID,
                                      Fixture_M_UID = A.Fixture_M_UID,
                                      Maintenance_Status = A.Maintenance_Status,
                                      Maintenance_Date = A.Maintenance_Date
                                  }).ToList();
                return notMainten;

            }
            else
            {
                var notMainten = (from A in DataContext.Fixture_Maintenance_Record
                                      //where fixTotalUIDList.Contains(A.Fixture_M_UID) && A.Maintenance_Status == 0
                                      //&& A.Maintenance_Date >= StartDate && A.Maintenance_Date <= EndDate
                                  select new Batch_FMTHasMainten
                                  {
                                      Fixture_Maintenance_Record_UID = A.Fixture_Maintenance_Record_UID,
                                      Fixture_M_UID = A.Fixture_M_UID,
                                      Maintenance_Status = A.Maintenance_Status,
                                      Maintenance_Date = A.Maintenance_Date
                                  }).Union
                                  (from A in DataContext.Fixture_Maintenance_Record_History
                                   select new Batch_FMTHasMainten
                                   {
                                       Fixture_Maintenance_Record_UID = A.Fixture_Maintenance_Record_UID,
                                       Fixture_M_UID = A.Fixture_M_UID,
                                       Maintenance_Status = A.Maintenance_Status,
                                       Maintenance_Date = A.Maintenance_Date
                                   });

                notMainten = notMainten.Where(A => A.Maintenance_Status == 0 && A.Maintenance_Date >= StartDate && A.Maintenance_Date <= EndDate && fixTotalUIDList.Contains(A.Fixture_M_UID));
                var list = notMainten.ToList();
                return list;
            }
        }

        public List<Batch_ReportByStatus> ExportReportByFMTValid(Batch_ReportByStatus model)
        {
            int count = 0;
            DateTime StartDate = model.StartDate.Value.Date.AddDays(-1).AddHours(8);
            DateTime EndDate = model.StartDate.Value.Date.AddHours(8);

            return QueryFixtureReportByFMT(model, null, out count, true, StartDate, EndDate);
        }

        #endregion 报表-FMT Dashboard Add by Rock 2017-12-11----------------End

        #region 主执行程序 -------------------Add By Rock 2017-12-18---------Start
        public void ExecFMTDashboard(string functionName, int Plant_Organization_UID, int System_Schedule_UID)
        {
            //DateTime StartDate = Convert.ToDateTime("2017-12-14 08:00:00");
            //DateTime EndDate = Convert.ToDateTime("2017-11-15 08:00:00");
            DateTime StartDate = DateTime.Now.Date.AddDays(-1).AddHours(8);
            DateTime EndDate = DateTime.Now.Date.AddHours(8);

            #region 第一步：生成主表的数据
            //var Plant_Organization_UID = DataContext.System_Organization.Where(m => m.Organization_Name == "WUXI_M").Select(m => m.Organization_UID).First();
            Batch_ReportByStatus model = new Batch_ReportByStatus();
            model.Plant_Organization_UID = Plant_Organization_UID;
            model.BG_Organization_UID = 0;
            model.FunPlant_Organization_UID = 0;
            model.StartDate = StartDate;
            model.EndDate = EndDate;
            int count = 0;
            var list = QueryFixtureReportByFMT(model, null, out count, true, StartDate, EndDate);
            #endregion

            #region 第二步：生成明细表的数据
            List<Batch_ReportByStatus> detailList = new List<Batch_ReportByStatus>();
            int SheetCount = 1;
            foreach (var item in list)
            {
                var details = QueryQueryFixtureReportByFMTDetail(item.Process_Info_UID, StartDate, EndDate, SheetCount);
                detailList.AddRange(details);
                SheetCount++;
            }
            #endregion

            #region 第三步：导出Excel
            //var fullName = ExportFMTExcel(list, detailList, StartDate, EndDate);
            #endregion



            #region 第四步： 查找需要发送邮件的人员将邮件内容插入邮件发送排程
            if (list.Count() > 0)
            {
                var sendEmaillinq = from A in DataContext.System_Schedule
                                    join B in DataContext.System_Email_Delivery
                                    on A.System_Schedule_UID equals B.System_Schedule_UID
                                    join C in DataContext.System_Function
                                    on A.Function_UID equals C.Function_UID

                                    where C.Function_Name == functionName && A.Is_Enable == true && B.Is_Enable == true && B.Plant_Organization_UID == Plant_Organization_UID
                                    select new SystemModuleEmailVM
                                    {
                                        Plant_Organization_UID = B.Plant_Organization_UID,
                                        BG_Organization_UID = B.BG_Organization_UID,
                                        System_Schedule_UID = A.System_Schedule_UID,
                                        Function_UID = A.Function_UID,
                                        Email = B.Email
                                    };
                var sendList = sendEmaillinq.ToList();
                System_Email_M emailItem = new System_Email_M();

                if (sendList.Count() > 0)
                {
                    var subject = "[Notice mail / FMT DashBoard Remind / for your notice]";
                    var emailList = sendList.Select(m => m.Email).ToList();
                    var to_List = DataContext.System_Users.Where(m => emailList.Contains(m.Email)).Select(m => m.Account_UID).ToList();

                    string toEmail = string.Join(",", emailList);

                    var body = SetEmailContext(list, detailList, StartDate, EndDate);

                    EmailFormat format = new EmailFormat();
                    EmailFormat.layout lay1 = format.Layout_SapErrorData(subject, body);


                    emailItem.System_Schedule_UID = System_Schedule_UID;
                    emailItem.Subject = lay1.Layout_Subject;
                    emailItem.Body = lay1.Layout_Content;
                    emailItem.Email_From = StructConstants.Email_From.PIS_Email_From;
                    emailItem.Email_To = toEmail;
                    //如果是系统用户则直接存AccountUID，如果不是则不存。可以通过Email_To查找出所有发送邮件的对象
                    emailItem.Email_To_UIDs = string.Join(",", to_List);
                    emailItem.Is_Send = false;
                    emailItem.Email_Type = 1;
                    emailItem.Modified_UID = ConstConstants.AdminUID;
                    emailItem.Modified_Date = DateTime.Now;
                    DataContext.System_Email_M.Add(emailItem);
                    DataContext.SaveChanges();

                }

            }
            #endregion


        }

        //第三步：导出Excel
        private string ExportFMTExcel(List<Batch_ReportByStatus> masterList, List<Batch_ReportByStatus> detailAllList, DateTime StartDate, DateTime EndDate)
        {
            string time = DateTime.Now.ToString("yyyyMMddhhmmss");
            string path = AppDomain.CurrentDomain.BaseDirectory;
            string fullName = string.Format("{0}ReportCenter\\FMTDashboard_{1}.xlsx", path, time);
            FileInfo newFile = new FileInfo(fullName);
            if (newFile.Exists)
            {
                newFile.Delete();
                newFile = new FileInfo(fullName);
            }
            //var fileName = PathHelper.SetGridExportExcelName("FMT Dashboard");
            string[] propertiesHead = GetMasterColumn();
            string[] propertiesDetaiHead = GetDetailColumn();
            using (var excelPackage = new ExcelPackage(newFile))
            {
                if (masterList.Count() > 0)
                {
                    var masterTime = string.Format("{0}  ~   {1}", StartDate.ToString("yyyy-MM-dd hh:mm:ss"), EndDate.ToString("yyyy-MM-dd hh:mm:ss"));
                    var worksheet = excelPackage.Workbook.Worksheets.Add("FMT Dashboard");
                    SetStatusStyle(worksheet, propertiesHead, masterTime);
                    //Excel sheet名称防止sheet名称重复导致不能创建Excel
                    int sheetCount = 1;
                    int currentRow = 4;
                    foreach (var item in masterList)
                    {
                        worksheet.Cells[currentRow, 1].Value = currentRow - 3;
                        worksheet.Cells[currentRow, 2].Value = item.PlantName;
                        worksheet.Cells[currentRow, 3].Value = item.OpType_Name;
                        worksheet.Cells[currentRow, 4].Value = item.Func_Name;
                        worksheet.Cells[currentRow, 5].Value = item.Process_Name;
                        worksheet.Cells[currentRow, 6].Value = item.TotalCount;
                        worksheet.Cells[currentRow, 7].Value = item.NewCount;
                        worksheet.Cells[currentRow, 8].Value = item.FreeCount;
                        worksheet.Cells[currentRow, 9].Value = item.SendRepairCount;
                        worksheet.Cells[currentRow, 10].Value = item.ShipCount;
                        worksheet.Cells[currentRow, 11].Value = item.WaitRepairCount;
                        worksheet.Cells[currentRow, 12].Value = item.NeedMaintenCount;
                        worksheet.Cells[currentRow, 13].Value = item.HasMaintenCount;
                        worksheet.Cells[currentRow, 14].Value = item.NotMaintenCount;
                        currentRow++;


                        //生成明细表
                        var maxSheetCount = detailAllList.Select(m => m.SheetCount).Max();
                        if (sheetCount <= maxSheetCount)
                        {
                            var detailList = detailAllList.Where(m => m.SheetCount == sheetCount).ToList();
                            var detailworksheet = excelPackage.Workbook.Worksheets.Add(item.Process_Name + "_" + sheetCount);
                            SetFMTDetailStyle(detailworksheet, propertiesDetaiHead, masterTime);
                            int detailCurrentRow = 4;

                            foreach (var detailItem in detailList)
                            {
                                detailworksheet.Cells[detailCurrentRow, 1].Value = detailCurrentRow - 3;
                                detailworksheet.Cells[detailCurrentRow, 2].Value = item.PlantName;
                                detailworksheet.Cells[detailCurrentRow, 3].Value = item.OpType_Name;
                                detailworksheet.Cells[detailCurrentRow, 4].Value = item.Func_Name;
                                detailworksheet.Cells[detailCurrentRow, 5].Value = item.Process_Name;
                                detailworksheet.Cells[detailCurrentRow, 6].Value = detailItem.WorkStation_Name;
                                detailworksheet.Cells[detailCurrentRow, 7].Value = detailItem.TotalCount;
                                detailworksheet.Cells[detailCurrentRow, 8].Value = detailItem.NewCount;
                                detailworksheet.Cells[detailCurrentRow, 9].Value = detailItem.FreeCount;
                                detailworksheet.Cells[detailCurrentRow, 10].Value = detailItem.SendRepairCount;
                                detailworksheet.Cells[detailCurrentRow, 11].Value = detailItem.ShipCount;
                                detailworksheet.Cells[detailCurrentRow, 12].Value = detailItem.WaitRepairCount;
                                detailworksheet.Cells[detailCurrentRow, 13].Value = detailItem.NeedMaintenCount;
                                detailworksheet.Cells[detailCurrentRow, 14].Value = detailItem.HasMaintenCount;
                                detailworksheet.Cells[detailCurrentRow, 15].Value = detailItem.NotMaintenCount;
                                detailCurrentRow++;
                            }
                            sheetCount++;
                        }
                    }

                    excelPackage.Save();
                }
            }
            return fullName;
        }


        //第四步：设置Email内容
        private string SetEmailContext(List<Batch_ReportByStatus> masterList, List<Batch_ReportByStatus> detailAllList, DateTime StartDate, DateTime EndDate)
        {
            var masterTime = string.Format("{0}  ~   {1}", StartDate.ToString("yyyy-MM-dd hh:mm:ss"), EndDate.ToString("yyyy-MM-dd hh:mm:ss"));

            StringBuilder sb = new StringBuilder();
            sb.Append("<!--LayContent-->");
            sb.Append("<div style=\"margin: 0px 0px 20px 0px;\">Dear Sir / Madam,<br />");
            sb.Append("以下是FMT Dashoboard 治具使用情况统计，请查看.");
            sb.Append("</div>");


            sb.Append("<div style=\"margin: 5px; border: 1px solid #B5B5B5; padding: 10px 10px 20px 10px; border-radius: 5px\">");
            sb.Append("<div style=\"margin-bottom: 2px; margin-top: 0px\">");
            sb.Append("<span style=\"font-size: 16px; color: #00764B; padding: 5px 0px 0px 0px; margin: 0px\">▎</span>");
            sb.Append("<span style=\"font-size: 16px; color: #333; padding: 5px 0px 5px 0px; margin: 0px\">");
            sb.Append(string.Format("<b>治具使用情况总计 统计时间: {0}</b>", masterTime));
            sb.Append("</span>");
            sb.Append("</div>");
            sb.Append("<div>");
            sb.Append("<table width=\"100%\" border=\"1\" cellpadding=\"5\" cellspacing=\"0\" style=\"border: 1px solid #EEEEEE; border-collapse: collapse;\">");
            sb.Append("<tbody>");

            sb.Append("<tr>");
            sb.Append("<th style=\"background: #546A74; color: #fff;\">序号</th>");
            sb.Append("<th style=\"background: #546A74; color: #fff;\">厂区</th>");
            sb.Append("<th style=\"background: #546A74; color: #fff;\">OP类型</th>");
            sb.Append("<th style=\"background: #546A74; color: #fff;\">功能厂</th>");
            sb.Append("<th style=\"background: #546A74; color: #fff;\">制程</th>");
            sb.Append("<th style=\"background: #546A74; color: #fff;\">当前治具总数</th>");
            sb.Append("<th style=\"background: #546A74; color: #fff;\">当日新增</th>");
            sb.Append("<th style=\"background: #546A74; color: #fff;\">当日报废</th>");
            sb.Append("<th style=\"background: #546A74; color: #fff;\">当日送修数</th>");
            sb.Append("<th style=\"background: #546A74; color: #fff;\">当日领用数</th>");
            sb.Append("<th style=\"background: #546A74; color: #fff;\">待修数</th>");
            sb.Append("<th style=\"background: #546A74; color: #fff;\">应保养数</th>");
            sb.Append("<th style=\"background: #546A74; color: #fff;\">已保养数</th>");
            sb.Append("<th style=\"background: #546A74; color: #fff;\">未保养数</th>");
            sb.Append("</tr>");


            int i = 1;
            foreach (var item in masterList)
            {
                sb.Append("<tr>");
                sb.Append(string.Format("<td>{0}</td>", i));
                sb.Append(string.Format("<td align='left'>{0}</td>", item.PlantName));
                sb.Append(string.Format("<td align='left'>{0}</td>", item.OpType_Name));
                sb.Append(string.Format("<td align='left'>{0}</td>", item.Func_Name));
                sb.Append(string.Format("<td align='left'>{0}</td>", item.Process_Name));
                sb.Append(string.Format("<td align='right'>{0}</td>", item.TotalCount));
                sb.Append(string.Format("<td align='right'>{0}</td>", item.NewCount));
                sb.Append(string.Format("<td align='right'>{0}</td>", item.FreeCount));
                sb.Append(string.Format("<td align='right'>{0}</td>", item.SendRepairCount));
                sb.Append(string.Format("<td align='right'>{0}</td>", item.ShipCount));
                sb.Append(string.Format("<td align='right'>{0}</td>", item.WaitRepairCount));
                sb.Append(string.Format("<td align='right'>{0}</td>", item.NeedMaintenCount));
                sb.Append(string.Format("<td align='right'>{0}</td>", item.HasMaintenCount));
                sb.Append(string.Format("<td align='right'>{0}</td>", item.NotMaintenCount));
                sb.Append("</tr>");
                i++;
            }
            sb.Append("</tbody>");
            sb.Append("</table>");
            sb.Append("<br/>");



            var maxSheetCount = detailAllList.Select(m => m.SheetCount).Max();
            var firstItem = masterList.First();
            for (int sheetCount = 1; sheetCount <= maxSheetCount; sheetCount++)
            {
                //for(int x=0; x<3; x++)
                //{
                sb.Append(string.Format("<b>治具使用情况明细总计 统计时间: {0}</b>", masterTime));
                sb.Append("<table width=\"100%\" border=\"1\" cellpadding=\"5\" cellspacing=\"0\" style=\"border: 1px solid #EEEEEE; border-collapse: collapse;\">");
                sb.Append("<tbody>");

                sb.Append("<tr>");
                sb.Append("<th style=\"background: #546A74; color: #fff;\">序号</th>");
                sb.Append("<th style=\"background: #546A74; color: #fff;\">厂区</th>");
                sb.Append("<th style=\"background: #546A74; color: #fff;\">OP类型</th>");
                sb.Append("<th style=\"background: #546A74; color: #fff;\">功能厂</th>");
                sb.Append("<th style=\"background: #546A74; color: #fff;\">制程</th>");
                sb.Append("<th style=\"background: #546A74; color: #fff;\">工站</th>");
                sb.Append("<th style=\"background: #546A74; color: #fff;\">当前治具总数</th>");
                sb.Append("<th style=\"background: #546A74; color: #fff;\">当日新增</th>");
                sb.Append("<th style=\"background: #546A74; color: #fff;\">当日报废</th>");
                sb.Append("<th style=\"background: #546A74; color: #fff;\">当日送修数</th>");
                sb.Append("<th style=\"background: #546A74; color: #fff;\">当日领用数</th>");
                sb.Append("<th style=\"background: #546A74; color: #fff;\">待修数</th>");
                sb.Append("<th style=\"background: #546A74; color: #fff;\">应保养数</th>");
                sb.Append("<th style=\"background: #546A74; color: #fff;\">已保养数</th>");
                sb.Append("<th style=\"background: #546A74; color: #fff;\">未保养数</th>");
                sb.Append("</tr>");

                var detailList = detailAllList.Where(m => m.SheetCount == sheetCount).ToList();

                int j = 1;
                foreach (var detailItem in detailList)
                {
                    sb.Append("<tr>");
                    sb.Append(string.Format("<td>{0}</td>", j));
                    sb.Append(string.Format("<td align='left'>{0}</td>", firstItem.PlantName));
                    sb.Append(string.Format("<td align='left'>{0}</td>", firstItem.OpType_Name));
                    sb.Append(string.Format("<td align='left'>{0}</td>", firstItem.Func_Name));
                    sb.Append(string.Format("<td align='left'>{0}</td>", detailItem.Process_Name));
                    sb.Append(string.Format("<td align='left'>{0}</td>", detailItem.WorkStation_Name));
                    sb.Append(string.Format("<td align='right'>{0}</td>", detailItem.TotalCount));
                    sb.Append(string.Format("<td align='right'>{0}</td>", detailItem.NewCount));
                    sb.Append(string.Format("<td align='right'>{0}</td>", detailItem.FreeCount));
                    sb.Append(string.Format("<td align='right'>{0}</td>", detailItem.SendRepairCount));
                    sb.Append(string.Format("<td align='right'>{0}</td>", detailItem.ShipCount));
                    sb.Append(string.Format("<td align='right'>{0}</td>", detailItem.WaitRepairCount));
                    sb.Append(string.Format("<td align='right'>{0}</td>", detailItem.NeedMaintenCount));
                    sb.Append(string.Format("<td align='right'>{0}</td>", detailItem.HasMaintenCount));
                    sb.Append(string.Format("<td align='right'>{0}</td>", detailItem.NotMaintenCount));
                    sb.Append("</tr>");
                    j++;
                }
                sb.Append("</tbody>");
                sb.Append("</table>");

                //}
                /*


                */

                sb.Append("</div>");
                sb.Append("</div>");
                sb.Append("<!--LayContent-->");
            }
            return sb.ToString();
        }


        private void SetStatusStyle(ExcelWorksheet worksheet, string[] propertiesHead, string masterTime)
        {
            worksheet.Cells[1, 1].Value = "治具使用情况总计";
            worksheet.Cells[2, 1].Value = masterTime;
            for (int colIndex = 0; colIndex < propertiesHead.Length; colIndex++)
            {
                worksheet.Cells[3, colIndex + 1].Value = propertiesHead[colIndex];
                worksheet.Column(colIndex + 1).Width = 10;
                worksheet.Cells["A1:N1"].Style.Font.Bold = true;
                worksheet.Cells["A1:N1"].Style.Fill.PatternType = ExcelFillStyle.Solid;
                worksheet.Cells["A1:N1"].Style.Fill.BackgroundColor.SetColor(Color.Gray);
            }
            worksheet.Column(6).Width = 20;
            worksheet.Cells["A1:N1"].Merge = true;
            worksheet.Cells["A1"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
        }

        private void SetFMTDetailStyle(ExcelWorksheet worksheet, string[] propertiesHead, string masterTime)
        {
            worksheet.Cells[1, 1].Value = "治具使用情况明细";
            worksheet.Cells[2, 1].Value = masterTime;
            for (int colIndex = 0; colIndex < propertiesHead.Length; colIndex++)
            {
                worksheet.Cells[3, colIndex + 1].Value = propertiesHead[colIndex];
                worksheet.Column(colIndex + 1).Width = 10;
                worksheet.Cells["A1:O1"].Style.Font.Bold = true;
                worksheet.Cells["A1:O1"].Style.Fill.PatternType = ExcelFillStyle.Solid;
                worksheet.Cells["A1:O1"].Style.Fill.BackgroundColor.SetColor(Color.Gray);
            }
            worksheet.Column(6).Width = 25;
            worksheet.Column(7).Width = 15;
            worksheet.Cells["A1:O1"].Merge = true;
            worksheet.Cells["A1"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

        }

        public string[] GetMasterColumn()
        {
            var propertiesHead = new[]
            {
                "序号",
                "厂区",
                "OP类型",
                "功能厂",
                "制程",
                "当前治具总数",
                "当日新增",
                "当日报废",
                "当日送修数",
                "当日领用数",
                "待修数",
                "应保养数",
                "已保养数",
                "未保养数"
            };
            return propertiesHead;
        }

        public string[] GetDetailColumn()
        {
            var propertiesHead = new[]
            {
                "序号",
                "厂区",
                "OP类型",
                "功能厂",
                "制程",
                "工站",
                "当前治具总数",
                "当日新增",
                "当日报废",
                "当日送修数",
                "当日领用数",
                "待修数",
                "应保养数",
                "已保养数",
                "未保养数"
            };
            return propertiesHead;
        }

        #endregion 主执行程序 -------------------Add By Rock 2017-12-18---------End


        public List<FixtureDTO> GetFixture_MByPlant(int Plant_Organization_UID)
        {

            var query = from fixture in DataContext.Fixture_M
                        select new FixtureDTO
                        {
                            Fixture_M_UID = fixture.Fixture_M_UID,
                            Plant_Organization_UID = fixture.Plant_Organization_UID,
                            BG_Organization_UID = fixture.BG_Organization_UID,
                            FunPlant_Organization_UID = fixture.FunPlant_Organization_UID,
                            Fixture_NO = fixture.Fixture_NO,
                            Version = fixture.Version,
                            Fixture_Seq = fixture.Fixture_Seq,
                            Fixture_Unique_ID = fixture.Fixture_Unique_ID,
                            Fixture_Name = fixture.Fixture_Name,
                            Createder = fixture.System_Users.User_Name,
                            Modifieder = fixture.System_Users1.User_Name
                        };

            query = query.Where(m => m.Plant_Organization_UID == Plant_Organization_UID);
            return query.ToList();
        }
        public int GetFixture_MCount(int Plant_Organization_UID, int BG_Organization_UID, string Fixture_Unique_ID)
        {
            var query = from fixture in DataContext.Fixture_M
                        select new FixtureDTO
                        {
                            Fixture_M_UID = fixture.Fixture_M_UID,
                            Plant_Organization_UID = fixture.Plant_Organization_UID,
                            BG_Organization_UID = fixture.BG_Organization_UID,
                            FunPlant_Organization_UID = fixture.FunPlant_Organization_UID,
                            Fixture_NO = fixture.Fixture_NO,
                            Version = fixture.Version,
                            Fixture_Seq = fixture.Fixture_Seq,
                            Fixture_Unique_ID = fixture.Fixture_Unique_ID,
                            Fixture_Name = fixture.Fixture_Name,
                            Createder = fixture.System_Users.User_Name,
                            Modifieder = fixture.System_Users1.User_Name
                        };

            query = query.Where(m => m.Plant_Organization_UID == Plant_Organization_UID && m.BG_Organization_UID == BG_Organization_UID && m.Fixture_Unique_ID == Fixture_Unique_ID);
            return query.Count();
        }

        public FixtureDTO GetFixtureDTO(int plantID, int optypeID, string SN)
        {

            var query = from fixture in DataContext.Fixture_M
                        where fixture.Plant_Organization_UID == plantID && fixture.BG_Organization_UID == optypeID && fixture.Fixture_Unique_ID == SN
                        select new FixtureDTO
                        {
                            Fixture_M_UID = fixture.Fixture_M_UID,
                            Plant_Organization_UID = fixture.Plant_Organization_UID,
                            BG_Organization_UID = fixture.BG_Organization_UID,
                            FunPlant_Organization_UID = fixture.FunPlant_Organization_UID,
                            Fixture_NO = fixture.Fixture_NO,
                            Version = fixture.Version,
                            Fixture_Seq = fixture.Fixture_Seq,
                            Fixture_Unique_ID = fixture.Fixture_Unique_ID,
                            Fixture_Name = fixture.Fixture_Name,
                            Project_UID = fixture.Project_UID,
                            Fixture_Machine_UID = fixture.Fixture_Machine_UID,
                            Vendor_Info_UID = fixture.Vendor_Info_UID,
                            Production_Line_UID = fixture.Production_Line_UID,
                            Status = fixture.Status,
                            StatuName = fixture.Enumeration.Enum_Value,
                            ShortCode = fixture.ShortCode,
                            TwoD_Barcode = fixture.TwoD_Barcode,
                            Created_Date = fixture.Created_Date,
                            Created_UID = fixture.Created_UID,
                            Modified_Date = fixture.Modified_Date,
                            Modified_UID = fixture.Modified_UID,
                            Equipment_No = fixture.Fixture_Machine.Equipment_No,
                            Machine_Type = fixture.Fixture_Machine.Machine_Type,
                            Machine_ID = fixture.Fixture_Machine.Machine_ID,
                            Line_Name = fixture.Production_Line.Line_Name,
                            Workshop = fixture.Production_Line.Workshop.Workshop_Name,
                            Workshop_UID = fixture.Production_Line.Workshop_UID,
                            Workstation = fixture.Production_Line.WorkStation.WorkStation_Name,
                            Workstation_UID = fixture.Production_Line.Workstation_UID,
                            Process_Info = fixture.Production_Line.Process_Info.Process_Name,
                            Process_Info_UID = fixture.Production_Line.Process_Info_UID,
                            Project = fixture.System_Project.Project_Name,
                            Createder = fixture.System_Users.User_Name,
                            Modifieder = fixture.System_Users1.User_Name,
                            UseTimesTotal = fixture.UseTimesTotal,
                            PlantName = fixture.System_Organization.Organization_Name,
                            OPName = fixture.System_Organization1.Organization_Name,
                            FunPlantName = fixture.System_Organization2.Organization_Name
                        };
      
            //return query.FirstOrDefault(m => m.Plant_Organization_UID == plantID && m.BG_Organization_UID == optypeID && m.Fixture_Unique_ID == SN);
            return SetFixtureDTO(query.FirstOrDefault());
        }

        public FixtureDTO SetFixtureDTO(FixtureDTO Fixtures)
        {
            if (Fixtures!=null)
            {
                List<FixtureStatuDTO> fixtureStatuDTOs = GetFixtureStatuDTO();
                var status = fixtureStatuDTOs.Where(o => o.Status == Fixtures.Status).FirstOrDefault();
                if (status != null)
                {
                    Fixtures.StatuName = status.StatuName;
                }
            }    
            return Fixtures;
        }
        public Fixture_Part_Setting_MDTO GetFixture_Part_Setting_MDTO(int plantID, int optypeID, string Fixture_NO)
        {
            var query = from fixture_Part_Setting_M in DataContext.Fixture_Part_Setting_M
                        where fixture_Part_Setting_M.Is_Enable == true
                        select new Fixture_Part_Setting_MDTO
                        {
                            Fixture_Part_Setting_M_UID = fixture_Part_Setting_M.Fixture_Part_Setting_M_UID,
                            Plant_Organization_UID = fixture_Part_Setting_M.Plant_Organization_UID,
                            BG_Organization_UID = fixture_Part_Setting_M.BG_Organization_UID,
                            FunPlant_Organization_UID = fixture_Part_Setting_M.FunPlant_Organization_UID,
                            Fixture_NO = fixture_Part_Setting_M.Fixture_NO,
                            Line_Qty = fixture_Part_Setting_M.Line_Qty,
                            Line_Fixture_Ratio_Qty = fixture_Part_Setting_M.Line_Fixture_Ratio_Qty,
                            UseTimesScanInterval = fixture_Part_Setting_M.UseTimesScanInterval,
                            Is_Enable = fixture_Part_Setting_M.Is_Enable,
                            Created_UID = fixture_Part_Setting_M.Created_UID,
                        };

            return query.FirstOrDefault(m => m.Plant_Organization_UID == plantID && m.BG_Organization_UID == optypeID && m.Fixture_NO == Fixture_NO);
        }


        public List<Fixture_Part_Setting_DDTO> GetFixture_Part_Setting_DDTOs(int fixture_Part_Setting_M_UID)
        {

            var query = from fixture_Part_Setting_D in DataContext.Fixture_Part_Setting_D
                        where fixture_Part_Setting_D.Is_Enable == true && fixture_Part_Setting_D.IsUseTimesManagement == true
                        select new Fixture_Part_Setting_DDTO
                        {
                            Fixture_Part_Setting_D_UID = fixture_Part_Setting_D.Fixture_Part_Setting_D_UID,
                            Fixture_Part_Setting_M_UID = fixture_Part_Setting_D.Fixture_Part_Setting_M_UID,
                            Fixture_Part_UID = fixture_Part_Setting_D.Fixture_Part_UID,
                            Fixture_Part_Qty = fixture_Part_Setting_D.Fixture_Part_Qty,
                            Fixture_Part_Life = fixture_Part_Setting_D.Fixture_Part_Life,
                            IsUseTimesManagement = fixture_Part_Setting_D.IsUseTimesManagement,
                            Fixture_Part_Life_UseTimes = fixture_Part_Setting_D.Fixture_Part_Life_UseTimes.Value,
                            Is_Enable = fixture_Part_Setting_D.Is_Enable,
                            Created_UID = fixture_Part_Setting_D.Created_UID,
                            Created_Date = fixture_Part_Setting_D.Created_Date,
                        };

            return query.Where(m => m.Fixture_Part_Setting_M_UID == fixture_Part_Setting_M_UID).ToList();


        }

        public Fixture_M_UseScanHistoryDTO GetFixture_M_UseScanHistoryDTO(int Fixture_M_UID)
        {
            var query = from fixture_M_UseScanHistoryDTO in DataContext.Fixture_M_UseScanHistory
                        select new Fixture_M_UseScanHistoryDTO
                        {
                            Fixture_M_UseScanHistory_UID = fixture_M_UseScanHistoryDTO.Fixture_M_UseScanHistory_UID,
                            Fixture_M_UID = fixture_M_UseScanHistoryDTO.Fixture_M_UID,
                            UseTimesTotal = fixture_M_UseScanHistoryDTO.UseTimesTotal,
                            IsValidSan = fixture_M_UseScanHistoryDTO.IsValidSan,
                            ScanDateTime = fixture_M_UseScanHistoryDTO.ScanDateTime,
                        };
            query = query.OrderByDescending(o => o.Fixture_M_UseScanHistory_UID);
            return query.FirstOrDefault(m => m.Fixture_M_UID == Fixture_M_UID);
        }

        public List<Fixture_Part_UseTimesDTO> GetFixture_Part_UseTimesDTO(int Fixture_M_UID)
        {
            var query = from fixture_Part_UseTimesDTO in DataContext.Fixture_Part_UseTimes
                        select new Fixture_Part_UseTimesDTO
                        {
                            Fixture_Part_UseTimes_UID = fixture_Part_UseTimesDTO.Fixture_Part_UseTimes_UID,
                            Fixture_M_UID = fixture_Part_UseTimesDTO.Fixture_M_UID,
                            Fixture_Part_Setting_D_UID = fixture_Part_UseTimesDTO.Fixture_Part_Setting_D_UID,
                            Fixture_Part_UseTimesCount = fixture_Part_UseTimesDTO.Fixture_Part_UseTimesCount
                        };

            return query.Where(m => m.Fixture_M_UID == Fixture_M_UID).ToList();
        }



        public string SaveFixturePartScanCodeDTO(string strsql)
        {
            try
            {

                using (var trans = DataContext.Database.BeginTransaction())
                {

                    DataContext.Database.ExecuteSqlCommand(strsql);
                    trans.Commit();
                }
                return "0";

            }
            catch (Exception e)
            {
                return e.Message;
            }
        }

        public List<FixturePartScanDTO> GetFixturePartScanDTOs(int Fixture_M_UID)
        {

            var query = from fixture_Part_UseTimesDTO in DataContext.Fixture_Part_UseTimes
                            // where fixture_Part_UseTimesDTO.Fixture_M_UID == Fixture_M_UID
                        select new FixturePartScanDTO
                        {
                            Fixture_M_UID = fixture_Part_UseTimesDTO.Fixture_M_UID,
                            Fixture_Part_Setting_D_UID = fixture_Part_UseTimesDTO.Fixture_Part_Setting_D_UID,
                            Fixture_Part_UID = fixture_Part_UseTimesDTO.Fixture_Part_Setting_D.Fixture_Part_UID,
                            Part_ID = fixture_Part_UseTimesDTO.Fixture_Part_Setting_D.Fixture_Part.Part_ID,
                            Part_Name = fixture_Part_UseTimesDTO.Fixture_Part_Setting_D.Fixture_Part.Part_Name,
                            Part_Spec = fixture_Part_UseTimesDTO.Fixture_Part_Setting_D.Fixture_Part.Part_Spec,
                            Fixture_Part_Qty = fixture_Part_UseTimesDTO.Fixture_Part_Setting_D.Fixture_Part_Qty,
                            Fixture_Part_Life_UseTimes = fixture_Part_UseTimesDTO.Fixture_Part_Setting_D.Fixture_Part_Life_UseTimes,
                            UseTimes = fixture_Part_UseTimesDTO.Fixture_Part_UseTimesCount,
                            IsUseTimesManagement = fixture_Part_UseTimesDTO.Fixture_Part_Setting_D.IsUseTimesManagement
                        };

            return query.Where(m => m.Fixture_M_UID == Fixture_M_UID && m.IsUseTimesManagement == true).ToList();

        }
    }
}

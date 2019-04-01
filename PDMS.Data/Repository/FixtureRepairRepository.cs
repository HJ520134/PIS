using PDMS.Data.Infrastructure;
using PDMS.Model;
using PDMS.Model.ViewModels;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using PDMS.Common.Helpers;
using PDMS.Model.EntityDTO;

namespace PDMS.Data.Repository
{

    public interface IFixtureRepairRepository : IRepository<Fixture_Repair_M>
    {

        IQueryable<FixtureRepairDTO> QueryFixtureRepairs(FixtureRepairSearch searchModel, Page page, out int totalcount);
        IQueryable<FixtureRepairDTO> QueryFixtureRepairsByQuery(FixtureRepairSearch searchModel);
        FixtureRepairItem QueryFixtureRepairByNo(string Repair_NO);
        List<Fixture_Repair_MDTO> GetFixture_Repair_MDTOList(int Plant_Organization_UID, int BG_Organization_UID, int FunPlant_Organization_UID);
        Fixture_Repair_MDTO GetFixture_Repair_MDTOByID(int Fixture_Repair_M_UID);

        string GetSentRepairNameById(string SentOut_Number);
    }
    public class FixtureRepairRepository : RepositoryBase<Fixture_Repair_M>, IFixtureRepairRepository
    {
        public FixtureRepairRepository(IDatabaseFactory databaseFactory)
            : base(databaseFactory)
        {

        }

        public IQueryable<FixtureRepairDTO> QueryFixtureRepairs(FixtureRepairSearch searchModel, Page page, out int totalcount)
        {
            var query = from fd in DataContext.Fixture_Repair_D
                        select new FixtureRepairDTO
                        {
                            Fixture_Repair_D_UID = fd.Fixture_Repair_D_UID,
                            Fixture_Repair_M_UID = fd.Fixture_Repair_M.Fixture_Repair_M_UID,
                            Plant_Organization_UID = fd.Fixture_Repair_M.Plant_Organization_UID,
                            BG_Organization_UID = fd.Fixture_Repair_M.BG_Organization_UID,
                            FunPlant_Organization_UID = fd.Fixture_Repair_M.FunPlant_Organization_UID,
                            Repair_NO = fd.Fixture_Repair_M.Repair_NO,
                            Plant_Organization_Name = fd.Fixture_Repair_M.System_Organization.Organization_Name,
                            BG_Organization_Name = fd.Fixture_Repair_M.System_Organization1.Organization_Name,
                            FunPlant_Organization_Name = fd.Fixture_Repair_M.System_Organization2.Organization_Name,
                            Repair_Location_UID = fd.Fixture_Repair_M.Repair_Location_UID,
                            Repair_Location_Name = fd.Fixture_Repair_M.Repair_Location.Repair_Location_Name,
                            Production_Line_Name = fd.Fixture_M.Production_Line.Line_Name,
                            Vendor_Info_UID = fd.Fixture_M.Vendor_Info_UID,
                            Production_Line_UID = fd.Fixture_M.Production_Line_UID,
                            Workstation = fd.Fixture_M.Production_Line.WorkStation.WorkStation_Name,
                            Workstation_UID = fd.Fixture_M.Production_Line.Workstation_UID,
                            Process_Info = fd.Fixture_M.Production_Line.Process_Info.Process_Name,
                            Process_Info_UID = fd.Fixture_M.Production_Line.Process_Info_UID,
                            Workshop = fd.Fixture_M.Production_Line.Workshop.Workshop_Name,
                            Workshop_UID = fd.Fixture_M.Production_Line.Workshop_UID,
                            Project = fd.Fixture_M.System_Project.Project_Name,
                            Fixture_Machine_UID = fd.Fixture_M.Fixture_Machine_UID,
                            Equipment_No = fd.Fixture_M.Fixture_Machine.Equipment_No,
                            Fixture_NO = fd.Fixture_M.Fixture_NO,
                            Fixture_Name = fd.Fixture_M.Fixture_Name,
                            Fixture_Unique_ID = fd.Fixture_M.Fixture_Unique_ID,
                            Status = fd.Status,
                            StatusName = fd.Enumeration.Enum_Value,
                            ShortCode = fd.Fixture_M.ShortCode,
                            Created_Date = fd.Created_Date,
                            Created_UID = fd.Created_UID,
                            Modified_Date = fd.Modified_Date,
                            Modified_UID = fd.Modified_UID,
                            Modified_UserName = fd.System_Users1.User_Name,
                            Modified_UserNTID = fd.System_Users1.User_NTID
                        };

            if (string.IsNullOrWhiteSpace(searchModel.ExportUIds))
            {
                //厂区
                if (searchModel.Plant_Organization_UID != 0)
                    query = query.Where(m => m.Plant_Organization_UID == searchModel.Plant_Organization_UID);
                //OP
                if (searchModel.BG_Organization_UID != 0)
                    query = query.Where(m => m.BG_Organization_UID == searchModel.BG_Organization_UID);
                //功能厂
                if (searchModel.FunPlant_Organization_UID.HasValue && searchModel.FunPlant_Organization_UID.Value != 0)
                    query = query.Where(m => m.FunPlant_Organization_UID == searchModel.FunPlant_Organization_UID.Value);
                //维修单号
                if (!string.IsNullOrWhiteSpace(searchModel.Repair_NO))
                    query = query.Where(m => m.Repair_NO.Contains(searchModel.Repair_NO));
                //治具编号
                if (!string.IsNullOrWhiteSpace(searchModel.Fixture_NO))
                    query = query.Where(m => m.Fixture_NO.Contains(searchModel.Fixture_NO));
                //治具名称
                if (!string.IsNullOrWhiteSpace(searchModel.Fixture_Name))
                    query = query.Where(m => m.Fixture_Name.Contains(searchModel.Fixture_Name));
                //供应商
                if (searchModel.Vendor_Info_UID.HasValue && searchModel.Vendor_Info_UID.Value != 0)
                    query = query.Where(m => m.Vendor_Info_UID == searchModel.Vendor_Info_UID.Value);
                //产线
                if (searchModel.Production_Line_UID.HasValue && searchModel.Production_Line_UID != 0)
                    query = query.Where(m => m.Production_Line_UID == searchModel.Production_Line_UID);
                //状态
                if (searchModel.Status.HasValue && searchModel.Status.Value != 0)
                    query = query.Where(m => m.Status == searchModel.Status);
                //断码
                if (!string.IsNullOrWhiteSpace(searchModel.ShortCode))
                    query = query.Where(m => m.ShortCode.Contains(searchModel.ShortCode));
                //机台
                if (searchModel.Fixture_Machine_UID.HasValue && searchModel.Status.Value != 0)
                    query = query.Where(m => m.Fixture_Machine_UID == searchModel.Fixture_Machine_UID.Value);
                //工站
                if (searchModel.Workstation_UID.HasValue)
                    query = query.Where(m => m.Workstation_UID == searchModel.Workstation_UID.Value);
                //制程
                if (searchModel.Process_Info_UID.HasValue)
                    query = query.Where(m => m.Process_Info_UID == searchModel.Process_Info_UID);
                //车间
                if (searchModel.Workshop_UID.HasValue)
                    query = query.Where(m => m.Workshop_UID == searchModel.Workshop_UID.Value);
                //修改人
                if (!string.IsNullOrWhiteSpace(searchModel.Modified_UserNTID))
                    query = query.Where(m => m.Modified_UserNTID == searchModel.Modified_UserNTID);
                //日期
                if (searchModel.End_Date_From.HasValue)
                    query = query.Where(m => m.Modified_Date >= searchModel.End_Date_From.Value);
                if (searchModel.End_Date_To.HasValue)
                {
                    searchModel.End_Date_To = searchModel.End_Date_To.Value.AddDays(1);
                    query = query.Where(m => m.Modified_Date < searchModel.End_Date_To.Value);
                }

                query = query.OrderBy(w => w.Plant_Organization_UID).ThenBy(w => w.BG_Organization_UID).ThenBy(w => w.Fixture_Repair_D_UID);
                totalcount = query.Count();
                query = query.GetPage(page);
                return query;
            }
            else
            {
                var array = Array.ConvertAll(searchModel.ExportUIds.Split(','), s => int.Parse(s));
                query = query.Where(p => array.Contains(p.Fixture_Repair_D_UID)).OrderBy(w => w.Plant_Organization_UID).ThenBy(w => w.BG_Organization_UID).ThenBy(w => w.Fixture_Repair_D_UID);
                totalcount = 0;
                return query;
            }
        }
        public IQueryable<FixtureRepairDTO> QueryFixtureRepairsByQuery(FixtureRepairSearch searchModel)
        {
            var query = from fd in DataContext.Fixture_Repair_D
                        select new FixtureRepairDTO
                        {
                            Fixture_Repair_D_UID = fd.Fixture_Repair_D_UID,
                            Fixture_Repair_M_UID = fd.Fixture_Repair_M.Fixture_Repair_M_UID,
                            Plant_Organization_UID = fd.Fixture_Repair_M.Plant_Organization_UID,
                            BG_Organization_UID = fd.Fixture_Repair_M.BG_Organization_UID,
                            FunPlant_Organization_UID = fd.Fixture_Repair_M.FunPlant_Organization_UID,
                            Repair_NO = fd.Fixture_Repair_M.Repair_NO,
                            Plant_Organization_Name = fd.Fixture_Repair_M.System_Organization.Organization_Name,
                            BG_Organization_Name = fd.Fixture_Repair_M.System_Organization1.Organization_Name,
                            FunPlant_Organization_Name = fd.Fixture_Repair_M.System_Organization2.Organization_Name,
                            Repair_Location_UID = fd.Fixture_Repair_M.Repair_Location_UID,
                            Repair_Location_Name = fd.Fixture_Repair_M.Repair_Location.Repair_Location_Name,
                            Production_Line_Name = fd.Fixture_M.Production_Line.Line_Name,
                            Vendor_Info_UID = fd.Fixture_M.Vendor_Info_UID,
                            Production_Line_UID = fd.Fixture_M.Production_Line_UID,
                            Workstation = fd.Fixture_M.Production_Line.WorkStation.WorkStation_Name,
                            Workstation_UID = fd.Fixture_M.Production_Line.Workstation_UID,
                            Process_Info = fd.Fixture_M.Production_Line.Process_Info.Process_Name,
                            Process_Info_UID = fd.Fixture_M.Production_Line.Process_Info_UID,
                            Workshop = fd.Fixture_M.Production_Line.Workshop.Workshop_Name,
                            Workshop_UID = fd.Fixture_M.Production_Line.Workshop_UID,
                            Project = fd.Fixture_M.System_Project.Project_Name,
                            Fixture_Machine_UID = fd.Fixture_M.Fixture_Machine_UID,
                            Equipment_No = fd.Fixture_M.Fixture_Machine.Equipment_No,
                            Fixture_NO = fd.Fixture_M.Fixture_NO,
                            Fixture_Name = fd.Fixture_M.Fixture_Name,
                            Fixture_Unique_ID = fd.Fixture_M.Fixture_Unique_ID,
                            Status = fd.Status,
                            StatusName = fd.Enumeration.Enum_Value,
                            ShortCode = fd.Fixture_M.ShortCode,
                            Created_Date = fd.Created_Date,
                            Created_UID = fd.Created_UID,
                            Modified_Date = fd.Modified_Date,
                            Modified_UID = fd.Modified_UID,
                            Modified_UserName = fd.System_Users1.User_Name,
                            Modified_UserNTID = fd.System_Users1.User_NTID
                        };

            //厂区
            if (searchModel.Plant_Organization_UID != 0)
                query = query.Where(m => m.Plant_Organization_UID == searchModel.Plant_Organization_UID);
            //OP
            if (searchModel.BG_Organization_UID != 0)
                query = query.Where(m => m.BG_Organization_UID == searchModel.BG_Organization_UID);
            //功能厂
            if (searchModel.FunPlant_Organization_UID.HasValue && searchModel.FunPlant_Organization_UID.Value != 0)
                query = query.Where(m => m.FunPlant_Organization_UID == searchModel.FunPlant_Organization_UID.Value);
            //维修单号
            if (!string.IsNullOrWhiteSpace(searchModel.Repair_NO))
                query = query.Where(m => m.Repair_NO.Contains(searchModel.Repair_NO));
            //治具编号
            if (!string.IsNullOrWhiteSpace(searchModel.Fixture_NO))
                query = query.Where(m => m.Fixture_NO.Contains(searchModel.Fixture_NO));
            //治具名称
            if (!string.IsNullOrWhiteSpace(searchModel.Fixture_Name))
                query = query.Where(m => m.Fixture_Name.Contains(searchModel.Fixture_Name));
            //供应商
            if (searchModel.Vendor_Info_UID.HasValue && searchModel.Vendor_Info_UID.Value != 0)
                query = query.Where(m => m.Vendor_Info_UID == searchModel.Vendor_Info_UID.Value);
            //产线
            if (searchModel.Production_Line_UID.HasValue && searchModel.Production_Line_UID != 0)
                query = query.Where(m => m.Production_Line_UID == searchModel.Production_Line_UID);
            //状态
            if (searchModel.Status.HasValue && searchModel.Status.Value != 0)
                query = query.Where(m => m.Status == searchModel.Status);
            //断码
            if (!string.IsNullOrWhiteSpace(searchModel.ShortCode))
                query = query.Where(m => m.ShortCode.Contains(searchModel.ShortCode));
            //机台
            if (searchModel.Fixture_Machine_UID.HasValue && searchModel.Status.Value != 0)
                query = query.Where(m => m.Fixture_Machine_UID == searchModel.Fixture_Machine_UID.Value);
            //工站
            if (searchModel.Workstation_UID.HasValue)
                query = query.Where(m => m.Workstation_UID == searchModel.Workstation_UID.Value);
            //制程
            if (searchModel.Process_Info_UID.HasValue)
                query = query.Where(m => m.Process_Info_UID == searchModel.Process_Info_UID);
            //车间
            if (searchModel.Workshop_UID.HasValue)
                query = query.Where(m => m.Workshop_UID == searchModel.Workshop_UID.Value);
            //修改人
            if (!string.IsNullOrWhiteSpace(searchModel.Modified_UserNTID))
                query = query.Where(m => m.Modified_UserNTID == searchModel.Modified_UserNTID);
            //日期
            if (searchModel.End_Date_From.HasValue)
                query = query.Where(m => m.Modified_Date >= searchModel.End_Date_From.Value);
            if (searchModel.End_Date_To.HasValue)
            {
                searchModel.End_Date_To = searchModel.End_Date_To.Value.AddDays(1);
                query = query.Where(m => m.Modified_Date < searchModel.End_Date_To.Value);
            }

            query = query.OrderBy(w => w.Plant_Organization_UID).ThenBy(w => w.BG_Organization_UID).ThenBy(w => w.Fixture_Repair_D_UID);

            return query;
        }

        public IQueryable<FixtureRepairDTO> SetFixtureDTO(List<FixtureRepairDTO> Fixtures)
        {

            List<FixtureStatuDTO> fixtureStatuDTOs = GetFixtureStatuDTO();
            foreach (var item in Fixtures)
            {

                var status = fixtureStatuDTOs.Where(o => o.Status == item.Status).FirstOrDefault();
                if (status != null)
                {
                    item.StatusName = status.StatuName;
                }

            }
            return Fixtures.AsQueryable();
        }
        /// <summary>
        /// 组装数据
        /// </summary>
        /// <param name="Fixtures"></param>
        /// <returns></returns>

        /// <summary>
        /// 根据ID获取当前的治具资料
        /// </summary>
        /// <param name="fixture_UID"></param>
        /// <returns></returns>
        public FixtureRepairItem QueryFixtureRepairByNo(string Repair_NO)
        {

            FixtureRepairItem returnItem = new FixtureRepairItem();
            var query = from FM in DataContext.Fixture_Repair_M
                        where FM.Repair_NO == Repair_NO
                        select FM;



            foreach (var item in query)
            {
                //给主档信息赋值
                returnItem.Fixture_Repair_M_UID = item.Fixture_Repair_M_UID;
                returnItem.Plant_Organization_Name = item.System_Organization.Organization_Name;
                returnItem.BG_Organization_Name = item.System_Organization1.Organization_Name;
                returnItem.FunPlant_Organization_Name = item.System_Organization2.Organization_Name;
                returnItem.Plant_Organization_UID = item.Plant_Organization_UID;
                returnItem.BG_Organization_UID = item.BG_Organization_UID;
                returnItem.FunPlant_Organization_UID = item.FunPlant_Organization_UID;
                returnItem.Receiver_Name = item.System_Users1.User_Name;
                //returnItem.SentOut_Name = item.System_Users.User_Name;
                returnItem.SentOut_Number = item.SentOut_Number;
                returnItem.SentOut_Name = item.SentOut_Name;
                returnItem.Receiver_UID = item.Receiver_UID;
                returnItem.Repair_NO = item.Repair_NO;
                returnItem.Repair_Location_UID = item.Repair_Location_UID;
                returnItem.Repair_Location_Name = item.Repair_Location.Repair_Location_Name;
                returnItem.SentOut_Time = item.SentOut_Date;
                //获取维修明细
                var queryD = from FD in DataContext.Fixture_Repair_D
                             where FD.Fixture_Repair_M_UID == item.Fixture_Repair_M_UID
                             select FD;

                List<FixtureRepairDItem> FixtureRepairDItems = new List<FixtureRepairDItem>();
                FixtureRepairDItem FRD = new FixtureRepairDItem();
                foreach (var FD in queryD)
                {

                    //给维修明细明细当赋值
                    //FRD.Completion_Date = FD.Completion_Date;

                }


            }

            return null;
        }
        /// <summary>
        /// 设置加载厂区，OP，功能厂，厂商
        /// </summary>
        /// <param name="Fixtures"></param>
        /// <returns></returns>
        public List<FixtureDTO> SetListFixtureDTO(List<FixtureDTO> Fixtures)
        {

            List<FixtureStatuDTO> fixtureStatuDTOs = GetFixtureStatuDTO();
            foreach (var item in Fixtures)
            {

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
            //List<FixtureStatuDTO> fixtureStatuDTOs = new List<FixtureStatuDTO>();
            //fixtureStatuDTOs.Add(new FixtureStatuDTO() { Status = 0, StatuName = "" });
            //fixtureStatuDTOs.Add(new FixtureStatuDTO() { Status = 1, StatuName = "使用中In - PRD" });
            //fixtureStatuDTOs.Add(new FixtureStatuDTO() { Status = 2, StatuName = "未使用Non - PRD" });
            //fixtureStatuDTOs.Add(new FixtureStatuDTO() { Status = 3, StatuName = "維修中In - Repair" });
            //fixtureStatuDTOs.Add(new FixtureStatuDTO() { Status = 4, StatuName = "報廢Scrap" });
            //fixtureStatuDTOs.Add(new FixtureStatuDTO() { Status = 5, StatuName = "返供應商維修RTV" });
            //fixtureStatuDTOs.Add(new FixtureStatuDTO() { Status = 6, StatuName = "保養逾時Over - Due Maintenance" });

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
                            WorkStation_Name = production_Line.WorkStation_Name,
                            Plant_Organization_UID = production_Line.Plant_Organization_UID,
                            BG_Organization_UID = production_Line.BG_Organization_UID,
                            FunPlant_Organization_UID = production_Line.FunPlant_Organization_UID,
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

        public List<Process_InfoDTO> GetProcess_InfoList(int Plant_Organization_UID, int BG_Organization_UID, int FunPlant_Organization_UID)
        {
            var query = from production_Line in DataContext.Process_Info
                        select new Process_InfoDTO
                        {
                            Process_Info_UID = production_Line.Process_Info_UID,
                            Plant_Organization_UID = production_Line.Plant_Organization_UID,
                            BG_Organization_UID = production_Line.BG_Organization_UID,
                            FunPlant_Organization_UID = production_Line.FunPlant_Organization_UID,
                            Process_Name = production_Line.Process_Name,
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

        public List<SystemProjectDTO> GetProjectList(int Plant_Organization_UID, int BG_Organization_UID, int FunPlant_Organization_UID)
        {

            var query = from production_Line in DataContext.System_Project
                        select new SystemProjectDTO
                        {
                            //Production_Line_UID = production_Line.Production_Line_UID,
                            //Plant_Organization_UID = production_Line.Plant_Organization_UID,
                            //BG_Organization_UID = production_Line.BG_Organization_UID,
                            //FunPlant_Organization_UID = production_Line.FunPlant_Organization_UID,
                            //Is_Enable = production_Line.Is_Enable
                        };
            //query = query.Where(m => m.Is_Enable == true);
            //if (Plant_Organization_UID != 0)
            //    query = query.Where(m => m.Plant_Organization_UID == Plant_Organization_UID);
            //if (BG_Organization_UID != 0)
            //    query = query.Where(m => m.BG_Organization_UID == BG_Organization_UID);
            //if (FunPlant_Organization_UID != 0)
            //    query = query.Where(m => m.FunPlant_Organization_UID == FunPlant_Organization_UID);
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
                            Created_UID = fixture_Machine.Created_UID

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
                    return "";
                else
                    return "删除治具记录失败";
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
                            Line_Name = fixture.Production_Line.Line_Name,
                            Workshop = fixture.Production_Line.Workshop.Workshop_Name,
                            Workshop_UID = fixture.Production_Line.Workshop_UID,
                            Workstation = fixture.Production_Line.WorkStation.WorkStation_Name,
                            Workstation_UID = fixture.Production_Line.Workstation_UID,
                            Process_Info = fixture.Production_Line.Process_Info.Process_Name,
                            Process_Info_UID = fixture.Production_Line.Process_Info_UID,
                            Project = fixture.System_Project.Project_Name
                        };
            query = query.Where(m => Fixture_M_UIDs.Contains("," + m.Fixture_M_UID + ","));
            var fixtures = SetListFixtureDTO(query.ToList());
            return fixtures;

        }

        #region 治具履历查询-----------------------Add by Rock 2017-10-03 -----------Start
        public List<FixtureResumeSearchVM> FixtureResumeSearchVM(FixtureResumeSearchVM searchVM, Page page, out int totalcount)
        {
            totalcount = 0;
            var linq = from A in DataContext.Fixture_Resume
                       join B in DataContext.Fixture_M
                       on A.Fixture_M_UID equals B.Fixture_M_UID
                       join C in DataContext.Production_Line
                       on B.Production_Line_UID equals C.Production_Line_UID
                       join D in DataContext.Process_Info
                       on C.Process_Info_UID equals D.Process_Info_UID
                       join E in DataContext.WorkStation
                       on C.Workstation_UID equals E.WorkStation_UID
                       join F in DataContext.Fixture_Machine
                       on B.Fixture_Machine_UID equals F.Fixture_Machine_UID
                       join G in DataContext.Vendor_Info
                       on B.Vendor_Info_UID equals G.Vendor_Info_UID
                       select A;

            return null;

        }

        #endregion 治具履历查询-----------------------Add by Rock 2017-10-03 -----------end
        public List<Fixture_Repair_MDTO> GetFixture_Repair_MDTOList(int Plant_Organization_UID, int BG_Organization_UID, int FunPlant_Organization_UID)
        {
            var query = from fd in DataContext.Fixture_Repair_M
                        select new Fixture_Repair_MDTO
                        {

                            Fixture_Repair_M_UID = fd.Fixture_Repair_M_UID,
                            Plant_Organization_UID = fd.Plant_Organization_UID,
                            BG_Organization_UID = fd.BG_Organization_UID,
                            FunPlant_Organization_UID = fd.FunPlant_Organization_UID,
                            Plant_Organization_Name = fd.System_Organization.Organization_Name,
                            BG_Organization_Name = fd.System_Organization1.Organization_Name,
                            FunPlant_Organization_Name = fd.System_Organization2.Organization_Name,
                            Repair_NO = fd.Repair_NO,
                            Repair_Location_UID = fd.Repair_Location_UID,
                            Repair_Location_Name = fd.Repair_Location.Repair_Location_Name,
                            SentOut_Number = fd.SentOut_Number,
                            SentOut_Name = fd.SentOut_Name,
                            Receiver_UID = fd.Receiver_UID,
                            //Receiver_NTID = fd.Receiver_NTID,
                            //Receiver_Name = fd.Receiver_Name,
                            SentOut_Date = fd.SentOut_Date,
                            Created_Date = fd.Created_Date,
                            Created_UID = fd.Created_UID,
                            Modified_Date = fd.Modified_Date,
                            Modified_UID = fd.Modified_UID,
                            Modified_UserName = fd.System_Users1.User_Name,
                            Modified_UserNTID = fd.System_Users1.User_NTID
                        };

            if (Plant_Organization_UID != 0)
                query = query.Where(m => m.Plant_Organization_UID == Plant_Organization_UID);
            if (BG_Organization_UID != 0)
                query = query.Where(m => m.BG_Organization_UID == BG_Organization_UID);
            if (FunPlant_Organization_UID != 0)
                query = query.Where(m => m.FunPlant_Organization_UID == FunPlant_Organization_UID);
            return query.ToList();
        }

        public Fixture_Repair_MDTO GetFixture_Repair_MDTOByID(int Fixture_Repair_M_UID)
        {

            var query = from fd in DataContext.Fixture_Repair_M
                        select new Fixture_Repair_MDTO
                        {

                            Fixture_Repair_M_UID = fd.Fixture_Repair_M_UID,
                            Plant_Organization_UID = fd.Plant_Organization_UID,
                            BG_Organization_UID = fd.BG_Organization_UID,
                            FunPlant_Organization_UID = fd.FunPlant_Organization_UID,
                            Plant_Organization_Name = fd.System_Organization.Organization_Name,
                            BG_Organization_Name = fd.System_Organization1.Organization_Name,
                            FunPlant_Organization_Name = fd.System_Organization2.Organization_Name,
                            Repair_NO = fd.Repair_NO,
                            Repair_Location_UID = fd.Repair_Location_UID,
                            Repair_Location_Name = fd.Repair_Location.Repair_Location_Name,
                            SentOut_Number = fd.SentOut_Number,
                            SentOut_Name = fd.SentOut_Name,
                            Receiver_UID = fd.Receiver_UID,
                            //Receiver_NTID = fd.Receiver_NTID,
                            //Receiver_Name = fd.Receiver_Name,
                            SentOut_Date = fd.SentOut_Date,
                            Created_Date = fd.Created_Date,
                            Created_UID = fd.Created_UID,
                            Modified_Date = fd.Modified_Date,
                            Modified_UID = fd.Modified_UID,
                            Modified_UserName = fd.System_Users1.User_Name,
                            Modified_UserNTID = fd.System_Users1.User_NTID
                        };

            if (Fixture_Repair_M_UID != 0)
                query = query.Where(m => m.Fixture_Repair_M_UID == Fixture_Repair_M_UID);
            return query.FirstOrDefault();
        }

        public string GetSentRepairNameById(string SentOut_Number)
        {
            var query = from fd in DataContext.Fixture_Repair_M
                        select new Fixture_Repair_MDTO
                        {
                            Fixture_Repair_M_UID = fd.Fixture_Repair_M_UID,
                            Plant_Organization_UID = fd.Plant_Organization_UID,
                            BG_Organization_UID = fd.BG_Organization_UID,
                            FunPlant_Organization_UID = fd.FunPlant_Organization_UID,
                            Plant_Organization_Name = fd.System_Organization.Organization_Name,
                            BG_Organization_Name = fd.System_Organization1.Organization_Name,
                            FunPlant_Organization_Name = fd.System_Organization2.Organization_Name,
                            Repair_NO = fd.Repair_NO,
                            Repair_Location_UID = fd.Repair_Location_UID,
                            Repair_Location_Name = fd.Repair_Location.Repair_Location_Name,
                            SentOut_Number = fd.SentOut_Number,
                            SentOut_Name = fd.SentOut_Name,
                            Receiver_UID = fd.Receiver_UID,
                            SentOut_Date = fd.SentOut_Date,
                            Created_Date = fd.Created_Date,
                            Created_UID = fd.Created_UID,
                            Modified_Date = fd.Modified_Date,
                            Modified_UID = fd.Modified_UID,
                            Modified_UserName = fd.System_Users1.User_Name,
                            Modified_UserNTID = fd.System_Users1.User_NTID
                        };
            if (!string.IsNullOrEmpty(SentOut_Number))
            {
                var result = query.Where(p => p.SentOut_Number == SentOut_Number);
                if (result.Count() > 0)
                {
                    return result.OrderByDescending(q => q.Modified_Date).FirstOrDefault().SentOut_Name;
                }
                else
                {
                    return string.Empty;
                }
            }
            else
            {
                return string.Empty;
            }
        }
    }
}
